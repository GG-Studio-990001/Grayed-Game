using UnityEngine;

namespace Runtime.CH3.Main
{
    public class Structure : BaseGridObject
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
                Vector3 world = gridManager.GridToWorldPosition(gridPosition == Vector2Int.zero ? gridPos : gridPosition);
                transform.position = world;
                UpdateGridPosition();
            }

            if (isBlocking && gridManager != null)
            {
                var indexPos = gridManager.ToArrayIndex(gridPosition);
                GridManager.Instance.SetCellBlocked(indexPos, true);
            }
        }

        public override void Remove()
        {
            if (isBlocking && gridManager != null)
            {
                var indexPos = gridManager.ToArrayIndex(gridPosition);
                GridManager.Instance.SetCellBlocked(indexPos, false);
            }
            base.Remove();
        }
    }
}