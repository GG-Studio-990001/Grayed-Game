using UnityEngine;

namespace Runtime.CH3.Main
{
    public class Structure : GridObject
    {
        [SerializeField] protected bool isBlocking = true;
        
        [Header("Grid Position Initialization")]
        [SerializeField] private GridPositionMode gridPositionMode = GridPositionMode.UseInspectorPosition;
        
        public enum GridPositionMode
        {
            UseInspectorPosition,    // 인스펙터에서 설정한 GridPosition 사용
            UseNearestGridPosition   // 현재 월드 위치에서 가장 가까운 그리드 좌표 사용
        }

        public override void Initialize(Vector2Int gridPos)
        {
            Vector2Int targetGrid;
            
            switch (gridPositionMode)
            {
                case GridPositionMode.UseInspectorPosition:
                    // 인스펙터에서 설정한 GridPosition 사용
                    targetGrid = gridPosition == Vector2Int.zero ? gridPos : gridPosition;
                    break;
                    
                case GridPositionMode.UseNearestGridPosition:
                    // 현재 월드 위치에서 가장 가까운 그리드 좌표 사용
                    if (gridManager == null)
                        gridManager = GridSystem.Instance;
                    
                    if (gridManager != null)
                    {
                        Vector2Int nearestGrid = gridManager.WorldToGridPosition(transform.position);
                        targetGrid = gridManager.IsValidGridPosition(nearestGrid) ? nearestGrid : gridPos;
                    }
                    else
                    {
                        targetGrid = gridPos;
                    }
                    break;
                    
                default:
                    targetGrid = gridPos;
                    break;
            }
            
            // base.Initialize를 올바른 좌표로 호출
            base.Initialize(targetGrid);

            if (isBlocking && gridManager != null)
            {
                // gridPosition을 직접 사용 (이미 그리드 좌표)
                GridSystem.Instance.SetCellBlocked(gridPosition, true);
                
                // 블록하는 Structure는 셀을 점유 상태로도 설정
                GridSystem.Instance.SetCellOccupied(gridPosition, true, gameObject);
            }
        }

        public override void Remove()
        {
            if (isBlocking && gridManager != null)
            {
                // gridPosition을 직접 사용 (이미 그리드 좌표)
                GridSystem.Instance.SetCellBlocked(gridPosition, false);
                
                // 셀 점유 상태도 해제
                GridSystem.Instance.SetCellOccupied(gridPosition, false);
            }
            base.Remove();
        }
    }
}