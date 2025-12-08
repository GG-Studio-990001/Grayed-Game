using System.Collections.Generic;
using UnityEngine;
using Runtime.ETC;
using System.Linq;

namespace Runtime.CH3.Main
{
    public class ShopCreatePanel : MonoBehaviour
    {
        private Transform contentParent;
        [SerializeField] private GameObject itemPrefab;

        private readonly List<ShopCreateItemText> itemViews = new List<ShopCreateItemText>();
        private ShopCreateItemText selectedItem;
        private Dictionary<string, CH3_LevelData> levelDataCache;
        private List<string> itemIdOrder = new List<string>();
        private Dictionary<ECurrencyData, Item> _currencyToItemMapCache;

        private void Awake()
        {
            InitializeReferences();
            PreloadCurrencyToItemMap();
        }
        
        private void OnEnable()
        {
            // UI가 활성화될 때 참조 초기화 및 아이템 로드
            InitializeReferences();
            if (contentParent != null && (levelDataCache == null || levelDataCache.Count == 0))
            {
                LoadItemsFromCSV();
            }
        }
        
        private void PreloadCurrencyToItemMap()
        {
            _currencyToItemMapCache = GetCurrencyToItemMap();
        }

        private void InitializeReferences()
        {
            var creatList = CH3Utils.FindChildByNameIgnoreCase(transform, "CreatList");
            var scrollView = CH3Utils.FindChildByNameIgnoreCase(creatList, "Scroll View");
            var viewport = CH3Utils.FindChildByNameIgnoreCase(scrollView, "Viewport");
            contentParent = CH3Utils.FindChildByNameIgnoreCase(viewport, "Content");

            var scrollViewImage = scrollView?.GetComponent<UnityEngine.UI.Image>();
            if (scrollViewImage != null && scrollViewImage.raycastTarget)
            {
                scrollViewImage.raycastTarget = false;
            }

            var viewportImage = viewport?.GetComponent<UnityEngine.UI.Image>();
            if (viewportImage != null && viewportImage.raycastTarget)
            {
                viewportImage.raycastTarget = false;
            }

            if (itemPrefab == null && contentParent != null)
            {
                var firstItem = CH3Utils.FindChildByNameIgnoreCase(contentParent, "CreateltemText");
                if (firstItem != null)
                {
                    itemPrefab = firstItem.gameObject;
                }
            }
        }

        private void FindExistingItems()
        {
            if (contentParent == null) return;

            for (int i = 0; i < contentParent.childCount; i++)
            {
                var child = contentParent.GetChild(i);
                if (child != null && child.name.StartsWith("CreateltemText"))
                {
                    var itemText = Utils.GetOrAddComponent<ShopCreateItemText>(child.gameObject);
                    SetupItemClickHandler(itemText);
                    itemViews.Add(itemText);
                }
            }
        }

        private void SetupItemClickHandler(ShopCreateItemText itemText)
        {
            if (itemText?.Button == null) return;
            
            itemText.Button.onClick.RemoveAllListeners();
            itemText.Button.onClick.AddListener(() => OnItemClicked(itemText));
        }

        private void OnItemClicked(ShopCreateItemText clickedItem)
        {
            if (clickedItem == null) return;

            if (selectedItem != null)
            {
                selectedItem.SetSelected(false);
            }

            selectedItem = clickedItem;
            selectedItem.SetSelected(true);

            UpdateSelectionPanel(clickedItem.LevelData);
        }

