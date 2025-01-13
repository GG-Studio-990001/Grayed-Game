using System;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CH2.Tcg
{
    public class TcgCard : MonoBehaviour
    {
        public int Index;
        [SerializeField] private CardManager _cardManager;
        [SerializeField] private Image _cardImg;
        private bool _isBack;

        public void SetCardIndex(int index)
        {
            Index = index;
            if (_cardImg == null || index < 0 || index > 6)
            {
                if (_cardImg == null)
                    Debug.LogWarning("Invalid Card Img");
                else
                    Debug.LogWarning("Invalid Card Idx");
                return;
            }
            SetCardFront();
        }

        public void SetCardBack()
        {
            _cardImg.sprite = _cardManager.CardBackSpr;
        }

        public void SetCardFront()
        {
            _cardImg.sprite = _cardManager.CardSprs[Index];
        }
    }
}