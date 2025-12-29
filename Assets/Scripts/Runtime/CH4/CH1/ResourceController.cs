using TMPro;
using UnityEngine;

namespace CH4.CH1
{
    public class ResourceController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _coinUi;
        [SerializeField] private TextMeshProUGUI _jewelryUi;
        [SerializeField] private TextMeshProUGUI _fishUi;
        public int Coin = 0;
        public int Jewelry = 0;
        public int Fish = 0;
        public int Chococat = 0;
        public int Jellycat = 0;
        public int MellowCat = 0;
        public int CandyPop = 0;
        public int StickCandy = 0;
        private readonly int _loseCoinCnt = 20;

        private void Start()
        {
            // _coinCnt = 0;
            UpdateUi();
        }

        public void AddCoin()
        {
            Coin++;
            UpdateUi();
        }

        public void LoseCoin()
        {
            Coin = Mathf.Clamp(Coin - _loseCoinCnt, 0, int.MaxValue);
            UpdateUi();
        }

        public bool UseCoin(int cost)
        {
            if (Coin >= cost)
            {
                Coin -= cost;
                UpdateUi();
                return true;
            }
            return false;
        }
        public bool UseJewerly(int cost)
        {
            if (Jewelry >= cost)
            {
                Jewelry -= cost;
                UpdateUi();
                return true;
            }
            return false;
        }

        public void GetJewelry(int cnt)
        {
            Jewelry += cnt;
        }

        public void GetFist(int cnt)
        {
            Fish += cnt;
        }

        private void UpdateUi()
        {
            _coinUi.text = Coin.ToString();
            _jewelryUi.text = Jewelry.ToString();
            _fishUi.text = Fish.ToString();
        }
    }
}