using UnityEngine;

namespace Runtime.CH3.Main
{
    public interface IGridObject
    {
        Vector2Int GridPosition { get; }
        GameObject GameObject { get; }  // GameObject 속성 추가
        void Initialize(Vector2Int gridPos);
        void Remove();
    }
}
