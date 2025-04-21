using System.Collections.Generic;
using UnityEngine;

namespace Runtime.CH3.Main
{
    public class GridCell
    {
        public Vector2Int GridPosition { get; private set; }
        public Vector3 WorldPosition { get; private set; }
        public bool IsOccupied { get; set; }
        public GameObject OccupyingObject { get; set; }

        public GridCell(Vector2Int gridPos, Vector3 worldPos)
        {
            GridPosition = gridPos;
            WorldPosition = worldPos;
            IsOccupied = false;
            OccupyingObject = null;
        }
    }

    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance;

        [Header("Grid Settings")] [SerializeField]
        private Vector2Int gridSize = new Vector2Int(10, 10);

        [SerializeField] private float cellWidth = 1f;
        [SerializeField] private float heightOffset = 0.3f;

        [Header("Camera Reference")] [SerializeField]
        private Camera mainCamera;

        //[Header("Debug Visualization")] [SerializeField]
        //private bool showDebugVisuals = true;

        [SerializeField] private Color gridLineColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        private Dictionary<Vector2Int, GridCell> _gridCells = new Dictionary<Vector2Int, GridCell>();
        private Vector3 _forward;
        private Vector3 _right;
        private Vector2Int _gridCenter;
        
        private bool[,] blockedCells;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            if (mainCamera == null)
                mainCamera = Camera.main;

            // 그리드 중심점 계산
            _gridCenter = new Vector2Int(gridSize.x / 2, gridSize.y / 2);
            blockedCells = new bool[gridSize.x, gridSize.y];
            InitializeCameraAlignment();
            InitializeGrid();
        }

        private void InitializeCameraAlignment()
        {
            _forward = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;
            _right = Vector3.ProjectOnPlane(mainCamera.transform.right, Vector3.up).normalized;
        }

        private void InitializeGrid()
        {
            // 그리드 중심을 (0,0)으로 하기 위해 오프셋 계산
            int startX = -_gridCenter.x;
            int startY = -_gridCenter.y;
            int endX = gridSize.x - _gridCenter.x;
            int endY = gridSize.y - _gridCenter.y;

            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    Vector2Int gridPosition = new Vector2Int(x, y);
                    Vector3 worldPosition = GridToWorldPosition(gridPosition);
                    _gridCells[gridPosition] = new GridCell(gridPosition, worldPosition);
                }
            }
        }

        public Vector3 GridToWorldPosition(Vector2Int gridPos)
        {
            Vector3 basePos = new Vector3(
                gridPos.x * cellWidth,
                heightOffset,
                gridPos.y * cellWidth
            );

            return transform.position +
                   _right * basePos.x +
                   Vector3.up * basePos.y +
                   _forward * basePos.z;
        }

        public Vector2Int WorldToGridPosition(Vector3 worldPos)
        {
            Vector3 relativePos = worldPos - transform.position;

            float x = Vector3.Dot(relativePos, _right) / cellWidth;
            float z = Vector3.Dot(relativePos, _forward) / cellWidth;

            return new Vector2Int(
                Mathf.RoundToInt(x),
                Mathf.RoundToInt(z)
            );
        }

        public bool IsValidGridPosition(Vector2Int gridPos)
        {
            return gridPos.x >= -_gridCenter.x && gridPos.x < gridSize.x - _gridCenter.x &&
                   gridPos.y >= -_gridCenter.y && gridPos.y < gridSize.y - _gridCenter.y;
        }

        public GridCell GetCell(Vector2Int gridPos)
        {
            if (_gridCells.TryGetValue(gridPos, out GridCell cell))
            {
                return cell;
            }

            return null;
        }

        /*
        private void OnDrawGizmos()
        {
            if (!showDebugVisuals) return;

            // 에디터에서도 카메라 방향 초기화가 필요함
            if (mainCamera == null)
                mainCamera = Camera.main;

            if (_forward == Vector3.zero || _right == Vector3.zero)
            {
                _forward = Vector3.ProjectOnPlane(mainCamera != null ? mainCamera.transform.forward : Vector3.forward,
                    Vector3.up).normalized;
                _right = Vector3
                    .ProjectOnPlane(mainCamera != null ? mainCamera.transform.right : Vector3.right, Vector3.up)
                    .normalized;
            }

            // 그리드 중심점 계산
            _gridCenter = new Vector2Int(gridSize.x / 2, gridSize.y / 2);

            Gizmos.color = gridLineColor;

            // 그리드 라인 그리기
            int startX = -_gridCenter.x;
            int startY = -_gridCenter.y;
            int endX = gridSize.x - _gridCenter.x;
            int endY = gridSize.y - _gridCenter.y;

            for (int x = startX; x <= endX; x++)
            {
                Vector3 startPos = GridToWorldPosition(new Vector2Int(x, startY));
                Vector3 endPos = GridToWorldPosition(new Vector2Int(x, endY));
                Gizmos.DrawLine(startPos, endPos);
            }

            for (int y = startY; y <= endY; y++)
            {
                Vector3 startPos = GridToWorldPosition(new Vector2Int(startX, y));
                Vector3 endPos = GridToWorldPosition(new Vector2Int(endX, y));
                Gizmos.DrawLine(startPos, endPos);
            }

            // 각 셀의 중심점과 좌표 표시
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    Vector2Int gridPosition = new Vector2Int(x, y);
                    Vector3 worldPosition = GridToWorldPosition(gridPosition);

                    // (0,0) 좌표는 다른 색상으로 표시
                    if (gridPosition == Vector2Int.zero)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireSphere(worldPosition, 0.15f);
                    }
                    else
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawWireSphere(worldPosition, 0.1f);
                    }

#if UNITY_EDITOR
                    UnityEditor.Handles.Label(worldPosition,
                        $"({gridPosition.x}, {gridPosition.y})");
#endif
                }
            }

            // 그리드 방향 표시
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, _forward * 2f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, _right * 2f);
        }
        

        // OnDrawGizmosSelected에서도 동일한 시각화를 제공하여 선택 시 더 잘 보이게 함
        private void OnDrawGizmosSelected()
        {
            if (!showDebugVisuals) return;
            OnDrawGizmos();
        }
*/
        public void RecalculateGridAlignment()
        {
            InitializeCameraAlignment();
            InitializeGrid();
        }

        // 중심 좌표 (0,0) 얻기 - y값을 유지하도록 수정
        public Vector3 GetCenterPosition(float customHeight)
        {
            Vector3 centerPos = GridToWorldPosition(Vector2Int.zero);
            centerPos.y = customHeight; // 원하는 높이 값 사용
            return centerPos;
        }

        // 오버로드 - 현재 오브젝트의 높이를 유지하고 싶을 때
        public Vector3 GetCenterPosition(Transform targetTransform)
        {
            Vector3 centerPos = GridToWorldPosition(Vector2Int.zero);
            centerPos.y = targetTransform.position.y; // 현재 오브젝트의 높이 유지
            return centerPos;
        }
        
        // 그리드 범위 내에 있는지 확인
        public bool IsWithinGridBounds(Vector2Int position)
        {
            return position.x >= 0 && position.x < gridSize.x &&
                   position.y >= 0 && position.y < gridSize.y;
        }

        // 셀이 블록되어 있는지 확인
        public bool IsCellBlocked(Vector2Int position)
        {
            if (!IsWithinGridBounds(position)) return true;
            return blockedCells[position.x, position.y];
        }

        // 셀 블록 상태 설정
        public void SetCellBlocked(Vector2Int position, bool blocked)
        {
            if (!IsWithinGridBounds(position)) return;
            blockedCells[position.x, position.y] = blocked;
        }
    }
}