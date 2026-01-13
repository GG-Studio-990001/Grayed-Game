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

        private bool _isFirstRefresh = true; // 최초 1회 여부 체크

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

            _isFirstRefresh = false; // 첫 갱신 후 false 처리
        }

        // 버튼에 연결
        public void RefreshShopWithCost()
        {
            if (_resourceController.UseCoin(3)) // 주의: 값 바꿀 때 UI 인스펙터와 ButtonInteractableController 모두 수정 필요
            {
                RefreshShop();
            }
        }

        public void RefreshSingleItem(int slotIndex)
        {
            List<ShopItem> currentItems = new();

            for (int i = 0; i < _slots.Count; i++)
            {
                if (i == slotIndex) continue;

                ShopItem item = _slots[i].GetCurrentItem();
                currentItems.Add(item);
            }

            ShopItem newItem;

            // 튜토리얼 물고기를 구매했으면 항상 Fish_01 고정
            if (_slots[slotIndex].GetCurrentItem().id == "Fish_Tutorial")
            {
                newItem = allItems.Find(i => i.id == "Fish_01");
            }
            else
            {
                newItem = PickOneItem(currentItems);
            }

            BindSlot(slotIndex, newItem);
        }

        private ShopItem PickOneItem(List<ShopItem> excludeItems)
        {
            List<ShopItem> candidates = new(allItems);

            candidates.RemoveAll(c =>
                excludeItems.Exists(e => e.id == c.id) ||
                IsFishConflict(excludeItems, c) ||
                c.id == "Fish_Tutorial" // 튜토리얼은 최초 한 번만
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
            List<ShopItem> result = new();

            for (int i = 0; i < SHOP_ITEM_COUNT; i++)
            {
                if (_isFirstRefresh && i == 0)
                {
                    // 최초 1회 첫 슬롯은 튜토리얼 고정
                    result.Add(allItems.Find(x => x.id == "Fish_Tutorial"));
                    continue;
                }

                List<ShopItem> candidates = new(allItems);
                candidates.RemoveAll(c => result.Exists(r => r.id == c.id) || IsFishConflict(result, c) || c.id == "Fish_Tutorial");

                int index = Random.Range(0, candidates.Count);
                result.Add(candidates[index]);
            }

            return result;
        }

        private bool IsFishConflict(List<ShopItem> current, ShopItem picked)
        {
            // Fish_Tutorial, Fish_01, Fish_02 모두 상호 충돌
            if (picked.id == "Fish_Tutorial" || picked.id == "Fish_01" || picked.id == "Fish_02")
            {
                return current.Exists(i => i.id == "Fish_Tutorial" || i.id == "Fish_01" || i.id == "Fish_02");
            }

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