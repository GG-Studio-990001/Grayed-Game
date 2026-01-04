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
    }
}