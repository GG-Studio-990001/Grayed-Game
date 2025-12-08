using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Runtime.Input;
using System.Collections.Generic;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 건축물 관리 UI - 건축물 상호작용 시 표시되는 관리 창
    /// </summary>
    public class BuildingManagementUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject managementPanel;
        [SerializeField] private Button recoveryButton;
        [SerializeField] private Button demolitionButton;
        [SerializeField] private TextMeshProUGUI itemCountText;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private Slider productionSlider;
        [SerializeField] private Image itemImage;
        
        private Producer _currentProducer;
        private BuildingProduction _currentProduction;
        
        private void Awake()
        {
            if (managementPanel != null)
            {
                managementPanel.SetActive(false);
            }
            
            InitializeReferences();
            SetupButtons();
        }
        
        private void InitializeReferences()
        {
            if (managementPanel == null) return;
            
            if (recoveryButton == null)
            {
                var recoveryObj = CH3Utils.FindChildRecursive(managementPanel.transform, "Recovery");
                if (recoveryObj != null)
                {
                    recoveryButton = recoveryObj.GetComponent<Button>();
                }
            }
            
            if (demolitionButton == null)
            {
                var demolitionObj = CH3Utils.FindChildRecursive(managementPanel.transform, "Demolition");
                if (demolitionObj != null)
                {
                    demolitionButton = demolitionObj.GetComponent<Button>();
                }
            }
            
            if (itemCountText == null)
            {
                var itemCountObj = CH3Utils.FindChildByNameIgnoreCase(managementPanel.transform, "ItemCountText");
                if (itemCountObj != null)
                {
                    itemCountText = itemCountObj.GetComponent<TextMeshProUGUI>();
                }
            }
            
            if (productionSlider == null)
            {
                var sliderObj = CH3Utils.FindChildByNameIgnoreCase(managementPanel.transform, "Slider");
                if (sliderObj != null)
                {
                    productionSlider = sliderObj.GetComponent<Slider>();
                }
            }
            
            if (itemImage == null)
            {
                var itemImageObj = CH3Utils.FindChildByNameIgnoreCase(managementPanel.transform, "ItemImage");
                if (itemImageObj != null)
                {
                    itemImage = itemImageObj.GetComponent<Image>();
                }
            }
            
            if (itemNameText == null)
            {
                var itemNameObj = CH3Utils.FindChildByNameIgnoreCase(managementPanel.transform, "ItemNameText");
                if (itemNameObj != null)
                {
                    itemNameText = itemNameObj.GetComponent<TextMeshProUGUI>();
                }
            }
        }
        
        private void SetupButtons()
        {
            if (recoveryButton != null)
            {
                recoveryButton.onClick.RemoveAllListeners();
                recoveryButton.onClick.AddListener(OnRecoveryButtonClicked);
            }
            
            if (demolitionButton != null)
            {
                demolitionButton.onClick.RemoveAllListeners();
                demolitionButton.onClick.AddListener(OnDemolitionButtonClicked);
            }
        }
        
        /// <summary>
        /// 건축물 관리창 표시
        /// </summary>
        public void Show(Producer producer)
        {
            if (producer == null)
            {
                return;
            }
            
            _currentProducer = producer;
            _currentProduction = producer.GetComponent<BuildingProduction>();
            
            if (managementPanel != null)
            {
                managementPanel.SetActive(true);
            }
            
            // 패널이 활성화된 후 참조 다시 초기화
            InitializeReferences();
            SetupButtons();
            
            // 플레이어 이동 비활성화
            if (Managers.Data != null && Managers.Data.InGameKeyBinder != null)
            {
                Managers.Data.InGameKeyBinder.PlayerInputDisable();
            }
            
            if (_currentProduction != null)
            {
                _currentProduction.OnProductionChanged += OnProductionChanged;
            }
            
            UpdateProductionDisplay();
            UpdateButtonStates();
        }
        
        /// <summary>
        /// 건축물 관리창 숨김
        /// </summary>
        public void Hide()
        {
            if (_currentProduction != null)
            {
                _currentProduction.OnProductionChanged -= OnProductionChanged;
            }
            
            _currentProducer = null;
            _currentProduction = null;
            
            if (managementPanel != null)
            {
                managementPanel.SetActive(false);
            }
            
            // 플레이어 이동 활성화
            if (Managers.Data != null && Managers.Data.InGameKeyBinder != null)
            {
                Managers.Data.InGameKeyBinder.PlayerInputEnable();
            }
        }
        
        /// <summary>
        /// 생산된 아이템 개수 표시 업데이트
        /// </summary>
        private void UpdateProductionDisplay()
        {
            var buildingData = _currentProducer != null ? _currentProducer.GetBuildingData() : null;
            
            if (_currentProduction == null || buildingData == null)
            {
                if (itemCountText != null)
                {
                    itemCountText.text = "0";
                }
                if (productionSlider != null)
                {
                    productionSlider.value = 0f;
                }
                if (itemImage != null)
                {
                    itemImage.sprite = null;
                    itemImage.color = new Color(1, 1, 1, 0);
                }
                if (itemNameText != null)
                {
                    itemNameText.text = "";
                }
                return;
            }
            
            var producedItems = _currentProduction.GetProducedItems();
            int totalCount = 0;
            Item displayItem = null;
            
            foreach (var kvp in producedItems)
            {
                totalCount += kvp.Value;
                if (displayItem == null && kvp.Key != null)
                {
                    displayItem = kvp.Key;
                }
            }
            
            if (displayItem == null && buildingData.productionItems != null && buildingData.productionItems.Count > 0)
            {
                foreach (var productionData in buildingData.productionItems)
                {
                    if (productionData.item != null)
                    {
                        displayItem = productionData.item;
                        break;
                    }
                }
            }
            
            int maxProduction = buildingData.maxProduction > 0 ? buildingData.maxProduction : 0;
            
            if (itemCountText != null)
            {
                if (maxProduction > 0)
                {
                    itemCountText.text = $"{totalCount} / {maxProduction}";
                }
                else
                {
                    itemCountText.text = totalCount.ToString();
                }
            }
            
            if (productionSlider != null)
            {
                if (maxProduction > 0)
                {
                    productionSlider.maxValue = maxProduction;
                    productionSlider.value = totalCount;
                }
                else
                {
                    productionSlider.maxValue = 1f;
                    productionSlider.value = totalCount > 0 ? 1f : 0f;
                }
            }
            
            if (itemImage != null)
            {
                if (displayItem != null)
                {
                    if (displayItem.itemIcon != null)
                    {
                        itemImage.sprite = displayItem.itemIcon;
                        itemImage.color = new Color(1, 1, 1, 1);
                    }
                    else
                    {
                        itemImage.sprite = null;
                        itemImage.color = new Color(1, 1, 1, 0);
                    }
                }
                else
                {
                    itemImage.sprite = null;
                    itemImage.color = new Color(1, 1, 1, 0);
                }
            }
            
            if (itemNameText != null)
            {
                if (displayItem != null && !string.IsNullOrEmpty(displayItem.itemName))
                {
                    itemNameText.text = displayItem.itemName;
                }
                else
                {
                    itemNameText.text = "";
                }
            }
            
            // 생산 상태가 변경될 때마다 버튼 상태도 업데이트
            UpdateButtonStates();
        }
        
        /// <summary>
        /// 생산 상태 변경 이벤트 핸들러
        /// </summary>
        private void OnProductionChanged()
        {
            UpdateProductionDisplay();
            UpdateButtonStates();
        }
        
        /// <summary>
        /// 버튼 상태 업데이트
        /// </summary>
        private void UpdateButtonStates()
        {
            if (_currentProduction == null)
            {
                if (recoveryButton != null)
                {
                    recoveryButton.interactable = false;
                }
                return;
            }
            
            var producedItems = _currentProduction.GetProducedItems();
            // 회수할 자원 개수가 한 개라도 있으면 활성화
            bool hasProducedItems = false;
            foreach (var kvp in producedItems)
            {
                if (kvp.Value > 0)
                {
                    hasProducedItems = true;
                    break;
                }
            }
            
            if (recoveryButton != null)
            {
                recoveryButton.interactable = hasProducedItems;
            }
        }
        
        /// <summary>
        /// Recovery 버튼 클릭 처리 - 생산된 아이템을 인벤토리로 수거
        /// </summary>
        private void OnRecoveryButtonClicked()
        {
            if (_currentProduction == null)
            {
                return;
            }
            
            bool success = _currentProduction.RecoverProducedItems();
            if (success)
            {
                UpdateProductionDisplay();
                UpdateButtonStates();
            }
        }
        
        /// <summary>
        /// Demolition 버튼 클릭 처리 - 건물 제거 및 건설 재료 반환
        /// </summary>
        private void OnDemolitionButtonClicked()
        {
            if (_currentProducer == null)
            {
                return;
            }
            
            var buildingData = _currentProducer.GetBuildingData();
            if (buildingData == null || buildingData.buildCurrency == null || buildingData.buildCurrency.Count == 0)
            {
                _currentProducer.Remove();
                Hide();
                return;
            }
            
            var inventory = FindObjectOfType<Inventory>();
            if (inventory == null)
            {
                _currentProducer.Remove();
                Hide();
                return;
            }
            
            var currencyToItemMap = GetCurrencyToItemMap();
            
            foreach (var currencyData in buildingData.buildCurrency)
            {
                if (currencyToItemMap.TryGetValue(currencyData.currency, out Item item))
                {
                    inventory.TryAdd(item, currencyData.amount);
                }
            }
            
            _currentProducer.Remove();
            Hide();
        }
        
        /// <summary>
        /// ECurrencyData를 Item으로 매핑하는 딕셔너리 생성
        /// </summary>
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
            
            if (itemWood != null) map[ECurrencyData.Tree] = itemWood;
            if (itemStone != null) map[ECurrencyData.Stone] = itemStone;
            if (itemCoin != null) map[ECurrencyData.Coin] = itemCoin;
            if (itemResourceA != null) map[ECurrencyData.ResourceA] = itemResourceA;
            if (itemResourceB != null) map[ECurrencyData.ResourceB] = itemResourceB;
            if (itemResourceC != null) map[ECurrencyData.ResourceC] = itemResourceC;
            if (itemResourceDefault != null) map[ECurrencyData.ResourceDefault] = itemResourceDefault;
            
            return map;
        }
        
        /// <summary>
        /// 현재 관리 중인 건축물 반환
        /// </summary>
        public Producer GetCurrentProducer()
        {
            return _currentProducer;
        }
        
        /// <summary>
        /// 관리창이 표시 중인지 확인
        /// </summary>
        public bool IsVisible()
        {
            return managementPanel != null && managementPanel.activeSelf;
        }
    }
}

