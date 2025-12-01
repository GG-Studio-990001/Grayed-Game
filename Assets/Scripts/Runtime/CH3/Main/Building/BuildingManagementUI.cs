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
                Debug.Log($"BuildingManagementUI: Recovery 버튼 리스너 등록 완료. interactable={recoveryButton.interactable}");
            }
            else
            {
                Debug.LogWarning("BuildingManagementUI: recoveryButton이 null입니다!");
            }
            
            if (demolitionButton != null)
            {
                demolitionButton.onClick.RemoveAllListeners();
                demolitionButton.onClick.AddListener(OnDemolitionButtonClicked);
                Debug.Log($"BuildingManagementUI: Demolition 버튼 리스너 등록 완료. interactable={demolitionButton.interactable}");
            }
            else
            {
                Debug.LogWarning("BuildingManagementUI: demolitionButton이 null입니다!");
            }
        }
        
        /// <summary>
        /// 건축물 관리창 표시
        /// </summary>
        public void Show(Producer producer)
        {
            if (producer == null)
            {
                Debug.LogWarning("Producer가 null입니다!");
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
                _currentProduction.OnProductionChanged += UpdateProductionDisplay;
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
                _currentProduction.OnProductionChanged -= UpdateProductionDisplay;
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
                        Debug.LogWarning($"BuildingManagementUI: {displayItem.itemName}의 itemIcon이 null입니다!");
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
            else
            {
                Debug.LogWarning("BuildingManagementUI: itemImage가 null입니다!");
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
            bool hasProducedItems = producedItems.Count > 0;
            
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
            Debug.Log("OnRecoveryButtonClicked 호출됨");
            
            if (_currentProduction == null)
            {
                Debug.LogWarning("BuildingManagementUI: _currentProduction이 null입니다!");
                return;
            }
            
            bool success = _currentProduction.RecoverProducedItems();
            if (success)
            {
                UpdateProductionDisplay();
                UpdateButtonStates();
            }
            else
            {
                Debug.LogWarning("생산된 아이템 수거에 실패했습니다. 인벤토리가 가득 찼을 수 있습니다.");
            }
        }
        
        /// <summary>
        /// Demolition 버튼 클릭 처리 - 건물 제거 및 건설 재료 반환
        /// </summary>
        private void OnDemolitionButtonClicked()
        {
            Debug.Log("OnDemolitionButtonClicked 호출됨");
            
            if (_currentProducer == null)
            {
                Debug.LogWarning("BuildingManagementUI: _currentProducer이 null입니다!");
                return;
            }
            
            var buildingData = _currentProducer.GetBuildingData();
            if (buildingData == null)
            {
                Debug.LogWarning("건물 데이터를 찾을 수 없습니다.");
                _currentProducer.Remove();
                Hide();
                return;
            }
            
            if (buildingData.buildCurrency == null || buildingData.buildCurrency.Count == 0)
            {
                EnsureBuildCurrency(buildingData);
                if (buildingData.buildCurrency == null || buildingData.buildCurrency.Count == 0)
                {
                    Debug.LogWarning($"건설 재료 정보를 찾을 수 없습니다. (id: {buildingData.id})");
                    _currentProducer.Remove();
                    Hide();
                    return;
                }
            }
            
            var inventory = FindObjectOfType<Inventory>();
            if (inventory == null)
            {
                Debug.LogWarning("인벤토리를 찾을 수 없습니다.");
                _currentProducer.Remove();
                Hide();
                return;
            }
            
            var currencyToItemMap = GetCurrencyToItemMap();
            bool allSuccess = true;
            
            foreach (var currencyData in buildingData.buildCurrency)
            {
                if (!currencyToItemMap.TryGetValue(currencyData.currency, out Item item))
                {
                    Debug.LogWarning($"건설 재료 {currencyData.currency}에 해당하는 Item을 찾을 수 없습니다!");
                    allSuccess = false;
                    continue;
                }
                
                if (!inventory.TryAdd(item, currencyData.amount))
                {
                    Debug.LogWarning($"건설 재료 {currencyData.currency} {currencyData.amount}개를 인벤토리에 추가하는데 실패했습니다.");
                    allSuccess = false;
                }
            }
            
            if (!allSuccess)
            {
                Debug.LogWarning("일부 건설 재료를 인벤토리에 추가하는데 실패했습니다. 인벤토리가 가득 찼을 수 있습니다.");
            }
            
            _currentProducer.Remove();
            Hide();
        }
        
        /// <summary>
        /// 건설 재료 정보가 없을 경우 자동으로 채워줌
        /// </summary>
        private void EnsureBuildCurrency(CH3_LevelData buildingData)
        {
            if (buildingData == null || string.IsNullOrEmpty(buildingData.id)) return;
            
            if (buildingData.buildCurrency == null)
            {
                buildingData.buildCurrency = new List<CurrencyData>();
            }
            
            if (buildingData.buildCurrency.Count == 0)
            {
                switch (buildingData.id)
                {
                    case "Build":
                        buildingData.buildCurrency.Add(new CurrencyData(ECurrencyData.Tree, 10));
                        buildingData.buildCurrency.Add(new CurrencyData(ECurrencyData.Stone, 5));
                        break;
                    case "endingPortal":
                        buildingData.buildCurrency.Add(new CurrencyData(ECurrencyData.Stone, 12));
                        break;
                    case "lvFactoryS":
                        buildingData.buildCurrency.Add(new CurrencyData(ECurrencyData.Stone, 10));
                        buildingData.buildCurrency.Add(new CurrencyData(ECurrencyData.Tree, 10));
                        break;
                    case "lvFactoryL":
                        buildingData.buildCurrency.Add(new CurrencyData(ECurrencyData.Stone, 20));
                        buildingData.buildCurrency.Add(new CurrencyData(ECurrencyData.Tree, 20));
                        buildingData.buildCurrency.Add(new CurrencyData(ECurrencyData.Coin, 20));
                        break;
                    case "factoryTimber":
                        buildingData.buildCurrency.Add(new CurrencyData(ECurrencyData.ResourceA, 5));
                        buildingData.buildCurrency.Add(new CurrencyData(ECurrencyData.ResourceDefault, 5));
                        break;
                    case "factoryStone":
                        buildingData.buildCurrency.Add(new CurrencyData(ECurrencyData.ResourceB, 5));
                        buildingData.buildCurrency.Add(new CurrencyData(ECurrencyData.ResourceDefault, 5));
                        break;
                    case "factoryCoin":
                        buildingData.buildCurrency.Add(new CurrencyData(ECurrencyData.ResourceC, 5));
                        buildingData.buildCurrency.Add(new CurrencyData(ECurrencyData.ResourceDefault, 5));
                        break;
                    case "skillResetTicket":
                        buildingData.buildCurrency.Add(new CurrencyData(ECurrencyData.Coin, 50));
                        break;
                }
            }
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

