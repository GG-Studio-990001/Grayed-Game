using UnityEngine;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 광물 오브젝트 - 채굴 가능한 광물
    /// </summary>
    public class Ore : MineableObject
    {
        protected override void OnMiningComplete()
        {
            if (GridSystem.Instance != null)
            {
                GridSystem.Instance.OnMineralRemoved(ObjectType);
            }
        }
    }
}