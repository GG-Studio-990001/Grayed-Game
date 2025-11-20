using UnityEngine;

namespace Runtime.CH3.Main
{
    public class InventoryBootstrap : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Inventory inventory;

        [Header("Initial Items")]
        [SerializeField] private Item wood;
        [SerializeField] private Item stone;
        [SerializeField] private Item coin;
        [SerializeField] private Item translationPack;
        [SerializeField] private Item build;

        private void Start()
        {
            if (inventory == null) inventory = GetComponent<Inventory>();
            if (inventory == null) return;

            // 초기 아이템: 목재, 석재, 코인, 번역팩
            inventory.TryAdd(wood, 1);
            inventory.TryAdd(stone, 1);
            inventory.TryAdd(coin, 1);
            inventory.TryAdd(translationPack, 1);
            inventory.TryAdd(build, 1);
        }
    }
}


