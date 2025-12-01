using UnityEngine;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 생산 아이템 데이터 구조체
    /// 건물이 생산할 아이템과 개수를 정의
    /// </summary>
    [System.Serializable]
    public class ItemProductionData
    {
        [Tooltip("생산할 아이템")]
        public Item item;
        
        [Tooltip("생산 개수")]
        public int amount = 1;
        
        public ItemProductionData()
        {
            item = null;
            amount = 1;
        }
        
        public ItemProductionData(Item item, int amount = 1)
        {
            this.item = item;
            this.amount = amount;
        }
    }
}

