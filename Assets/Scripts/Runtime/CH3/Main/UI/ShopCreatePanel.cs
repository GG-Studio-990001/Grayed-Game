using System.Collections;
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
        private readonly List<ShopCreateItemText> itemPool = new List<ShopCreateItemText>(); // 재사용을 위한 아이템 풀
        private ShopCreateItemText selectedItem;
        private Dictionary<string, CH3_LevelData> levelDataCache;
        private List<string> itemIdOrder = new List<string>();
        private Dictionary<ECurrencyData, Item> _currencyToItemMapCache;
        private Inventory _inventory;
        private bool _isLoadingItems = false;

        private void Awake()
        {
            // 씬 재시작 시 상태 완전 초기화
            if (itemViews != null)
            {
                itemViews.Clear();
            }
            
            // 아이템 풀도 초기화 (씬 재시작 시 오브젝트가 파괴되므로)
            if (itemPool != null)
            {
                // 파괴된 참조 제거
                itemPool.RemoveAll(item => item == null || item.gameObject == null);
            }
            
            selectedItem = null;
            _isLoadingItems = false;
            levelDataCache = null;
            itemIdOrder.Clear();
            
            InitializeReferences();
            PreloadCurrencyToItemMap();
            SubscribeToInventoryEvents();
        }

        private void Start()
        {
            // Start에서는 참조만 초기화 (로드는 Show()에서만 수행)
            if (contentParent == null)
            {
                InitializeReferences();
            }
        }
        
        private void OnEnable()
        {
            // UI가 활성화될 때 참조 초기화
            InitializeReferences();
            
            // 인벤토리 이벤트 재구독 (씬 전환 등으로 인벤토리가 바뀔 수 있음)
            SubscribeToInventoryEvents();
            
            // Show()에서 호출될 수 있으므로 여기서는 로드하지 않음
            // Show()에서 명시적으로 로드하도록 변경
        }

        private IEnumerator LoadItemsWhenReady()
        {
            // 중복 실행 방지
            if (_isLoadingItems)
            {
                yield break;
            }
            
            _isLoadingItems = true;
            
            // 최대 30프레임까지 contentParent가 준비될 때까지 대기 (씬 재시작 시 더 오래 기다림)
            int maxWaitFrames = 30;
            int waitedFrames = 0;
            
            while (contentParent == null && waitedFrames < maxWaitFrames)
            {
                InitializeReferences();
                yield return null;
                waitedFrames++;
            }
            
            // contentParent가 준비되었으면 항상 로드
            if (contentParent != null)
            {
                // 기존 아이템 완전히 정리 (중복 방지)
                ClearItems();
                
                // 씬 재시작 시에도 항상 로드하여 정상 작동 보장
                LoadItemsFromCSV();
                RefreshSoldOutStates();
                
                // 첫 번째 아이템 자동 선택 (재료 정보 표시를 위해) - 코루틴으로 지연 실행
                StartCoroutine(SelectFirstItemDelayed());
            }
            else
            {
                Debug.LogError($"ShopCreatePanel: contentParent를 찾을 수 없습니다! (대기 프레임: {waitedFrames}/{maxWaitFrames}, gameObject.activeInHierarchy: {gameObject.activeInHierarchy})");
            }
            
            _isLoadingItems = false;
        }

        private void CleanupInvalidItemViews()
        {
            // 파괴된 GameObject 참조 제거
            if (itemViews == null) return;
            
            for (int i = itemViews.Count - 1; i >= 0; i--)
            {
                if (itemViews[i] == null || itemViews[i].gameObject == null)
                {
                    itemViews.RemoveAt(i);
                }
            }
            
            // 선택된 아이템이 유효하지 않으면 null로 설정
            if (selectedItem != null && (selectedItem.gameObject == null || !itemViews.Contains(selectedItem)))
            {
                selectedItem = null;
            }
        }

        private bool HasValidItems()
        {
            // contentParent가 없으면 false
            if (contentParent == null)
            {
                return false;
            }
            
            // contentParent의 실제 자식 개수 확인 (씬 재시작 시 파괴된 GameObject는 자식이 아님)
            int actualChildCount = 0;
            for (int i = 0; i < contentParent.childCount; i++)
            {
                var child = contentParent.GetChild(i);
                if (child != null && child.name.StartsWith("CreateltemText"))
                {
                    actualChildCount++;
                }
            }
            
            // 실제 자식이 있고, itemViews와 개수가 일치하면 true
            // 씬 재시작 시 contentParent의 자식이 모두 파괴되므로 actualChildCount는 0이 됨
            return actualChildCount > 0 && itemViews != null && itemViews.Count == actualChildCount;
        }

        private void OnDisable()
                {
            UnsubscribeFromInventoryEvents();
            
            // 상태 리셋 (오브젝트는 파괴하지 않고 비활성화만)
            if (selectedItem != null)
            {
                selectedItem.SetSelected(false);
                selectedItem = null;
            }
            
            // 모든 아이템 비활성화 (풀에 반환)
            ClearItems();
            
            // 로딩 플래그 리셋
            _isLoadingItems = false;
        }

        private void OnDestroy()
        {
            UnsubscribeFromInventoryEvents();
        }

        private void SubscribeToInventoryEvents()
        {
            UnsubscribeFromInventoryEvents();
            
            _inventory = FindObjectOfType<Inventory>();
            if (_inventory != null)
            {
                _inventory.OnSlotChanged += HandleInventorySlotChanged;
            }
        }

        private void UnsubscribeFromInventoryEvents()
        {
            if (_inventory != null)
            {
                _inventory.OnSlotChanged -= HandleInventorySlotChanged;
                _inventory = null;
                }
        }

        private void HandleInventorySlotChanged(int slotIndex, ItemStack stack)
        {
            // 인벤토리가 변경되면 품절 상태 갱신
            if (gameObject.activeInHierarchy)
            {
                RefreshSoldOutStates();
            }
        }
        
        private void PreloadCurrencyToItemMap()
        {
            _currencyToItemMapCache = GetCurrencyToItemMap();
        }

        private void InitializeReferences()
        {
            var creatList = CH3Utils.FindChildByNameIgnoreCase(transform, "CreatList");
            if (creatList == null)
            {
                Debug.LogWarning($"ShopCreatePanel: CreatList를 찾을 수 없습니다. transform: {transform.name}, activeInHierarchy: {gameObject.activeInHierarchy}");
                return;
            }
            
            var scrollView = CH3Utils.FindChildByNameIgnoreCase(creatList, "Scroll View");
            if (scrollView == null)
            {
                Debug.LogWarning($"ShopCreatePanel: Scroll View를 찾을 수 없습니다.");
                return;
            }
            
            var viewport = CH3Utils.FindChildByNameIgnoreCase(scrollView, "Viewport");
            if (viewport == null)
            {
                Debug.LogWarning($"ShopCreatePanel: Viewport를 찾을 수 없습니다.");
                return;
            }
            
            contentParent = CH3Utils.FindChildByNameIgnoreCase(viewport, "Content");
            if (contentParent == null)
            {
                Debug.LogWarning($"ShopCreatePanel: Content를 찾을 수 없습니다.");
                return;
            }

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

            // itemPrefab이 null이거나 파괴된 GameObject를 참조하는 경우 다시 찾기
            if (contentParent != null)
            {
                if (itemPrefab == null || itemPrefab == null) // Unity의 null 체크 (파괴된 객체)
            {
                var firstItem = CH3Utils.FindChildByNameIgnoreCase(contentParent, "CreateltemText");
                if (firstItem != null)
                {
                    itemPrefab = firstItem.gameObject;
                    }
                }
                else
                {
                    // itemPrefab이 유효한지 확인 (파괴되지 않았는지)
                    // contentParent의 자식인 경우 인스턴스일 수 있으므로 원본을 찾아야 함
                    if (!itemPrefab.activeInHierarchy || itemPrefab.transform.parent == contentParent)
                    {
                        // 인스턴스인 경우, 원본 프리팹을 찾거나 첫 번째 자식을 원본으로 사용
                        var firstItem = CH3Utils.FindChildByNameIgnoreCase(contentParent, "CreateltemText");
                        if (firstItem != null && firstItem.gameObject != itemPrefab)
                        {
                            // 원본이 아닌 경우, 첫 번째 자식을 원본으로 설정하지 않고
                            // Inspector에서 설정된 itemPrefab을 유지하거나 다시 찾기
                            // 여기서는 첫 번째 자식을 원본으로 사용 (임시 해결책)
                            // 실제로는 Inspector에서 프리팹을 설정해야 함
                        }
                    }
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
            if (shopController == null || shopController.SelectionPanel == null)
            {
                // SelectionPanel이 없으면 나중에 재시도
                StartCoroutine(UpdateSelectionPanelDelayed(levelData));
                return;
            }

            var selectionPanel = shopController.SelectionPanel;
            
            // SelectionPanel이 비활성화되어 있으면 활성화 시도
            if (!selectionPanel.gameObject.activeInHierarchy)
            {
                selectionPanel.Show();
                // 활성화 후 한 프레임 기다렸다가 다시 시도
                StartCoroutine(UpdateSelectionPanelDelayed(levelData));
                return;
            }
            
            UpdateSelectionPanelInternal(levelData, selectionPanel);
        }

        private IEnumerator UpdateSelectionPanelDelayed(CH3_LevelData levelData)
        {
            yield return null;
            
            var shopController = GetComponentInParent<ShopUIController>();
            if (shopController == null || shopController.SelectionPanel == null) yield break;

            var selectionPanel = shopController.SelectionPanel;
            
            // SelectionPanel이 비활성화되어 있으면 활성화 시도
            if (!selectionPanel.gameObject.activeInHierarchy)
            {
                selectionPanel.Show();
                yield return null;
            }
            
            UpdateSelectionPanelInternal(levelData, selectionPanel);
        }

        private void UpdateSelectionPanelInternal(CH3_LevelData levelData, ShopSelectionPanel selectionPanel)
        {
            if (selectionPanel == null) return;

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
                    selectionPanel.SetBottomItemImage(i, null);
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
            
            // 인벤토리 이벤트 구독
            SubscribeToInventoryEvents();
            
            // 로딩 플래그 리셋하여 강제로 로드하도록 함
            _isLoadingItems = false;
            
            // 코루틴으로 지연하여 UI가 완전히 준비된 후 로드
            // Show()에서 명시적으로 호출하여 항상 로드 보장
            StartCoroutine(LoadItemsWhenReady());
        }

        private IEnumerator SelectFirstItemDelayed()
        {
            // 한 프레임 기다려서 모든 초기화가 완료되도록 함
            yield return null;
            
            // SelectionPanel이 활성화될 때까지 대기
            var shopController = GetComponentInParent<ShopUIController>();
            if (shopController != null && shopController.SelectionPanel != null)
            {
                // SelectionPanel이 비활성화되어 있으면 활성화
                if (!shopController.SelectionPanel.gameObject.activeInHierarchy)
                {
                    shopController.SelectionPanel.Show();
                }
                
                // SelectionPanel이 완전히 활성화되고 초기화될 때까지 대기
                int maxWaitFrames = 10;
                int waitedFrames = 0;
                while (!shopController.SelectionPanel.gameObject.activeInHierarchy && waitedFrames < maxWaitFrames)
                {
                    yield return null;
                    waitedFrames++;
            }
                
                // 한 프레임 더 기다려서 InitializeReferences가 완료되도록 함
                yield return null;
            }
            
            SelectFirstItemIfNoneSelected();
        }

        public void SelectFirstItemIfNoneSelected()
        {
            // 선택된 아이템이 없고 아이템이 있으면 첫 번째 아이템 선택
            if (selectedItem == null && itemViews.Count > 0 && itemViews[0] != null)
            {
                OnItemClicked(itemViews[0]);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ClearItems()
        {
            // 모든 아이템을 비활성화하여 풀로 반환 (삭제하지 않음)
            foreach (var item in itemViews)
            {
                if (item != null && item.gameObject != null)
                {
                    item.gameObject.SetActive(false);
                    item.SetSelected(false);
                }
            }
            itemViews.Clear();
            selectedItem = null;
        }
        
        private ShopCreateItemText GetOrCreateItemFromPool()
        {
            // 풀에서 비활성화된 아이템 찾기
            foreach (var item in itemPool)
            {
                if (item != null && !item.gameObject.activeSelf)
                {
                    item.gameObject.SetActive(true);
                    return item;
                }
            }
            
            // 풀에 사용 가능한 아이템이 없으면 새로 생성
            if (itemPrefab == null || contentParent == null)
            {
                Debug.LogError("ShopCreatePanel: itemPrefab 또는 contentParent가 null입니다!");
                return null;
            }
            
            var itemView = Instantiate(itemPrefab, contentParent);
            itemView.name = $"CreateltemText_{itemPool.Count}";
            
            var itemText = Utils.GetOrAddComponent<ShopCreateItemText>(itemView);
            SetupItemClickHandler(itemText);
            
            // 풀에 추가
            itemPool.Add(itemText);
            
            return itemText;
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
            if (levelData == null)
            {
                Debug.LogError($"ShopCreatePanel: AddItem() 실패 - levelData가 null입니다!");
                return;
            }

            // 풀에서 아이템 가져오기 또는 새로 생성
            var itemText = GetOrCreateItemFromPool();
            if (itemText == null)
            {
                Debug.LogError($"ShopCreatePanel: AddItem() 실패 - 아이템을 생성할 수 없습니다!");
                return;
            }
            
            // 아이템 데이터 설정
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
            if (contentParent == null)
            {
                Debug.LogError("ShopCreatePanel: LoadItemsFromCSV() 호출 시 contentParent가 null입니다!");
                return;
            }
            
            // itemPrefab이 null이면 다시 찾기 (ClearItems() 후 파괴되었을 수 있음)
            if (itemPrefab == null)
            {
                var firstItem = CH3Utils.FindChildByNameIgnoreCase(contentParent, "CreateltemText");
                if (firstItem != null)
                {
                    itemPrefab = firstItem.gameObject;
                    Debug.Log("ShopCreatePanel: itemPrefab을 다시 찾았습니다.");
                }
                else
                {
                    Debug.LogError("ShopCreatePanel: itemPrefab을 찾을 수 없습니다! contentParent에 CreateltemText가 없습니다.");
                    return;
                }
            }
            
            // 캐시 클리어 후 다시 로드 (씬 재시작 시 ScriptableObject 인스턴스가 유효하지 않을 수 있음)
            CH3_LevelDataCSVLoader.ClearCache();
            levelDataCache = CH3_LevelDataCSVLoader.LoadAllFromCSV();
            
            if (levelDataCache == null || levelDataCache.Count == 0)
            {
                Debug.LogError("ShopCreatePanel: levelDataCache가 비어있습니다!");
                return;
            }
            
            // ClearItems()는 LoadItemsWhenReady()에서 호출되므로 여기서는 itemIdOrder만 클리어
            itemIdOrder.Clear();

            var buildableItems = CH3_LevelDataCSVLoader.GetBuildableItems();
            if (buildableItems == null)
            {
                Debug.LogError("ShopCreatePanel: GetBuildableItems()가 null을 반환했습니다!");
                return;
            }
            
            Debug.Log($"ShopCreatePanel: LoadItemsFromCSV() - {buildableItems.Count}개 아이템 로드 시작 (itemPrefab: {(itemPrefab != null ? itemPrefab.name : "null")}, contentParent: {(contentParent != null ? contentParent.name : "null")})");
            
            int successCount = 0;
            int failCount = 0;
            
            foreach (var itemData in buildableItems)
            {
                if (itemData == null)
                {
                    Debug.LogError($"ShopCreatePanel: buildableItems에 null 항목이 포함되어 있습니다!");
                    failCount++;
                    continue;
                }
                
                if (string.IsNullOrEmpty(itemData.id))
                {
                    Debug.LogError($"ShopCreatePanel: itemData.id가 비어있습니다! dev: {itemData.dev}");
                    failCount++;
                    continue;
                }
                
                itemIdOrder.Add(itemData.id);
                bool soldOut = CheckItemSoldOut(itemData.id);
                AddItem(itemData, soldOut);
                
                if (itemViews.Count > successCount)
                {
                    successCount++;
                }
                else
                {
                    failCount++;
                }
            }
            
            Debug.Log($"ShopCreatePanel: LoadItemsFromCSV() 완료 - {itemViews.Count}개 아이템 생성됨 (성공: {successCount}, 실패: {failCount})");
        }

        private bool CheckItemSoldOut(string itemId)
        {
            if (_inventory == null)
            {
                _inventory = FindObjectOfType<Inventory>();
            }
            if (_inventory == null) return false;

            var itemSO = LoadItemById(itemId);
            if (itemSO == null) return false;

            for (int i = 0; i < _inventory.Slots.Count; i++)
            {
                var slot = _inventory.GetSlot(i);
                if (slot != null && slot.item == itemSO && slot.count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public void RefreshSoldOutStates()
        {
            if (_inventory == null)
            {
                _inventory = FindObjectOfType<Inventory>();
            }
            if (_inventory == null) return;

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
            if (selectedItem == null || selectedItem.LevelData == null)
            {
                Debug.LogWarning("제작할 아이템이 선택되지 않았습니다.");
                return;
            }

            var levelData = selectedItem.LevelData;
            if (_inventory == null)
            {
                _inventory = FindObjectOfType<Inventory>();
            }

            if (_inventory == null)
            {
                Debug.LogWarning("인벤토리를 찾을 수 없습니다.");
                return;
            }

            // 이미 제작된 아이템이 있는지 확인 (maxBuild 제한)
            if (CheckItemSoldOut(levelData.id))
            {
                Debug.LogWarning($"이미 최대 제작 개수({levelData.maxBuild})에 도달했습니다.");
                return;
            }

            if (levelData.buildCurrency == null || levelData.buildCurrency.Count == 0)
            {
                Debug.LogWarning("제작 재료가 설정되지 않았습니다.");
                return;
            }

            // 재료 확인
            if (!HasRequiredMaterials(levelData.buildCurrency))
            {
                Debug.LogWarning("재료가 부족합니다.");
                return;
            }

            // 재료 소비
            if (!ConsumeMaterials(levelData.buildCurrency))
            {
                Debug.LogWarning("재료 소모에 실패했습니다.");
                return;
            }

            // 아이템 로드
            var itemSO = LoadItemById(levelData.id);
            if (itemSO == null)
            {
                Debug.LogError($"Item ScriptableObject를 찾을 수 없습니다: {levelData.id}_Item");
                return;
            }

            // 인벤토리에 아이템 추가
            if (!_inventory.TryAdd(itemSO, 1))
            {
                Debug.LogWarning("인벤토리가 가득 찼습니다.");
                return;
            }

            // 제작 성공
            Debug.Log($"제작 성공: {levelData.dev} (ID: {levelData.id})");
            
            // UI 상태 업데이트
            RefreshSoldOutStates();
            UpdateCreateButtonState();
            
            // 선택된 아이템의 재료 정보 업데이트 (소비 후 변경된 재료 개수 반영)
            if (selectedItem != null && selectedItem.LevelData != null)
            {
                UpdateSelectionPanel(selectedItem.LevelData);
            }
        }

        private bool ConsumeMaterials(List<CurrencyData> currencies)
        {
            if (currencies == null || currencies.Count == 0) return true;

            if (_inventory == null)
            {
                _inventory = FindObjectOfType<Inventory>();
            }
            if (_inventory == null) return false;

            var currencyToItemMap = GetCurrencyToItemMap();

            foreach (var currency in currencies)
            {
                if (!currencyToItemMap.TryGetValue(currency.currency, out Item requiredItem))
                {
                    return false;
                }

                int remaining = currency.amount;
                for (int i = 0; i < _inventory.Slots.Count && remaining > 0; i++)
                {
                    var slot = _inventory.GetSlot(i);
                    if (slot != null && slot.item == requiredItem && slot.count > 0)
                    {
                        int consume = Mathf.Min(remaining, slot.count);
                        if (_inventory.TryConsumeAt(i, consume))
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

            if (_inventory == null)
            {
                _inventory = FindObjectOfType<Inventory>();
            }
            if (_inventory == null) return false;

            var currencyToItemMap = GetCurrencyToItemMap();

            foreach (var currency in currencies)
            {
                if (!currencyToItemMap.TryGetValue(currency.currency, out Item requiredItem))
                {
                    return false;
                }

                int totalAmount = 0;
                for (int i = 0; i < _inventory.Slots.Count; i++)
                {
                    var slot = _inventory.GetSlot(i);
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
