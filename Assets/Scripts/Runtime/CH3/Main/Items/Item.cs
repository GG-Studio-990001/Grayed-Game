using UnityEngine;

namespace Runtime.CH3.Main
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Item")]
    public class Item : ScriptableObject
    {
        public string itemName;
        public Sprite itemIcon;
        
        [Tooltip("건축 아이템인 경우 건축 데이터 참조")]
        public CH3_LevelData buildingData;
        
        /// <summary>
        /// 건축 아이템인지 확인
        /// </summary>
        public bool IsBuildingItem => buildingData != null && buildingData.isBuilding;
    }
}