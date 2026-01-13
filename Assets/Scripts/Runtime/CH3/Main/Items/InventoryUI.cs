using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Runtime.ETC;
using UnityEngine.UI;

namespace Runtime.CH3.Main
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private Transform slotsTopRoot;   // 첫 번째 줄 컨테이너
        [SerializeField] private Transform slotsBottomRoot; // 두 번째 줄 이후 컨테이너
        [SerializeField] private Transform hotbarRoot;
        [SerializeField] private Slot slotPrefab;
        [SerializeField] private TextMeshProUGUI tooltipText;
        [SerializeField] private GameObject inventoryPanel; // 전체 인벤토리 열고닫기

        private readonly List<Slot> slotViews = new List<Slot>();
        private readonly List<Slot> hotbarViews = new List<Slot>();
        private int selectedHotbar = 0;
        private bool inventoryOpen;
        private CH3KeyBinder _keyBinder; // CH3KeyBinder 참조 캐싱 (성능 최적화)
        private BuildingSystem _buildingSystem; // BuildingSystem 참조 캐싱

        private void Awake()
        {
            // Bind hover tooltip once
            if (tooltipText != null) Slot.hoverText = tooltipText;
            BuildViews();
            inventory.OnSlotChanged += HandleSlotChanged;
            RefreshAll();
            SelectHotbar(0);
            
            // CH3KeyBinder 참조 캐싱 (성능 최적화)
            _keyBinder = FindObjectOfType<CH3KeyBinder>();
            _buildingSystem = BuildingSystem.Instance;
        }

        private void OnDestroy()
        {
            if (inventory != null)
            {
                inventory.OnSlotChanged -= HandleSlotChanged;
            }
        }

        private void BuildViews()
        {
            for (int i = 0; i < Inventory.Capacity; i++)
            {
                // 첫 번째 줄(0~Width-1)은 상단 컨테이너, 나머지는 하단 컨테이너에 배치
                Transform parent = (i < Inventory.Width) ? slotsTopRoot : slotsBottomRoot;
                var v = Instantiate(slotPrefab, parent);
                v.SetIndex(
                    i,
                    this,
                    getter: (idx) => { var s = inventory.GetSlot(idx); return (s?.item, s?.count ?? 0); },
                    mover: (from, to) => inventory.MoveOrMerge(from, to),
                    clearer: (idx) => inventory.ClearSlot(idx),
                    useItemCallback: (idx) => OnSlotUseItem(idx)
                );
                v.SetHotbarView(false);
                slotViews.Add(v);
            }

            for (int i = 0; i < Inventory.HotbarSize; i++)
            {
                var v = Instantiate(slotPrefab, hotbarRoot);
                v.SetIndex(
                    i,
                    this,
                    getter: (idx) => { var s = inventory.GetSlot(idx); return (s?.item, s?.count ?? 0); },
                    mover: (from, to) => inventory.MoveOrMerge(from, to),
                    clearer: (idx) => inventory.ClearSlot(idx),
                    useItemCallback: (idx) => OnSlotUseItem(idx)
                );
                v.SetHotbarView(true);
                hotbarViews.Add(v);
            }
        }
        
        /// <summary>
        /// 슬롯에서 아이템 사용 시 호출되는 콜백
        /// </summary>
        private void OnSlotUseItem(int slotIndex)
        {
            // CH3KeyBinder의 UseItem 메서드 호출 (캐싱된 참조 사용)
            if (_keyBinder != null)
            {
                _keyBinder.UseItem(slotIndex);
            }
        }

        private void HandleSlotChanged(int index, ItemStack stack)
        {
            if (index >= 0 && index < slotViews.Count)
            {
                slotViews[index].Refresh();
            }
            if (index >= 0 && index < hotbarViews.Count)
            {
                hotbarViews[index].Refresh();
            }
        }

        // 외부에서 드롭 위치 판정에 사용할 루트 Transform 노출
        public Transform GetInventoryPanelTransform() => inventoryPanel != null ? inventoryPanel.transform : transform;
        public Transform GetHotbarRootTransform() => hotbarRoot != null ? hotbarRoot.transform : null;

        public void RefreshAll()
        {
            for (int i = 0; i < slotViews.Count; i++) slotViews[i].Refresh();
            for (int i = 0; i < hotbarViews.Count; i++) hotbarViews[i].Refresh();
        }

        public void SelectHotbar(int index)
        {
            selectedHotbar = Mathf.Clamp(index, 0, Inventory.HotbarSize - 1);
            for (int i = 0; i < hotbarViews.Count; i++)
            {
                hotbarViews[i].SetSelected(i == selectedHotbar);
            }
            
            // 빌드모드 처리
            ProcessHotbarSelection(selectedHotbar);
        }
        
        /// <summary>
        /// 핫바 선택 후 빌드모드 처리
        /// </summary>
        private void ProcessHotbarSelection(int index)
        {
            // CH3KeyBinder의 OnHotbarSelected 호출
            if (_keyBinder == null)
            {
                _keyBinder = FindObjectOfType<CH3KeyBinder>();
            }
            
            if (_keyBinder != null)
            {
                _keyBinder.OnHotbarSelected(index);
            }
            else
            {
                // CH3KeyBinder를 찾을 수 없으면 직접 처리
                if (_buildingSystem == null)
                {
                    _buildingSystem = BuildingSystem.Instance;
                }
                
                if (_buildingSystem != null && inventory != null)
                {
                    var slot = inventory.GetSlot(index);
                    
                    if (slot != null && slot.item != null)
                    {
                        // 건축 아이템이면 빌드모드 자동 진입
                        if (slot.item.IsBuildingItem)
                        {
                            if (!_buildingSystem.IsBuildingMode)
                            {
                                _buildingSystem.StartBuildingMode(slot.item, index);
                            }
                        }
                        else
                        {
                            // 다른 아이템 선택 시 빌드모드 취소
                            if (_buildingSystem.IsBuildingMode)
                            {
                                _buildingSystem.EndBuildingMode();
                            }
                        }
                    }
                    else
                    {
                        // 빈 슬롯 선택 시 빌드모드 취소
                        if (_buildingSystem.IsBuildingMode)
                        {
                            _buildingSystem.EndBuildingMode();
                        }
                    }
                }
            }
        }

        public void ToggleInventory()
        {
            inventoryOpen = !inventoryOpen;
            if (inventoryPanel != null) inventoryPanel.SetActive(inventoryOpen);
            
            // 인벤토리 열릴 때 핫바 끄기, 닫힐 때 핫바 켜기
            if (hotbarRoot != null)
            {
                hotbarRoot.gameObject.SetActive(!inventoryOpen);
            }
            
            // 인벤토리 열릴 때 핫바 선택 취소 및 빌드모드 종료
            if (inventoryOpen)
            {
                // 모든 핫바 선택 취소
                for (int i = 0; i < hotbarViews.Count; i++)
                {
                    hotbarViews[i].SetSelected(false);
                }
                selectedHotbar = -1; // 선택 없음으로 표시
                
                // 빌드모드 종료
                if (_buildingSystem == null)
                {
                    _buildingSystem = BuildingSystem.Instance;
                }
                if (_buildingSystem != null && _buildingSystem.IsBuildingMode)
                {
                    _buildingSystem.EndBuildingMode();
                }
            }
            
            // TODO: 인벤열고닫음 효과음 재생
            Managers.Sound.Play(Sound.SFX, "CH3/CH3_SFX_Inven_Onoff");
            // 인벤토리 열릴 때와 닫힐 때 모두 툴팁 숨기기
            InventoryTooltip.Hide();
        }
        
        public bool IsInventoryOpen() => inventoryOpen;

        public int GetSelectedHotbarIndex() => selectedHotbar;
        
        // 줄 간격은 상위 VerticalLayoutGroup.spacing으로 조정하세요.
    }
}


