using UnityEngine;

namespace Runtime.CH3.Main
{
    public class BlockedArea : BaseGridObject
    {
        public override void Initialize(Vector2Int gridPos)
        {
            base.Initialize(gridPos);
            GridManager.Instance.SetCellBlocked(gridPos, true);
        }

        public override void Remove()
        {
            GridManager.Instance.SetCellBlocked(gridPosition, false);
            base.Remove();
        }
    }
}