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
        [Header("Scripts")]
        [SerializeField] private ResourceController _resourceController;

        [Header("UI Slots")]
        [SerializeField] private List<ShopItemSlotUI> _slots;
        [SerializeField] private Sprite[] _itemSprs;
        [SerializeField] private Sprite _coinSprite;
        [SerializeField] private Sprite _jewelSprite;

        private List<ShopItem> allItems = new();
        private const int SHOP_ITEM_COUNT = 4;

        private void Start()
        {
            LoadCSV();

            for (int i = 0; i < _slots.Count; i++)
                _slots[i].SetSlotIndex(i);

            RefreshShop();
        }

        private void RefreshShop()
        {
            List<ShopItem> pickedItems = RefreshItems();
            BindUI(pickedItems);
        }

        public void RefreshSingleItem(int slotIndex)
        {
            List<ShopItem> currentItems = new();

            for (int i = 0; i < _slots.Count; i++)
            {
                if (i == slotIndex) continue;
                currentItems.Add(_slots[i].GetCurrentItem());
            }

            ShopItem newItem = PickOneItem(currentItems);
            BindSlot(slotIndex, newItem);
        }

        private ShopItem PickOneItem(List<ShopItem> excludeItems)
        {
            List<ShopItem> candidates = new(allItems);

            candidates.RemoveAll(c =>
                excludeItems.Exists(e => e.id == c.id) ||
                IsFishConflict(excludeItems, c)
            );

            return candidates[Random.Range(0, candidates.Count)];
        }

        private void BindUI(List<ShopItem> items)
        {
            for (int i = 0; i < SHOP_ITEM_COUNT; i++)
                BindSlot(i, items[i]);
        }

        private void BindSlot(int index, ShopItem item)
        {
            Sprite currency = item.costType == CostType.Coin ? _coinSprite : _jewelSprite;

            Sprite icon = item.name switch
            {
                "물고기 젤리" => _itemSprs[0],
                "초코캣" => _itemSprs[1],
                "젤리캣" => _itemSprs[2],
                "멜로캣" => _itemSprs[3],
                "캔디팝" => _itemSprs[4],
                "스틱캔디" => _itemSprs[5],
                _ => null
            };

            _slots[index].Bind(item, icon, currency);
        }

        private List<ShopItem> RefreshItems()
        {
            List<ShopItem> candidates = new(allItems);
            List<ShopItem> result = new();

            while (result.Count < SHOP_ITEM_COUNT)
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
                item.name = row["name"].ToString();

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