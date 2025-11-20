using System.Collections.Generic;
using UnityEngine;

namespace Runtime.CH3.Main
{
    public class PlayerGrid : GridObject
    {
        private PlayerController playerController;

        protected override void Start()
        { 
            base.Start();
            gridManager = GridSystem.Instance;
            playerController = GetComponent<PlayerController>();

            // 스폰 좌표가 설정되어 있으면 그 위치로 이동, 없으면 기존 흐름 유지
            if (gridManager != null && gridManager.HasPlayerSpawn)
            {
                gridPosition = gridManager.PlayerSpawnGrid;
                Vector3 spawnWorld = gridManager.GridToWorldPosition(gridManager.PlayerSpawnGrid);
                transform.position = spawnWorld;
            }
        }

        private void LateUpdate()
        {
            UpdateGridPosition();
        }

        public bool CanMoveTo(Vector3 targetWorldPosition)
        {
            Vector2Int targetGridPos = gridManager.WorldToGridPosition(targetWorldPosition);
            if (!gridManager.IsValidGridPosition(targetGridPos))
                return false;
            
            // 차단된 셀인지 확인
            if (gridManager.IsCellBlocked(targetGridPos))
                return false;
                
            return !gridManager.IsCellOccupiedByImpassableObject(targetGridPos);
        }

        public List<Vector2Int> GetNeighborCells()
        {
            List<Vector2Int> neighbors = new List<Vector2Int>();
            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(1, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, -1)
            };
            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighborPos = gridPosition + dir;
                if (gridManager.IsValidGridPosition(neighborPos))
                    neighbors.Add(neighborPos);
            }
            return neighbors;
        }

        private void OnDrawGizmos()
        {
            if (gridManager != null && Application.isPlaying)
            {
                Gizmos.color = new Color(0, 1, 0, 0.3f);
                Vector3 cellCenter = gridManager.GridToWorldPosition(gridPosition);
                Gizmos.DrawCube(cellCenter, new Vector3(1f, 0.1f, 1f));
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
