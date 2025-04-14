using UnityEngine;

namespace Runtime.CH3.Main
{
    public class GridObject : MonoBehaviour
    {
        protected GridManager gridManager;
        protected Vector2Int currentGridPosition;
        protected SpriteRenderer spriteRenderer;
        protected MinimapManager minimapManager;

        protected virtual void Start()
        {
            gridManager = FindObjectOfType<GridManager>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            minimapManager = FindObjectOfType<MinimapManager>();

            if (gridManager != null)
            {
                UpdateGridPosition();
            }
            
            if (minimapManager != null)
            {
                // 그리드 오브젝트용 아이콘 생성
                minimapManager.CreateMinimapIcon(transform);
            }
        }

        protected virtual void UpdateGridPosition()
        {
            Vector2Int newGridPos = gridManager.WorldToGridPosition(transform.position);

            if (gridManager.IsValidGridPosition(newGridPos))
            {
                // 이전 위치 정리
                if (gridManager.GetCell(currentGridPosition) != null)
                {
                    gridManager.GetCell(currentGridPosition).IsOccupied = false;
                    gridManager.GetCell(currentGridPosition).OccupyingObject = null;
                }

                // 새 위치 설정
                currentGridPosition = newGridPos;
                GridCell cell = gridManager.GetCell(currentGridPosition);
                if (cell != null)
                {
                    cell.IsOccupied = true;
                    cell.OccupyingObject = gameObject;
                }
            }
        }

        public Vector2Int GetCurrentGridPosition()
        {
            return currentGridPosition;
        }

        protected virtual void OnDestroy()
        {
            if (gridManager != null && gridManager.GetCell(currentGridPosition) != null)
            {
                gridManager.GetCell(currentGridPosition).IsOccupied = false;
                gridManager.GetCell(currentGridPosition).OccupyingObject = null;
            }
        }
    }
}