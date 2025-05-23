﻿using System;
using UnityEngine;

namespace Runtime.CH3.Main
{
    public class BaseGridObject : MonoBehaviour, IGridObject
    {
        [SerializeField] protected GridObjectType objectType;  // Inspector에서 설정할 수 있도록
        [SerializeField] protected Vector2Int gridPosition;
        public GridObjectType ObjectType => objectType;
        public Vector2Int GridPosition => gridPosition;
        public GameObject GameObject => gameObject;  // GameObject 속성 구현

        protected SpriteRenderer spriteRenderer;
        private MinimapManager minimapManager;
        protected GridManager gridManager;

        protected virtual void Start()
        {
            Initialize(Vector2Int.zero);
        }
        
        //TODO: Vector2Int 없애기
        public virtual void Initialize(Vector2Int gridPos)
        {
            gridManager = GridManager.Instance;
            minimapManager = FindObjectOfType<MinimapManager>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            minimapManager.CreateMinimapIcon(transform);
            //transform.position = gridManager.GridToWorldPosition(gridPos);
            UpdateGridPosition();
        }

        public virtual void UpdateGridPosition()
        {
            if (gridManager == null)
                return;

            Vector2Int newGridPos = gridManager.WorldToGridPosition(transform.position);

            if (gridManager.IsValidGridPosition(newGridPos))
            {
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
