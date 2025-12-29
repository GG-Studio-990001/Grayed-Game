using System.Collections.Generic;
using UnityEngine;
using Runtime.ETC;

namespace CH4.CH1
{
    public enum CostType
    {
        Coin,
        Jewel
    }

    public class ShopItem
    {
        public string id;
        public string name;
        public CostType costType;
        public int cost;
    }

    public class ShopRandomPicker : MonoBehaviour
    {
        [SerializeField]  ResourceController resourceController;
        [Header("UI Slots")]
        [SerializeField] private List<ShopItemSlotUI> slots; // 4개
        [SerializeField] private Sprite[] itemSprs;
        [SerializeField] private Sprite coinSprite;
        [SerializeField] private Sprite jewelSprite;
        //private Dictionary<string, Sprite> iconMap = new Dictionary<string, Sprite>();

        private List<ShopItem> allItems = new();
        private const int SHOP_ITEM_COUNT = 4;

        private void Start()
        {
            LoadCSV();
            RefreshShop();
        }

        public void RefreshShopWithCost()
        {
            if (resourceController.UseCoin(10))
                RefreshShop();
        }

        private void RefreshShop()
        {
            List<ShopItem> pickedItems = RefreshItems();
            BindUI(pickedItems);
            // LogPickedItems(pickedItems);
        }

        private void BindUI(List<ShopItem> items)
        {
            for (int i = 0; i < SHOP_ITEM_COUNT; i++)
            {
                ShopItem item = items[i];
                Sprite currency = item.costType == CostType.Coin ? coinSprite : jewelSprite;

                Sprite icon = item.name switch
                {
                    "물고기 젤리" => itemSprs[0],
                    "초코캣" => itemSprs[1],
                    "젤리캣" => itemSprs[2],
                    "멜로캣" => itemSprs[3],
                    "캔디팝" => itemSprs[4],
                    "스틱캔디" => itemSprs[5],
                    _ => null
                };

                slots[i].Bind(item, icon, currency);
            }
        }

        private void LogPickedItems(List<ShopItem> items)
        {
            Debug.Log("=== Shop Items ===");
            foreach (var item in items)
            {
                Debug.Log($"{item.id} | {item.costType} : {item.cost}");
            }
        }

        private List<ShopItem> RefreshItems()
        {
            List<ShopItem> candidates = new(allItems);
            List<ShopItem> result = new();

            while (result.Count < SHOP_ITEM_COUNT && candidates.Count > 0)
            {
                int index = Random.Range(0, candidates.Count);
                ShopItem picked = candidates[index];

                if (IsFishConflict(result, picked))
                {
                    candidates.RemoveAt(index);
                    continue;
                }

                result.Add(picked);
                candidates.RemoveAt(index);
            }

            return result;
        }

        private bool IsFishConflict(List<ShopItem> current, ShopItem picked)
        {
            if (picked.id == "Fish_01")
                return current.Exists(i => i.id == "Fish_02");

            if (picked.id == "Fish_02")
                return current.Exists(i => i.id == "Fish_01");

            return false;
        }

        private void LoadCSV()
        {
            var data = CSVReader.Read("JellyStoreData");

            allItems.Clear();

            foreach (var row in data)
            {
                ShopItem item = new();
                item.id = row["id"].ToString();
                item.name = row["name"].ToString(); // 추가

                if (row.ContainsKey("jewel") && row["jewel"].ToString() != "")
                {
                    item.costType = CostType.Jewel;
                    item.cost = (int)row["jewel"];
                }
                else
                {
                    item.costType = CostType.Coin;
                    item.cost = (int)row["coin"];
                }

                allItems.Add(item);
            }
        }

    }
}