using System;
using System.Collections.Generic;
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

        [Header("Tile Size")]
        [Tooltip("오브젝트가 차지할 타일 크기 (기본값: 1x1). 오브젝트 중심을 기준으로 확장됩니다.")]
        [SerializeField] protected Vector2Int tileSize = Vector2Int.one;
        public Vector2Int TileSize => tileSize;

        /// <summary>
        /// 그리드 위치 초기화 방식
        /// </summary>
        public enum GridPositionInitializationMode
        {
            /// <summary>인스펙터에서 설정한 GridPosition 좌표로 오브젝트를 이동시킵니다.</summary>
            UseInspectorPosition,
            /// <summary>현재 월드 위치에서 가장 가까운 그리드 좌표로 오브젝트를 이동시킵니다.</summary>
            UseNearestGridPosition
        }
        
        [Header("Grid Position Initialization")]
        [Tooltip("그리드 위치 초기화 방식 선택")]
        [SerializeField] protected GridPositionInitializationMode gridPositionMode = GridPositionInitializationMode.UseNearestGridPosition;

        [Header("Position Overrides")]
        [SerializeField] protected bool useCustomY = false;
        [SerializeField] protected float customY = 0.53f;

        [Header("Initial Sorting (Grid-Based)")]
        [Tooltip("그리드 y 좌표를 기반으로 초기 정렬 순서를 설정합니다.")]
        [SerializeField] protected bool applyInitialGridSorting = true;
        [SerializeField] protected int gridSortingScale = 1;

        [Header("Child Object References (Optional)")]
        [Tooltip("그리드 볼륨: 타일 점유/차단의 기준이 되는 자식 오브젝트. 비어있으면 자동으로 찾습니다.")]
        [SerializeField] protected Transform gridVolumeTransform;
        [Tooltip("이미지 리소스: SpriteRenderer가 있는 자식 오브젝트. 비어있으면 자동으로 찾습니다.")]
        [SerializeField] protected Transform spriteTransform;
        [Tooltip("자식 오브젝트 자동 바인딩 활성화")]
        [SerializeField] protected bool autoBindChildren = true;

        protected SpriteRenderer spriteRenderer;
        private MinimapManager minimapManager;
        protected GridSystem gridManager;
        
        // 점유 중인 모든 타일 위치를 추적
        protected List<Vector2Int> occupiedTiles = new List<Vector2Int>();
        
        // 건축 시스템에서 사용하는 건물 ID (건물 파괴 시 카운트 감소용)
        protected string buildingId;
        
        /// <summary>
        /// 타일 점유/차단의 기준이 되는 Transform을 반환합니다.
        /// gridVolumeTransform이 설정되어 있으면 그것을, 없으면 최상단 오브젝트를 반환합니다.
        /// </summary>
        protected Transform GridVolumeTransform => gridVolumeTransform != null ? gridVolumeTransform : transform;
        
        /// <summary>
        /// SpriteRenderer가 있는 Transform을 반환합니다.
        /// spriteTransform이 설정되어 있으면 그것을, 없으면 최상단 오브젝트를 반환합니다.
        /// </summary>
        protected Transform SpriteTransform => spriteTransform != null ? spriteTransform : transform;
        
        /// <summary>
        /// 외부 컴포넌트(OcclusionFader, SortingOrderObject 등)에서 접근할 수 있도록 public 프로퍼티 제공
        /// </summary>
        public Transform GetSpriteTransform() => SpriteTransform;
        
        /// <summary>
        /// 외부 컴포넌트에서 접근할 수 있도록 public 프로퍼티 제공
        /// </summary>
        public Transform GetGridVolumeTransform() => GridVolumeTransform;
        
        /// <summary>
        /// 자식 오브젝트를 자동으로 찾아서 바인딩합니다.
        /// </summary>
        protected virtual void AutoBindChildren()
        {
            // GridVolume 자동 찾기
            if (gridVolumeTransform == null)
            {
                // "GridVolume"이라는 이름의 자식 오브젝트 찾기
                Transform found = transform.Find("GridVolume");
                if (found == null)
                {
                    // 대소문자 구분 없이 찾기
                    foreach (Transform child in transform)
                    {
                        if (child.name.Equals("GridVolume", System.StringComparison.OrdinalIgnoreCase))
                        {
                            found = child;
                            break;
                        }
                    }
                }
                gridVolumeTransform = found;
            }
            
            // Sprite 자동 찾기
            if (spriteTransform == null)
            {
                // 먼저 SpriteRenderer가 있는 자식 오브젝트 찾기
                SpriteRenderer[] childRenderers = GetComponentsInChildren<SpriteRenderer>();
                
                // 자기 자신의 SpriteRenderer는 제외
                SpriteRenderer selfRenderer = GetComponent<SpriteRenderer>();
                
                foreach (var renderer in childRenderers)
                {
                    // 자기 자신이 아니고, 자식 오브젝트인 경우
                    if (renderer != selfRenderer && renderer.transform.IsChildOf(transform))
                    {
                        spriteTransform = renderer.transform;
                        break;
                    }
                }
                
                // SpriteRenderer가 없으면 "Sprite" 또는 "Image"라는 이름의 자식 오브젝트 찾기
                if (spriteTransform == null)
                {
                    Transform found = transform.Find("Sprite");
                    if (found == null)
                    {
                        found = transform.Find("Image");
                    }
                    if (found == null)
                    {
                        // 대소문자 구분 없이 찾기
                        foreach (Transform child in transform)
                        {
                            string childName = child.name.ToLower();
                            if (childName.Contains("sprite") || childName.Contains("image"))
                            {
                                found = child;
                                break;
                            }
                        }
                    }
                    spriteTransform = found;
                }
            }
        }

        protected virtual void Awake()
        {
            // 자식 오브젝트 자동 바인딩
            if (autoBindChildren)
            {
                AutoBindChildren();
            }
        }

        protected virtual void Start()
        {
            // spriteRenderer 초기화 (Initialize에서 설정되지 않은 경우)
            if (spriteRenderer == null)
            {
                spriteRenderer = SpriteTransform.GetComponent<SpriteRenderer>();
            }
            
            // GridSystem.CreateObject에서 Initialize를 호출하므로 여기서는 호출하지 않음
            // 단, GridSystem이 없는 경우에만 기본 초기화 수행
            if (gridManager == null)
            {
                gridManager = GridSystem.Instance;
                if (gridManager != null)
                {
                    // Initialize가 호출되지 않은 경우(씬에 직접 배치된 오브젝트) 처리
                    if (occupiedTiles == null || occupiedTiles.Count == 0)
                    {
                        // 현재 월드 위치를 기본값으로 사용 (UseNearestGridPosition일 때)
                        // 그리드 볼륨의 위치를 기준으로 계산
                        Vector2Int defaultGridPos = gridManager.WorldToGridPosition(GridVolumeTransform.position);
                        if (!gridManager.IsValidGridPosition(defaultGridPos))
                        {
                            defaultGridPos = Vector2Int.zero;
                        }
                        
                        Vector2Int targetGrid = DetermineGridPosition(defaultGridPos);
                        
                        // UseInspectorPosition일 때는 GridPosition이 유효하면 무조건 사용
                        // UseNearestGridPosition일 때는 계산된 위치가 유효하면 사용
                        bool shouldUsePosition = false;
                        if (gridPositionMode == GridPositionInitializationMode.UseInspectorPosition)
                        {
                            // UseInspectorPosition: GridPosition이 유효하면 무조건 사용 (0,0 포함)
                            shouldUsePosition = gridManager.IsValidGridPosition(GridPosition);
                            if (shouldUsePosition)
                            {
                                targetGrid = GridPosition;
                            }
                        }
                        else
                        {
                            // UseNearestGridPosition: 계산된 위치가 유효하면 사용
                            shouldUsePosition = targetGrid != Vector2Int.zero && gridManager.IsValidGridPosition(targetGrid);
                        }
                        
                        if (shouldUsePosition)
                        {
                            gridPosition = targetGrid;
                            
                            // 월드 위치 설정 (gridPositionMode에 따라)
                            Vector3 worldPos = gridManager.GridToWorldPosition(targetGrid);
                            if (useCustomY)
                            {
                                worldPos.y = customY;
                            }
                            transform.position = worldPos;
                            
                            // 타일 점유
                            OccupyTiles(gridPosition);
                            
                            // 그리드 위치가 설정되었으므로 SortingOrder도 업데이트
                            if (applyInitialGridSorting)
                            {
                                var sorting = SpriteTransform.GetComponent<SortingOrderObject>();
                                if (sorting != null)
                                {
                                    // 뒤(y가 작을수록)일수록 더 높은 정렬이 되도록 부호 반전
                                    sorting.SetBaseOrder(-gridPosition.y * gridSortingScale);
                                }
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
                    var sorting = SpriteTransform.GetComponent<SortingOrderObject>();
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
        
        /// <summary>
        /// CH3_LevelData로부터 데이터를 초기화합니다.
        /// </summary>
        public virtual void InitializeFromData(CH3_LevelData data)
        {
            if (data == null) return;
            
            tileSize = data.TileSize;
            gridPositionMode = data.gridPositionMode;
            useCustomY = data.useCustomY;
            customY = data.customY;
            applyInitialGridSorting = data.applyInitialGridSorting;
            gridSortingScale = data.gridSortingScale;
            
            // 건물 ID 저장 (건물 파괴 시 카운트 감소용)
            buildingId = data.id;
        }
        
        /// <summary>
        /// 자식 오브젝트 참조를 설정합니다.
        /// </summary>
        public void SetChildReferences(Transform spriteTransformRef, Transform gridVolumeTransformRef, bool autoBind = false)
        {
            spriteTransform = spriteTransformRef;
            gridVolumeTransform = gridVolumeTransformRef;
            autoBindChildren = autoBind;
        }
        
        //TODO: Vector2Int 없애기
        public virtual void Initialize(Vector2Int gridPos)
        {
            gridManager = GridSystem.Instance;
            minimapManager = FindObjectOfType<MinimapManager>();
            spriteRenderer = SpriteTransform.GetComponent<SpriteRenderer>();
            minimapManager.CreateMinimapIcon(transform);
            
            // 그리드 위치 결정
            Vector2Int targetGrid = DetermineGridPosition(gridPos);
            gridPosition = targetGrid;
            
            // 월드 위치 설정 (useCustomY 고려)
            Vector3 worldPos = gridManager.GridToWorldPosition(targetGrid);
            if (useCustomY)
            {
                worldPos.y = customY;
            }
            
            // 최상단 오브젝트 위치 설정
            transform.position = worldPos;
            
            // 그리드 볼륨이 별도로 설정되어 있으면, 그리드 볼륨의 위치도 조정
            if (gridVolumeTransform != null)
            {
                // 그리드 볼륨은 최상단 오브젝트를 기준으로 상대 위치 유지
                // (이미 자식이므로 자동으로 따라옴)
            }
            
            // 여러 타일 점유
            OccupyTiles(targetGrid);
            
            // 그리드 셀 점유 상태 설정은 하위 클래스에서 처리
            // (Structure 등에서 블록 설정과 함께 처리)

            // 초기 정렬: 그리드 y(앞/뒤) 기준으로 baseOrder 오프셋 적용
            if (applyInitialGridSorting)
            {
                var sorting = SpriteTransform.GetComponent<SortingOrderObject>();
                if (sorting != null)
                {
                    // 뒤(y가 작을수록)일수록 더 높은 정렬이 되도록 부호 반전
                    sorting.SetBaseOrder(-gridPosition.y * gridSortingScale);
                }
            }
        }
        
        /// <summary>
        /// 그리드 위치 초기화 모드에 따라 최종 그리드 위치를 결정합니다.
        /// </summary>
        protected virtual Vector2Int DetermineGridPosition(Vector2Int defaultGridPos)
        {
            switch (gridPositionMode)
            {
                case GridPositionInitializationMode.UseInspectorPosition:
                    // UseInspectorPosition을 선택했다면 무조건 GridPosition 사용
                    // GridPosition이 유효한지 확인하고, 유효하지 않으면 경고 후 defaultGridPos 사용
                    if (gridManager == null)
                        gridManager = GridSystem.Instance;
                    
                    if (gridManager == null)
                    {
                        Debug.LogWarning($"{gameObject.name}: GridSystem을 찾을 수 없습니다. 기본 위치를 사용합니다.");
                        return defaultGridPos;
                    }
                    
                    // GridPosition이 유효한 범위 내에 있는지 확인
                    if (gridManager.IsValidGridPosition(GridPosition))
                    {
                        return GridPosition;
                    }
                    else
                    {
                        // GridPosition이 범위를 벗어났을 때 경고
                        Debug.LogWarning($"{gameObject.name}: 설정한 GridPosition {GridPosition}이 그리드 범위를 벗어났습니다. 기본 위치 {defaultGridPos}를 사용합니다.");
                        return defaultGridPos;
                    }
                    
                case GridPositionInitializationMode.UseNearestGridPosition:
                    // 현재 월드 위치에서 가장 가까운 그리드 좌표 사용
                    if (gridManager == null)
                        gridManager = GridSystem.Instance;
                    
                    if (gridManager != null)
                    {
                        // 그리드 볼륨의 위치를 기준으로 가장 가까운 그리드 좌표 계산
                        Vector2Int nearestGrid = gridManager.WorldToGridPosition(GridVolumeTransform.position);
                        return gridManager.IsValidGridPosition(nearestGrid) ? nearestGrid : defaultGridPos;
                    }
                    return defaultGridPos;
                    
                default:
                    return defaultGridPos;
            }
        }
        
        /// <summary>
        /// 오브젝트 중심을 기준으로 여러 타일을 점유합니다.
        /// 그리드 볼륨의 위치를 기준으로 타일을 점유합니다.
        /// </summary>
        protected virtual void OccupyTiles(Vector2Int centerPos)
        {
            if (gridManager == null) return;
            
            // 기존에 점유한 타일 해제
            ReleaseTiles();
            
            occupiedTiles.Clear();
            
            // 타일 크기가 1x1이면 단일 타일만 점유
            if (tileSize.x == 1 && tileSize.y == 1)
            {
                occupiedTiles.Add(centerPos);
                gridManager.SetCellOccupied(centerPos, true, gameObject);
                return;
            }
            
            // 중심을 기준으로 타일 범위 계산
            // 홀수 크기: 중심에서 (size-1)/2 만큼 확장
            // 짝수 크기: 중심에서 size/2-1 만큼 확장 (중심이 약간 오프셋됨)
            int offsetX = tileSize.x % 2 == 0 ? tileSize.x / 2 - 1 : tileSize.x / 2;
            int offsetY = tileSize.y % 2 == 0 ? tileSize.y / 2 - 1 : tileSize.y / 2;
            
            int startX = centerPos.x - offsetX;
            int startY = centerPos.y - offsetY;
            int endX = centerPos.x + offsetX;
            int endY = centerPos.y + offsetY;
            
            // 짝수 크기인 경우 한쪽 방향으로 1칸 더 확장
            if (tileSize.x % 2 == 0) endX++;
            if (tileSize.y % 2 == 0) endY++;
            
            // 모든 타일 점유
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Vector2Int tilePos = new Vector2Int(x, y);
                    if (gridManager.IsValidGridPosition(tilePos))
                    {
                        occupiedTiles.Add(tilePos);
                        gridManager.SetCellOccupied(tilePos, true, gameObject);
                    }
                }
            }
        }
        
        /// <summary>
        /// 점유한 모든 타일을 해제합니다.
        /// </summary>
        protected virtual void ReleaseTiles()
        {
            if (gridManager == null) return;
            
            foreach (var tilePos in occupiedTiles)
            {
                gridManager.SetCellOccupied(tilePos, false);
            }
            
            occupiedTiles.Clear();
        }
        
        /// <summary>
        /// 점유한 모든 타일을 차단합니다.
        /// </summary>
        protected virtual void BlockTiles(bool blocked)
        {
            if (gridManager == null) return;
            
            foreach (var tilePos in occupiedTiles)
            {
                gridManager.SetCellBlocked(tilePos, blocked);
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

            // 그리드 볼륨의 위치를 기준으로 그리드 위치 계산
            Vector2Int newGridPos = gridManager.WorldToGridPosition(GridVolumeTransform.position);

            if (gridManager.IsValidGridPosition(newGridPos))
            {
                Vector2Int oldGridPos = gridPosition;
                
                // 기존 타일 해제
                ReleaseTiles();

                gridPosition = newGridPos;
                
                // 새로운 위치의 타일 점유
                OccupyTiles(newGridPos);
                
                // 그리드 위치가 변경되었을 때 SortingOrder 업데이트
                if (applyInitialGridSorting && oldGridPos != newGridPos)
                {
                    // SpriteTransform에서 SortingOrderObject 찾기
                    var sorting = SpriteTransform.GetComponent<SortingOrderObject>();
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
            ReleaseTiles();
            minimapManager.RemoveMinimapIcon(transform);
            Destroy(gameObject);
        }

        protected virtual void OnDestroy()
        {
            // 건물이 파괴될 때 BuildingSystem에 알림
            NotifyBuildingDestroyed();
            
            ReleaseTiles();
        }
        
        /// <summary>
        /// 건물이 파괴되었을 때 BuildingSystem에 알립니다.
        /// </summary>
        private void NotifyBuildingDestroyed()
        {
            // 건축 아이템으로 건설된 건물인 경우에만 알림
            if (!string.IsNullOrEmpty(buildingId))
            {
                var buildingSystem = BuildingSystem.Instance;
                if (buildingSystem != null)
                {
                    buildingSystem.BuildingDestroyed(buildingId);
                }
            }
        }
    }
}
