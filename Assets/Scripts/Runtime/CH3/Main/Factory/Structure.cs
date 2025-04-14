using UnityEngine;

namespace Runtime.CH3.Main
{
    public class Structure : BaseGridObject
    {
        [SerializeField] private bool isBlocking = true;

        public override void Initialize(Vector2Int gridPos)
        {
            base.Initialize(gridPos);
            if (isBlocking)
            {
                //GridManager.Instance.SetCellBlocked(gridPos, true);
            }
        }

        public override void Remove()
        {
            if (isBlocking)
            {
                //GridManager.Instance.SetCellBlocked(gridPosition, false);
            }
            base.Remove();
        }
    }
}