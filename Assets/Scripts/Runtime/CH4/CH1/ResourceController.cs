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
        [SerializeField] private TextMeshProUGUI _ChococatUi;
        [SerializeField] private TextMeshProUGUI _JellycatUi;
        [SerializeField] private TextMeshProUGUI _MellowCatUi;
        [SerializeField] private TextMeshProUGUI _CandyPopUi;
        [SerializeField] private TextMeshProUGUI _StickCandyUi;
        [SerializeField] private GameObject[] _Chococats;
        [SerializeField] private GameObject[] _Jellycats;
        [SerializeField] private GameObject[] _MellowCats;
        [SerializeField] private GameObject[] _CandyPops;
        [SerializeField] private GameObject[] _StickCandys;
        public int Chococat = 0;
        public int Jellycat = 0;
        public int MellowCat = 0;
        public int CandyPop = 0;
        public int StickCandy = 0;
        [SerializeField] private GameObject[] keyAndPanel;
        private readonly int _loseCoinCnt = 20;

        private void Start()
        {
            // _coinCnt = 0;
            UpdateUi();
        }

        public void PurchaseKey()
        {
            if (Fish < 7) return;
            Fish -= 7;
            UpdateUi();
            foreach (GameObject obj in keyAndPanel)
                obj.SetActive(false);
        }

        public void RefreshExchangeUi()
        {
            _ChococatUi.text = Chococat.ToString() + "/3";
            _JellycatUi.text = Jellycat.ToString() + "/3";
            _MellowCatUi.text = MellowCat.ToString() + "/3";
            _CandyPopUi.text = CandyPop.ToString() + "/3";
            _StickCandyUi.text = StickCandy.ToString() + "/3";

            for (int i = 0; i < 3; i++)
            {
                _Chococats[i].SetActive(Chococat >= i + 1);
                _Jellycats[i].SetActive(Jellycat >= i + 1);
                _MellowCats[i].SetActive(MellowCat >= i + 1);
                _CandyPops[i].SetActive(CandyPop >= i + 1);
                _StickCandys[i].SetActive(StickCandy >= i + 1);
            }
        }

        public void ExchangeChococat()
        {
            if (Chococat < 3) return;

            Chococat -= 3;
            Jewelry += 5;
            UpdateUi();
        }

        public void ExchangeJellyCat()
        {
            if (Jellycat < 3) return;

            Jellycat -= 3;
            Jewelry += 2;
            UpdateUi();
        }

        public void ExchangeMellowCat()
        {
            if (MellowCat < 3) return;

            MellowCat -= 3;
            Coin += 100;
            UpdateUi();
        }

        public void ExchangeCandyPop()
        {
            if (CandyPop < 3) return;

            CandyPop -= 3;
            Coin += 40;
            UpdateUi();
        }

        public void ExchangeStickCandy()
        {
            if (StickCandy < 3) return;

            StickCandy -= 3;
            Coin += 50;
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
            RefreshExchangeUi();
        }
    }
}