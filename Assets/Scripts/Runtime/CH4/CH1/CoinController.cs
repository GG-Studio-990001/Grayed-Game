using TMPro;
using UnityEngine;

namespace CH4.CH1
{
    // 라플리에 부착(했지만 옮기는 게 낫겠지)
    public class CoinController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _coinUi;
        [SerializeField] private int _coinCnt;

        private void Start()
        {
            _coinCnt = 0;
            UpdateCoinUi();
        }

        public void AddCoin()
        {
            _coinCnt++;
            UpdateCoinUi();
        }

        private void UpdateCoinUi()
        {
            _coinUi.text = _coinCnt.ToString();
        }
    }
}