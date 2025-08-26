using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Runtime.CH3.Main
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;
        [SerializeField] private Transform slotsRoot;
        [SerializeField] private Transform hotbarRoot;
        [SerializeField] private Slot slotPrefab;
        [SerializeField] private TextMeshProUGUI tooltipText;
        [SerializeField] private GameObject inventoryPanel; // 전체 인벤토리 열고닫기

        private readonly List<Slot> slotViews = new List<Slot>();
        private readonly List<Slot> hotbarViews = new List<Slot>();
        private int selectedHotbar = 0;
        private bool inventoryOpen;

        private void Awake()
        {
            // Bind hover tooltip once
            if (tooltipText != null) Slot.hoverText = tooltipText;
            BuildViews();
            inventory.OnSlotChanged += HandleSlotChanged;
            RefreshAll();
            SelectHotbar(0);
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
                var v = Instantiate(slotPrefab, slotsRoot);
                v.SetIndex(
                    i,
                    this,
                    getter: (idx) => { var s = inventory.GetSlot(idx); return (s?.item, s?.count ?? 0); },
                    mover: (from, to) => inventory.MoveOrMerge(from, to),
                    clearer: (idx) => inventory.ClearSlot(idx)
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
                    clearer: (idx) => inventory.ClearSlot(idx)
                );
                v.SetHotbarView(true);
                hotbarViews.Add(v);
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
        }

        public int GetSelectedHotbarIndex() => selectedHotbar;
    }
}


