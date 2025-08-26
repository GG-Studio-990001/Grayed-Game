using UnityEngine;

namespace Runtime.CH3.Main
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Item")]
    public class Item : ScriptableObject
    {
        public string itemName;
        public Sprite itemIcon;
    }
}