        private void UpdateSelectionPanel(CH3_LevelData levelData)
        {
            var shopController = GetComponentInParent<ShopUIController>();
            if (shopController == null || shopController.SelectionPanel == null) return;

            var selectionPanel = shopController.SelectionPanel;
            
            if (!selectionPanel.gameObject.activeInHierarchy)
            {
                return;
            }

            if (levelData != null)
            {
                selectionPanel.SetItemTitle(levelData.dev);
                
                var itemSO = LoadItemById(levelData.id);
                if (itemSO != null && itemSO.itemIcon != null)
                {
                    selectionPanel.SetSelectedItem(itemSO.itemIcon);
                }
                else
                {
                    selectionPanel.SetSelectedItem(levelData.sprite);
                }

                if (levelData.buildCurrency != null && levelData.buildCurrency.Count > 0)
                {
                    if (_currencyToItemMapCache == null)
                    {
                        _currencyToItemMapCache = GetCurrencyToItemMap();
                    }
                    
                    for (int i = 0; i < 4; i++)
                    {
                        if (i < levelData.buildCurrency.Count)
                        {
                            var currency = levelData.buildCurrency[i];
                            string currencyName = GetCurrencyDisplayName(currency.currency);
                            selectionPanel.SetBottomItemCount(i, $"{currencyName} x{currency.amount}");
                            
                            Sprite iconToSet = null;
                            if (_currencyToItemMapCache != null && _currencyToItemMapCache.TryGetValue(currency.currency, out Item item))
                            {
                                if (item != null && item.itemIcon != null)
                                {
                                    iconToSet = item.itemIcon;
                                }
                            }
                            
                            selectionPanel.SetBottomItemImage(i, iconToSet);
                        }
                        else
                        {
                            selectionPanel.SetBottomItemCount(i, "");
                            selectionPanel.SetBottomItemImage(i, null);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        selectionPanel.SetBottomItemCount(i, "");
                        selectionPanel.SetBottomItemImage(i, null);
                    }
                }
            }
            else
            {
                selectionPanel.SetItemTitle("");
                selectionPanel.SetSelectedItem(null);
                for (int i = 0; i < 4; i++)
                {
                    selectionPanel.SetBottomItemCount(i, "");
                }
            }

            UpdateCreateButtonState();
        }

        private string GetCurrencyDisplayName(ECurrencyData currency)
        {
            switch (currency)
            {
                case ECurrencyData.Tree: return "목재";
                case ECurrencyData.Stone: return "석재";
                case ECurrencyData.ResourceDefault: return "일반 리소스";
                case ECurrencyData.ResourceA: return "리소스 A";
                case ECurrencyData.ResourceB: return "리소스 B";
                case ECurrencyData.ResourceC: return "리소스 C";
                case ECurrencyData.Coin: return "코인";
                default: return currency.ToString();
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            // UI가 활성화된 후 참조 초기화 및 아이템 로드
            InitializeReferences();
            if (contentParent != null && (levelDataCache == null || levelDataCache.Count == 0))
            {
                LoadItemsFromCSV();
            }
            RefreshSoldOutStates();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ClearItems()
        {
            foreach (var item in itemViews)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
            itemViews.Clear();
            selectedItem = null;
        }

        public void DeselectAll()
        {
            if (selectedItem != null)
            {
                selectedItem.SetSelected(false);
            }
            selectedItem = null;
        }

        public void SelectItem(int index)
        {
            if (index >= 0 && index < itemViews.Count && itemViews[index] != null)
            {
                OnItemClicked(itemViews[index]);
            }
        }

        public int GetSelectedItemIndex()
        {
            if (selectedItem == null) return -1;
            return itemViews.IndexOf(selectedItem);
        }

        public void AddItem(CH3_LevelData levelData, bool soldOut = false)
        {
            if (itemPrefab == null || contentParent == null || levelData == null) return;

            var itemView = Instantiate(itemPrefab, contentParent);
            itemView.name = $"CreateltemText_{itemViews.Count}";
            
            var itemText = Utils.GetOrAddComponent<ShopCreateItemText>(itemView);
            SetupItemClickHandler(itemText);
            itemText.SetLevelData(levelData);
            itemText.SetItemName(levelData.dev);
            itemText.SetSoldOut(soldOut);
            itemText.SetSelected(false);
            
            itemViews.Add(itemText);
        }

        public void SetItemSoldOut(int index, bool soldOut)
        {
            if (index >= 0 && index < itemViews.Count && itemViews[index] != null)
            {
                itemViews[index].SetSoldOut(soldOut);
            }
        }

        public CH3_LevelData GetSelectedItemData()
        {
            if (selectedItem == null) return null;
            int index = GetSelectedItemIndex();
            if (index < 0 || levelDataCache == null || index >= itemIdOrder.Count) return null;

            var itemId = itemIdOrder[index];
            return levelDataCache.TryGetValue(itemId, out var data) ? data : null;
        }

        private void LoadItemsFromCSV()
        {
            levelDataCache = CH3_LevelDataCSVLoader.LoadAllFromCSV();
            ClearItems();
            itemIdOrder.Clear();

            var buildableItems = CH3_LevelDataCSVLoader.GetBuildableItems();
            foreach (var itemData in buildableItems)
            {
                itemIdOrder.Add(itemData.id);
                bool soldOut = CheckItemSoldOut(itemData.id);
                AddItem(itemData, soldOut);
            }
        }

        private bool CheckItemSoldOut(string itemId)
        {
            var inventory = FindObjectOfType<Inventory>();
            if (inventory == null) return false;

            var itemSO = LoadItemById(itemId);
            if (itemSO == null) return false;

            for (int i = 0; i < inventory.Slots.Count; i++)
            {
                var slot = inventory.GetSlot(i);
                if (slot != null && slot.item == itemSO && slot.count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public void RefreshSoldOutStates()
        {
            var inventory = FindObjectOfType<Inventory>();
            if (inventory == null) return;

            for (int i = 0; i < itemViews.Count && i < itemIdOrder.Count; i++)
            {
                var itemId = itemIdOrder[i];
                var itemView = itemViews[i];
                if (itemView != null)
                {
                    bool soldOut = CheckItemSoldOut(itemId);
                    itemView.SetSoldOut(soldOut);
                }
            }

            UpdateCreateButtonState();
        }

        public void TryCraftSelectedItem()
        {
            if (selectedItem == null || selectedItem.LevelData == null) return;

            var levelData = selectedItem.LevelData;
            var inventory = FindObjectOfType<Inventory>();

            if (inventory == null)
            {
                Debug.LogWarning("인벤토리를 찾을 수 없습니다.");
                return;
            }

            if (CheckItemSoldOut(levelData.id))
            {
                Debug.LogWarning("이미 제작된 아이템이 인벤토리에 있습니다.");
                return;
            }

            if (levelData.buildCurrency == null || levelData.buildCurrency.Count == 0)
            {
                Debug.LogWarning("제작 재료가 설정되지 않았습니다.");
                return;
            }

            if (!HasRequiredMaterials(levelData.buildCurrency))
            {
                Debug.LogWarning("재료가 부족합니다.");
                return;
            }

            if (!ConsumeMaterials(levelData.buildCurrency))
            {
                Debug.LogWarning("재료 소모에 실패했습니다.");
                return;
            }

            var itemSO = LoadItemById(levelData.id);
            if (itemSO == null)
            {
                Debug.LogWarning($"Item ScriptableObject를 찾을 수 없습니다: {levelData.id}_Item");
                return;
            }

            if (!inventory.TryAdd(itemSO, 1))
            {
                Debug.LogWarning("인벤토리가 가득 찼습니다.");
                return;
            }

            RefreshSoldOutStates();
        }

        private bool ConsumeMaterials(List<CurrencyData> currencies)
        {
            if (currencies == null || currencies.Count == 0) return true;

            var inventory = FindObjectOfType<Inventory>();
            if (inventory == null) return false;

            var currencyToItemMap = GetCurrencyToItemMap();

            foreach (var currency in currencies)
            {
                if (!currencyToItemMap.TryGetValue(currency.currency, out Item requiredItem))
                {
                    return false;
                }

                int remaining = currency.amount;
                for (int i = 0; i < inventory.Slots.Count && remaining > 0; i++)
                {
                    var slot = inventory.GetSlot(i);
                    if (slot != null && slot.item == requiredItem && slot.count > 0)
                    {
                        int consume = Mathf.Min(remaining, slot.count);
                        if (inventory.TryConsumeAt(i, consume))
                        {
                            remaining -= consume;
                        }
                    }
                }

                if (remaining > 0)
                {
                    return false;
                }
            }

            return true;
        }

        private void UpdateCreateButtonState()
        {
            var shopController = GetComponentInParent<ShopUIController>();
            if (shopController == null || shopController.SelectionPanel == null) return;

            var selectionPanel = shopController.SelectionPanel;
            bool canCraft = false;

            if (selectedItem != null && selectedItem.LevelData != null)
            {
                var levelData = selectedItem.LevelData;
                bool soldOut = CheckItemSoldOut(levelData.id);
                
                if (!soldOut && levelData.buildCurrency != null && levelData.buildCurrency.Count > 0)
                {
                    canCraft = HasRequiredMaterials(levelData.buildCurrency);
                }
            }

            if (selectionPanel.createButton != null)
            {
                selectionPanel.createButton.interactable = canCraft;
            }
        }

        private bool HasRequiredMaterials(List<CurrencyData> currencies)
        {
            if (currencies == null || currencies.Count == 0) return true;

            var inventory = FindObjectOfType<Inventory>();
            if (inventory == null) return false;

            var currencyToItemMap = GetCurrencyToItemMap();

            foreach (var currency in currencies)
            {
                if (!currencyToItemMap.TryGetValue(currency.currency, out Item requiredItem))
                {
                    return false;
                }

                int totalAmount = 0;
                for (int i = 0; i < inventory.Slots.Count; i++)
                {
                    var slot = inventory.GetSlot(i);
                    if (slot != null && slot.item == requiredItem)
                    {
                        totalAmount += slot.count;
                    }
                }

                if (totalAmount < currency.amount)
                {
                    return false;
                }
            }

            return true;
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

        private Dictionary<ECurrencyData, Item> GetCurrencyToItemMap()
        {
            var map = new Dictionary<ECurrencyData, Item>();

            var itemWood = LoadItemById("wood");
            var itemStone = LoadItemById("stone");
            var itemCoin = LoadItemById("coin");
            var itemResourceA = LoadItemById("resourceA");
            var itemResourceB = LoadItemById("resourceB");
            var itemResourceC = LoadItemById("resourceC");
            var itemResourceDefault = LoadItemById("resourceDefault");

            if (itemWood != null)
            {
                map[ECurrencyData.Tree] = itemWood;
            }
            else
            {
                Debug.LogWarning("ShopCreatePanel: wood_item을 찾을 수 없습니다!");
            }
            
            if (itemStone != null)
            {
                map[ECurrencyData.Stone] = itemStone;
            }
            else
            {
                Debug.LogWarning("ShopCreatePanel: stone_item을 찾을 수 없습니다!");
            }
            
            if (itemCoin != null)
            {
                map[ECurrencyData.Coin] = itemCoin;
            }
            else
            {
                Debug.LogWarning("ShopCreatePanel: coin_item을 찾을 수 없습니다!");
            }
            
            if (itemResourceA != null)
            {
                map[ECurrencyData.ResourceA] = itemResourceA;
            }
            else
            {
                Debug.LogWarning("ShopCreatePanel: resourceA_item을 찾을 수 없습니다!");
            }
            
            if (itemResourceB != null)
            {
                map[ECurrencyData.ResourceB] = itemResourceB;
            }
            else
            {
                Debug.LogWarning("ShopCreatePanel: resourceB_item을 찾을 수 없습니다!");
            }
            
            if (itemResourceC != null)
            {
                map[ECurrencyData.ResourceC] = itemResourceC;
            }
            else
            {
                Debug.LogWarning("ShopCreatePanel: resourceC_item을 찾을 수 없습니다!");
            }
            
            if (itemResourceDefault != null)
            {
                map[ECurrencyData.ResourceDefault] = itemResourceDefault;
            }
            else
            {
                Debug.LogWarning("ShopCreatePanel: resourceDefault_item을 찾을 수 없습니다!");
            }

            return map;
        }

    }
}
