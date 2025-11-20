using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

namespace Runtime.CH3.Main
{
    public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private GameObject selectedBorder;
        [SerializeField] private int index;
        [SerializeField] private MonoBehaviour inventoryBehaviour; // holder only
        [SerializeField] private bool isHotbarView; // 이 슬롯이 퀵슬롯 UI인지 여부
        private Func<int, (Item item, int count)> getSlotData;
        private Action<int, int> moveOrMerge;
        private Action<int> clearSlot;
        private Action<int> useItem; // 아이템 사용 콜백
        private InventoryUI ownerUI => inventoryBehaviour as InventoryUI;

        private static Slot draggingFrom;
        private static Image dragGhost;
        private static Canvas rootCanvas;
        private static bool isDragging;

        private Item _item;
        private int _count;
        public Item item
        {
            get { return _item; }
            set
            {
                _item = value;
                if (_item != null)
                {
                    image.sprite = item.itemIcon;
                    image.color = new Color(1, 1, 1, 1);
                }
                else
                {
                    image.color = new Color(1, 1, 1, 0);
                }
            }
        }

        public int count
        {
            get { return _count; }
            set
            {
                _count = Mathf.Max(0, value);
                if (countText != null)
                {
                    countText.text = _count > 1 ? _count.ToString() : string.Empty;
                }
            }
        }

        public void SetIndex(int newIndex, MonoBehaviour inv,
            System.Func<int, (Item item, int count)> getter = null,
            System.Action<int, int> mover = null,
            System.Action<int> clearer = null,
            System.Action<int> useItemCallback = null)
        {
            index = newIndex;
            inventoryBehaviour = inv;
            getSlotData = getter;
            moveOrMerge = mover;
            clearSlot = clearer;
            useItem = useItemCallback;
            Refresh();
        }

        public void SetHotbarView(bool value)
        {
            isHotbarView = value;
        }

        public void Refresh()
        {
            if (getSlotData == null) return;
            var data = getSlotData(index);
            item = data.item;
            count = data.count;
        }

        public void SetSelected(bool selected)
        {
            if (selectedBorder != null) selectedBorder.SetActive(selected);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isDragging) return;
            if (item != null)
            {
                InventoryTooltip.Show(item.itemName, GetTooltipPosition(eventData.position));
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            InventoryTooltip.Hide();
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (isDragging) return;
            if (item != null)
            {
                InventoryTooltip.Show(item.itemName, GetTooltipPosition(eventData.position));
            }
            else
            {
                InventoryTooltip.Hide();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (item == null || moveOrMerge == null) return;
            isDragging = true;
            InventoryTooltip.Hide();
            draggingFrom = this;
            EnsureGhost();
            dragGhost.sprite = image.sprite;
            dragGhost.color = Color.white;
            dragGhost.gameObject.SetActive(true);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragGhost != null && dragGhost.gameObject.activeSelf)
            {
                dragGhost.rectTransform.position = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (draggingFrom == null) return;

            dragGhost.gameObject.SetActive(false);

            var target = eventData.pointerCurrentRaycast.gameObject == null
                ? null
                : eventData.pointerCurrentRaycast.gameObject.GetComponentInParent<Slot>();

            if (target != null && target.moveOrMerge == moveOrMerge)
            {
                moveOrMerge(draggingFrom.index, target.index);
                draggingFrom.Refresh();
                target.Refresh();
            }
            else
            {
                // 버리기: 인벤토리 패널과 핫바의 Rect 안쪽이면 버리지 않음 (빈 공간 포함)
                bool droppedOutside = true;
                if (ownerUI != null)
                {
                    bool IsInside(Transform t)
                    {
                        var rt = t as RectTransform;
                        if (rt == null) return false;
                        var cam = eventData.pressEventCamera != null ? eventData.pressEventCamera : eventData.enterEventCamera;
                        return RectTransformUtility.RectangleContainsScreenPoint(rt, eventData.position, cam);
                    }

                    var panel = ownerUI.GetInventoryPanelTransform();
                    var hotbar = ownerUI.GetHotbarRootTransform();
                    if (panel != null && IsInside(panel)) droppedOutside = false;
                    if (droppedOutside && hotbar != null && IsInside(hotbar)) droppedOutside = false;
                }

                if (droppedOutside)
                {
                    clearSlot?.Invoke(draggingFrom.index);
                    draggingFrom.Refresh();
                }
            }

            draggingFrom = null;
            isDragging = false;
        }

        private Vector2 GetTooltipPosition(Vector2 pointerScreenPos)
        {
            float yOffset = isHotbarView ? 25f : -25f;
            float xOffset = 50f;
            return new Vector2(pointerScreenPos.x + xOffset, pointerScreenPos.y + yOffset);
        }

        private void EnsureGhost()
        {
            if (dragGhost != null) return;
            if (rootCanvas == null) rootCanvas = GetComponentInParent<Canvas>();
            var go = new GameObject("DragGhost", typeof(Image));
            go.transform.SetParent(rootCanvas.transform, false);
            dragGhost = go.GetComponent<Image>();
            var c = go.AddComponent<CanvasGroup>();
            c.blocksRaycasts = false;
            dragGhost.raycastTarget = false;
            go.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // 우클릭 시 아이템 사용
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (item != null && useItem != null && !isDragging)
                {
                    useItem(index);
                }
            }
        }

        // Simple static hover label shared by all slots
        public static TextMeshProUGUI hoverText;
    }
}