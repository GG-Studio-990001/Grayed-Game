using TMPro;
using UnityEngine;

namespace CH4.CH1
{
    public class CoinController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _coinUi;
        [SerializeField] private int _coinCnt;
        private readonly int _loseCoinCnt = 20;

        private void Start()
        {
            _coinCnt = 0;
            UpdateCoinUi();
        }

        public void AddCoin()
        {
            // TODO: 코인 획득 효과음 추가
            _coinCnt++;
            UpdateCoinUi();
        }

        public void LoseCoin()
        {
            _coinCnt = Mathf.Clamp(_coinCnt - _loseCoinCnt, 0, int.MaxValue);
            UpdateCoinUi();
        }

        private void UpdateCoinUi()
        {
            _coinUi.text = _coinCnt.ToString();
        }
    }
}