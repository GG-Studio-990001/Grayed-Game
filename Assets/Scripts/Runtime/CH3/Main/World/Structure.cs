using UnityEngine;

namespace Runtime.CH3.Main
{
    public class Structure : GridObject
    {
        [SerializeField] protected bool isBlocking = true;
        [SerializeField] private bool initializeToGridPosition = true; // 초기 스폰을 GridPosition에 맞춤

        public override void Initialize(Vector2Int gridPos)
        {
            base.Initialize(gridPos);
            // 초기 배치: 인스펙터의 gridPosition을 우선 적용
            if (initializeToGridPosition && gridManager != null)
            {
                // BaseGridObject가 gridPosition을 유지하고 있으므로 그 좌표로 월드 위치 재배치
                Vector2Int targetGrid = gridPosition == Vector2Int.zero ? gridPos : gridPosition;
                Vector3 world = GetWorldPositionForGrid(targetGrid);
                transform.position = world;
                UpdateGridPosition();
            }

            if (isBlocking && gridManager != null)
            {
                // gridPosition을 직접 사용 (이미 그리드 좌표)
                // 디버그 로그 추가
                Debug.Log($"Structure 블록 설정: {gridPosition}, isBlocking: {isBlocking}");
                GridSystem.Instance.SetCellBlocked(gridPosition, true);
            }
        }

        public override void Remove()
        {
            if (isBlocking && gridManager != null)
            {
                // gridPosition을 직접 사용 (이미 그리드 좌표)
                GridSystem.Instance.SetCellBlocked(gridPosition, false);
            }
            base.Remove();
        }
    }
}