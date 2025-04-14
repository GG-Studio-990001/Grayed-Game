using UnityEngine;

namespace Runtime.CH3.Main
{
    public interface IGridObject
    {
        Vector2Int GridPosition { get; }
        void Initialize(Vector2Int gridPos);
        void Remove();
    }
}
