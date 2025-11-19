using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
        }

        public void ToggleInventory()
        {
            inventoryOpen = !inventoryOpen;
            if (inventoryPanel != null) inventoryPanel.SetActive(inventoryOpen);
            
            // 인벤토리를 닫을 때 툴팁도 숨기기
            if (!inventoryOpen)
            {
                InventoryTooltip.Hide();
            }
        }
        
        public bool IsInventoryOpen() => inventoryOpen;

        public int GetSelectedHotbarIndex() => selectedHotbar;
        
        // 줄 간격은 상위 VerticalLayoutGroup.spacing으로 조정하세요.
    }
}


