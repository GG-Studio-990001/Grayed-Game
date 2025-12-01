using UnityEngine;

namespace Runtime.CH3.Main
{
    public class InventoryBootstrap : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Inventory inventory;

        [Header("Initial Materials")]
        [SerializeField] private bool addInitialMaterials = true;
        [SerializeField] private int woodAmount = 50;
        [SerializeField] private int stoneAmount = 50;
        [SerializeField] private int coinAmount = 100;
        [SerializeField] private int resourceDefaultAmount = 20;
        [SerializeField] private int resourceAAmount = 10;
        [SerializeField] private int resourceBAmount = 10;
        [SerializeField] private int resourceCAmount = 10;

        private void Start()
        {
            if (inventory == null) inventory = GetComponent<Inventory>();
            if (inventory == null) return;

            // 제작 재료 추가
            if (addInitialMaterials)
            {
                AddInitialMaterials();
            }
        }

        private Item LoadItemById(string itemId)
        {
            if (string.IsNullOrEmpty(itemId)) return null;
            
            var item = Resources.Load<Item>($"Data/CH3/Items/{itemId}_Item");
            if (item == null)
            {
                item = Resources.Load<Item>($"Data/CH3/Items/{itemId}_item");
            }
            
            return item;
        }

        private void AddInitialMaterials()
        {
            if (inventory == null)
            {
                Debug.LogError("[InventoryBootstrap] Inventory is null!");
                return;
            }

            var itemBuild = LoadItemById("build");
            var itemWood = LoadItemById("wood");
            var itemStone = LoadItemById("stone");
            var itemCoin = LoadItemById("coin");
            var itemResourceA = LoadItemById("resourceA");
            var itemResourceB = LoadItemById("resourceB");
            var itemResourceC = LoadItemById("resourceC");
            var itemResourceDefault = LoadItemById("resourceDefault");

            if (itemWood == null) Debug.LogWarning("[InventoryBootstrap] wood_item을 찾을 수 없습니다!");
            if (itemStone == null) Debug.LogWarning("[InventoryBootstrap] stone_item을 찾을 수 없습니다!");
            if (itemCoin == null) Debug.LogWarning("[InventoryBootstrap] coin_item을 찾을 수 없습니다!");
            if (itemResourceA == null) Debug.LogWarning("[InventoryBootstrap] resourceA_item을 찾을 수 없습니다!");
            if (itemResourceB == null) Debug.LogWarning("[InventoryBootstrap] resourceB_item을 찾을 수 없습니다!");
            if (itemResourceC == null) Debug.LogWarning("[InventoryBootstrap] resourceC_item을 찾을 수 없습니다!");
            if (itemResourceDefault == null) Debug.LogWarning("[InventoryBootstrap] resourceDefault_item을 찾을 수 없습니다!");
            if (itemBuild == null) Debug.LogWarning("[InventoryBootstrap] build_Item을 찾을 수 없습니다!");

            if (itemWood != null)
            {
                bool success = inventory.TryAdd(itemWood, woodAmount);
                if (!success) Debug.LogWarning($"[InventoryBootstrap] wood_item 추가 실패 (요청: {woodAmount})");
            }

            if (itemStone != null)
            {
                bool success = inventory.TryAdd(itemStone, stoneAmount);
                if (!success) Debug.LogWarning($"[InventoryBootstrap] stone_item 추가 실패 (요청: {stoneAmount})");
            }

            if (itemCoin != null)
            {
                bool success = inventory.TryAdd(itemCoin, coinAmount);
                if (!success) Debug.LogWarning($"[InventoryBootstrap] coin_item 추가 실패 (요청: {coinAmount})");
            }

            if (itemResourceDefault != null)
            {
                bool success = inventory.TryAdd(itemResourceDefault, resourceDefaultAmount);
                if (!success) Debug.LogWarning($"[InventoryBootstrap] resourceDefault_item 추가 실패 (요청: {resourceDefaultAmount})");
            }

            if (itemResourceA != null)
            {
                bool success = inventory.TryAdd(itemResourceA, resourceAAmount);
                if (!success) Debug.LogWarning($"[InventoryBootstrap] resourceA_item 추가 실패 (요청: {resourceAAmount})");
            }

            if (itemResourceB != null)
            {
                bool success = inventory.TryAdd(itemResourceB, resourceBAmount);
                if (!success) Debug.LogWarning($"[InventoryBootstrap] resourceB_item 추가 실패 (요청: {resourceBAmount})");
            }

            if (itemResourceC != null)
            {
                bool success = inventory.TryAdd(itemResourceC, resourceCAmount);
                if (!success) Debug.LogWarning($"[InventoryBootstrap] resourceC_item 추가 실패 (요청: {resourceCAmount})");
            }

            if (itemBuild != null)
            {
                bool success = inventory.TryAdd(itemBuild, 1);
                if (!success) Debug.LogWarning("[InventoryBootstrap] build_Item 추가 실패");
            }
        }
    }
}


