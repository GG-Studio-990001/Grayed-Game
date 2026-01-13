using System;
using UnityEngine;

namespace CH4.CH1
{
    // 재화 관련하여 패널이 켜질 때 버튼 활성화/비활성화 결정
    public class ButtonInteractableController : MonoBehaviour
    {
        [NonSerialized] public ResourceController ResourceController;
        [SerializeField] private ButtonInteractableVisual _fishBtn;
        [SerializeField] private ButtonInteractableVisual[] _exchangeBtns;
        [SerializeField] private ButtonInteractableVisual[] _purchaseBtns;
        [SerializeField] private ButtonInteractableVisual _refreshBtn;
        private ShopItemSlotUI[] _itemSlots;
        private readonly int itemCnt = 4;

        private void Awake()
        {
            _itemSlots = new ShopItemSlotUI[itemCnt];

            for (int i = 0; i < itemCnt; i++)
            {
                _itemSlots[i] = _purchaseBtns[i].GetComponent<ShopItemSlotUI>();
            }
        }

        public void CheckPurchaseKeyBtn()
        {
            _fishBtn.SetBtnAlpha(ResourceController.Fish >= 7);
        }

        public void CheckExchangeBtns()
        {
            _exchangeBtns[0].SetBtnAlpha(ResourceController.Chococat >= 3);
            _exchangeBtns[1].SetBtnAlpha(ResourceController.Jellycat >= 3);
            _exchangeBtns[2].SetBtnAlpha(ResourceController.MellowCat >= 3);
            _exchangeBtns[3].SetBtnAlpha(ResourceController.CandyPop >= 3);
            _exchangeBtns[4].SetBtnAlpha(ResourceController.StickCandy >= 3);
        }

        public void CheckPurchaseBtns()
        {
            for (int i = 0; i < itemCnt; i++)
            {
                _purchaseBtns[i].SetBtnAlpha(_itemSlots[i].CheckCanPurchase(), 0.85f);
            }
        }

        public void CheckRefreshBtn()
        {
            _refreshBtn.SetBtnAlpha(ResourceController.Coin >= 3, 0.85f);
        }
    }
}