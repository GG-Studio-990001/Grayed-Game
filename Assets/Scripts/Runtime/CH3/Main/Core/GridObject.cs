using System;
using UnityEngine;

namespace Runtime.CH3.Main
{
    public class GridObject : MonoBehaviour, IGridObject
    {
        [SerializeField] protected GridObjectType objectType;  // Inspector에서 설정할 수 있도록
        [SerializeField] protected Vector2Int gridPosition;
        public GridObjectType ObjectType => objectType;
        public Vector2Int GridPosition => gridPosition;
        public GameObject GameObject => gameObject;  // GameObject 속성 구현

        [Header("Position Overrides")]
        [SerializeField] protected bool useCustomY = false;
        [SerializeField] protected float customY = 0.53f;

        [Header("Initial Sorting (Grid-Based)")]
        [SerializeField] protected bool applyInitialGridSorting = true;
        [SerializeField] protected int gridSortingScale = 1;

        protected SpriteRenderer spriteRenderer;
        private MinimapManager minimapManager;
        protected GridSystem gridManager;

        protected virtual void Start()
        {
            // spriteRenderer 초기화 (Initialize에서 설정되지 않은 경우)
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            
            // GridSystem.CreateObject에서 Initialize를 호출하므로 여기서는 호출하지 않음
            // 단, GridSystem이 없는 경우에만 기본 초기화 수행
            if (gridManager == null)
            {
                gridManager = GridSystem.Instance;
                if (gridManager != null)
                {
                    // 이미 월드 위치에 있는 오브젝트의 경우 그리드 위치만 동기화 (월드 위치 재설정은 하지 않음)
                    Vector2Int calculatedGridPos = gridManager.WorldToGridPosition(transform.position);
                    if (gridManager.IsValidGridPosition(calculatedGridPos))
                    {
                        gridPosition = calculatedGridPos;
                        
                        // 그리드 위치가 설정되었으므로 SortingOrder도 업데이트
                        if (applyInitialGridSorting)
                        {
                            var sorting = GetComponent<SortingOrderObject>();
                            if (sorting != null)
                            {
                                // 뒤(y가 작을수록)일수록 더 높은 정렬이 되도록 부호 반전
                                sorting.SetBaseOrder(-gridPosition.y * gridSortingScale);
                            }
                        }
                    }
                }
            }
            else
            {
                // gridManager가 이미 있는 경우에도 SortingOrder가 설정되지 않았으면 설정
                // Initialize가 호출되지 않은 경우(씬에 직접 배치된 오브젝트)를 대비
                if (applyInitialGridSorting && gridPosition != Vector2Int.zero)
                {
                    var sorting = GetComponent<SortingOrderObject>();
                    if (sorting != null)
                    {
                        // baseOrder가 0이고 gridPosition이 설정되어 있으면 그리드 기반으로 설정
                        // SortingOrderObject의 baseOrder는 private이므로 직접 확인 불가
                        // 대신 spriteRenderer의 sortingOrder를 확인하여 기본값이면 설정
                        if (spriteRenderer != null && spriteRenderer.sortingOrder == 0)
                        {
                            sorting.SetBaseOrder(-gridPosition.y * gridSortingScale);
                        }
                    }
                }
            }
        }
        
        //TODO: Vector2Int 없애기
        public virtual void Initialize(Vector2Int gridPos)
        {
            gridManager = GridSystem.Instance;
            minimapManager = FindObjectOfType<MinimapManager>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            minimapManager.CreateMinimapIcon(transform);
            
            // gridPosition을 먼저 설정
            gridPosition = gridPos;
            
            // 월드 위치 설정 (useCustomY 고려)
            Vector3 worldPos = gridManager.GridToWorldPosition(gridPos);
            if (useCustomY)
            {
                worldPos.y = customY;
            }
            transform.position = worldPos;
            
            // 그리드 셀 점유 상태 설정은 하위 클래스에서 처리
            // (Structure 등에서 블록 설정과 함께 처리)

            // 초기 정렬: 그리드 y(앞/뒤) 기준으로 baseOrder 오프셋 적용
            if (applyInitialGridSorting)
            {
                var sorting = GetComponent<SortingOrderObject>();
                if (sorting != null)
                {
                    // 뒤(y가 작을수록)일수록 더 높은 정렬이 되도록 부호 반전
                    sorting.SetBaseOrder(-gridPosition.y * gridSortingScale);
                }
            }
        }

        protected Vector3 GetWorldPositionForGrid(Vector2Int desiredGridPos)
        {
            if (gridManager == null)
                gridManager = GridSystem.Instance;
            Vector3 world = gridManager != null
                ? gridManager.GridToWorldPosition(desiredGridPos)
                : transform.position;
            if (useCustomY)
                world.y = customY;
            return world;
        }

        public virtual void UpdateGridPosition()
        {
            if (gridManager == null)
                return;

            Vector2Int newGridPos = gridManager.WorldToGridPosition(transform.position);

            if (gridManager.IsValidGridPosition(newGridPos))
            {
                Vector2Int oldGridPos = gridPosition;
                
                GridCell oldCell = gridManager.GetCell(gridPosition);
                if (oldCell != null)
                {
                    oldCell.IsOccupied = false;
                    oldCell.OccupyingObject = null;
                }

                gridPosition = newGridPos;
                GridCell newCell = gridManager.GetCell(gridPosition);
                if (newCell != null)
                {
                    newCell.IsOccupied = true;
                    newCell.OccupyingObject = gameObject;
                }
                
                // 그리드 위치가 변경되었을 때 SortingOrder 업데이트
                if (applyInitialGridSorting && oldGridPos != newGridPos)
                {
                    var sorting = GetComponent<SortingOrderObject>();
                    if (sorting != null)
                    {
                        // 뒤(y가 작을수록)일수록 더 높은 정렬이 되도록 부호 반전
                        sorting.SetBaseOrder(-gridPosition.y * gridSortingScale);
                    }
                }
            }
        }

        public virtual void Remove()
        {
            minimapManager.RemoveMinimapIcon(transform);
            Destroy(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (gridManager != null)
            {
                GridCell cell = gridManager.GetCell(gridPosition);
                if (cell != null)
                {
                    cell.IsOccupied = false;
                    cell.OccupyingObject = null;
                }
            }
        }
    }
}
