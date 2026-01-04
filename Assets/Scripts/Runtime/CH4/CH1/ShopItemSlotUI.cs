using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace CH4.CH1
{
    [RequireComponent(typeof(Button))]
    public class ShopItemSlotUI : MonoBehaviour
    {
        [SerializeField] private ResourceController _resourceController;
        [SerializeField] private ShopRandomPicker _shopRandomPicker;

        [SerializeField] private GameObject _recommendImage;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Image _currencyIcon;
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private GameObject _completeImg;
        [SerializeField] private Slider _loadingSlider;

        private ShopItem _currentItem;
        private Button _button;
        private bool _canBuy;

        private int _slotIndex;
        private bool _isRefreshing;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(Purchase);
        }

        public void SetSlotIndex(int index)
        {
            _slotIndex = index;
        }

        private void Purchase()
        {
            if (!_canBuy) return;

            bool success = _currentItem.costType == CostType.Coin
                ? _resourceController.UseCoin(_currentItem.cost)
                : _resourceController.UseJewerly(_currentItem.cost);

            if (success)
                PurchaseComplete();
        }

        private void PurchaseComplete()
        {
            _completeImg.SetActive(true);
            _canBuy = false;
            _button.interactable = false;

            switch (_currentItem.name)
            {
                case "물고기 젤리": _resourceController.Fish++; break;
                case "초코캣": _resourceController.Chococat++; break;
                case "젤리캣": _resourceController.Jellycat++; break;
                case "멜로캣": _resourceController.MellowCat++; break;
                case "캔디팝": _resourceController.CandyPop++; break;
                case "스틱캔디": _resourceController.StickCandy++; break;
            }

            _resourceController.UpdateUi();
            LoadNewItem();
        }

        private void LoadNewItem()
        {
            _isRefreshing = true;
            float loadTime = 1f;
            _loadingSlider.value = 1f;

            _loadingSlider
                .DOValue(0f, loadTime)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (_isRefreshing)
                        _shopRandomPicker.RefreshSingleItem(_slotIndex);
                });
        }

        public void Bind(ShopItem item, Sprite icon, Sprite currency)
        {
            // Init
            _completeImg.SetActive(false);
            _canBuy = true;
            _button.interactable = true;
            _isRefreshing = false;

            // Bind
            _currentItem = item;

            _recommendImage.SetActive(item.id.Contains("Fish"));
            _iconImage.sprite = icon;
            _nameText.text = item.name;

            _currencyIcon.sprite = currency;
            _costText.text = item.cost.ToString();
        }

        public ShopItem GetCurrentItem()
        {
            return _currentItem;
        }
    }
}
