using TMPro;
using UnityEngine;

namespace CH4.CH1
{
    public class ResourceController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _coinUi;
        [SerializeField] private TextMeshProUGUI _jewelryUi;
        [SerializeField] private TextMeshProUGUI _fishUi;
        [SerializeField] private int _coinCnt = 0;
        [SerializeField] private int _jewelryCnt = 0;
        [SerializeField] private int _fishCnt = 0;
        private readonly int _loseCoinCnt = 20;

        private void Start()
        {
            // _coinCnt = 0;
            UpdateUi();
        }

        public void AddCoin()
        {
            _coinCnt++;
            UpdateUi();
        }

        public void LoseCoin()
        {
            _coinCnt = Mathf.Clamp(_coinCnt - _loseCoinCnt, 0, int.MaxValue);
            UpdateUi();
        }

        public void GetJewelry(int cnt)
        {
            _jewelryCnt += cnt;
        }

        public void GetFist(int cnt)
        {
            _fishCnt += cnt;
        }

        private void UpdateUi()
        {
            _coinUi.text = _coinCnt.ToString();
            _jewelryUi.text = _jewelryCnt.ToString();
            _fishUi.text = _fishCnt.ToString();
        }
    }
}