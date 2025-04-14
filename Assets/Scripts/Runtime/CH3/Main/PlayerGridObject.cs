using System.Collections.Generic;
using UnityEngine;

namespace Runtime.CH3.Main
{
    public class PlayerGridObject : GridObject
    {
        private QuaterViewPlayer playerController;

        protected override void Start()
        {
            base.Start();
            playerController = GetComponent<QuaterViewPlayer>();

            // 시작 시 중앙 위치로 이동 (필요한 경우)
            if (gridManager != null)
            {
                Vector3 centerPos = gridManager.GetCenterPosition(transform);
                transform.position = centerPos;
                UpdateGridPosition();
            }
        }

        private void LateUpdate()
        {
            // 플레이어가 이동할 때마다 그리드 위치 업데이트
            UpdateGridPosition();
        }

        // 이동 전 위치 유효성 검사
        public bool CanMoveTo(Vector3 targetWorldPosition)
        {
            Vector2Int targetGridPos = gridManager.WorldToGridPosition(targetWorldPosition);
            if (!gridManager.IsValidGridPosition(targetGridPos))
                return false;

            GridCell targetCell = gridManager.GetCell(targetGridPos);
            return targetCell != null && !targetCell.IsOccupied;
        }

        // 현재 그리드 위치 기준으로 이웃한 셀들 확인
        public List<Vector2Int> GetNeighborCells()
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(1, 0), // 오른쪽
                new Vector2Int(-1, 0), // 왼쪽
                new Vector2Int(0, 1), // 위
                new Vector2Int(0, -1), // 아래
            };

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighborPos = currentGridPosition + dir;
                if (gridManager.IsValidGridPosition(neighborPos))
                {
                    neighbors.Add(neighborPos);
                }
            }

            return neighbors;
        }

        // 디버그용 시각화
        private void OnDrawGizmos()
        {
            if (gridManager != null && Application.isPlaying)
            {
                // 현재 위치한 셀 하이라이트
                Gizmos.color = new Color(0, 1, 0, 0.3f);
                Vector3 cellCenter = gridManager.GridToWorldPosition(currentGridPosition);
                Gizmos.DrawCube(cellCenter, new Vector3(1f, 0.1f, 1f));

                // 이동 가능한 이웃 셀들 표시
                Gizmos.color = new Color(0, 0, 1, 0.2f);
                foreach (Vector2Int neighbor in GetNeighborCells())
                {
                    Vector3 neighborPos = gridManager.GridToWorldPosition(neighbor);
                    Gizmos.DrawCube(neighborPos, new Vector3(1f, 0.1f, 1f));
                }
            }
        }
    }
}