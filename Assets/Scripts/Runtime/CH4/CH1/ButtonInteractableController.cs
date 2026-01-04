using System;
using UnityEngine;
using UnityEngine.UI;

namespace CH4.CH1
{
    // 재화 관련하여 패널이 켜질 때 버튼 활성화/비활성화 결정
    public class ButtonInteractableController : MonoBehaviour
    {
        [NonSerialized] public ResourceController ResourceController;
        [SerializeField] private Button _fishBtn;
        [SerializeField] private Button[] _exchangeBtns;

        public void CheckPurchaseKeyBtn()
        {
            _fishBtn.interactable = ResourceController.Fish >= 7;
        }

        public void CheckExchangeBtns()
        {
            _exchangeBtns[0].interactable = ResourceController.Chococat >= 3;
            _exchangeBtns[1].interactable = ResourceController.Jellycat >= 3;
            _exchangeBtns[2].interactable = ResourceController.MellowCat >= 3;
            _exchangeBtns[3].interactable = ResourceController.CandyPop >= 3;
            _exchangeBtns[4].interactable = ResourceController.StickCandy >= 3;
        }
    }
}