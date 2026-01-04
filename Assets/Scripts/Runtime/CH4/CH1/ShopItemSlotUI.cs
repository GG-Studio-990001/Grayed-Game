using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CH4.CH1
{
    // JellyStore 각 슬롯에 연결
    [RequireComponent(typeof(Button))]
    public class ShopItemSlotUI : MonoBehaviour
    {
        [SerializeField] ResourceController _resourceController;
        [SerializeField] private GameObject _recommendImage; // 추천 뱃지
        [SerializeField] private Image _iconImage;           // 아이템 아이콘
        [SerializeField] private TextMeshProUGUI _nameText;  // 이름 텍스트
        [SerializeField] private Image _currencyIcon;        // 재화 아이콘
        [SerializeField] private TextMeshProUGUI _costText;  // 재화 개수 텍스트
        [SerializeField] private GameObject _completeImg; // 추천 뱃지
        private ShopItem _currentItem;
        private Button _button;
        private bool _canBuy;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Purchase);
        }

        private void Purchase()
        {
            if (!_canBuy) return;

            if (_currentItem.costType == CostType.Coin)
            {
                if (_resourceController.UseCoin(_currentItem.cost))
                {
                    PurchaseComplete();
                }
            }
            else
            {
                if (_resourceController.UseJewerly(_currentItem.cost))
                {
                    PurchaseComplete();
                }
            }
        }

        private void PurchaseComplete()
        {
            _completeImg.SetActive(true);
            _canBuy = false;
            _button.interactable = false;

            switch (_currentItem.name)
            {
                case "물고기 젤리":
                    _resourceController.Fish++;
                    break;
                case "초코캣":
                    _resourceController.Chococat++;
                    break;
                case "젤리캣":
                    _resourceController.Jellycat++;
                    break;
                case "멜로캣":
                    _resourceController.MellowCat++;
                    break;
                case "캔디팝":
                    _resourceController.CandyPop++;
                    break;
                case "스틱캔디":
                    _resourceController.StickCandy++;
                    break;
            }
            _resourceController.UpdateUi();
        }

        public void Bind(ShopItem item, Sprite icon, Sprite currency)
        {
            // Init
            _completeImg.SetActive(false);
            _canBuy = true;
            _button.interactable = true;

            // Bind
            _currentItem = item;
            _recommendImage.SetActive(item.id.Contains("Fish"));

            _iconImage.sprite = icon;
            _nameText.text = item.name.ToString();

            _currencyIcon.sprite = currency;
            _costText.text = item.cost.ToString();
        }
    }
}