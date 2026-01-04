using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CH4.CH1
{
    [RequireComponent(typeof(Button))]
    public class ShopItemSlotUI : MonoBehaviour
    {
        [SerializeField] ResourceController resourceController;
        [SerializeField] private GameObject recommendImage; // 추천 뱃지
        [SerializeField] private Image iconImage;           // 아이템 아이콘
        [SerializeField] private TextMeshProUGUI nameText;  // 이름 텍스트
        [SerializeField] private Image currencyIcon;        // 재화 아이콘
        [SerializeField] private TextMeshProUGUI costText;  // 재화 개수 텍스트
        [SerializeField] private GameObject completeImg; // 추천 뱃지
        private ShopItem currentItem;
        private Button _button;
        private bool canbuy;

        private void Awake()
        {
            _button = GetComponent<Button>();
            if (_button != null)
            {
                _button.onClick.AddListener(Purchase);
            }
        }

        private void Purchase()
        {
            if (!canbuy) return;

            if (currentItem.costType == CostType.Coin)
            {
                if (resourceController.UseCoin(currentItem.cost))
                {
                    PurchaseComplete();
                }
            }
            else
            {
                if (resourceController.UseJewerly(currentItem.cost))
                {
                    PurchaseComplete();
                }
            }
        }

        private void PurchaseComplete()
        {
            completeImg.SetActive(true);
            canbuy = false;
            _button.interactable = false;

            switch (currentItem.name)
            {
                case "물고기 젤리":
                    resourceController.Fish++;
                    break;
                case "초코캣":
                    resourceController.Chococat++;
                    break;
                case "젤리캣":
                    resourceController.Jellycat++;
                    break;
                case "멜로캣":
                    resourceController.MellowCat++;
                    break;
                case "캔디팝":
                    resourceController.CandyPop++;
                    break;
                case "스틱캔디":
                    resourceController.StickCandy++;
                    break;
            }
            resourceController.UpdateUi();
            Debug.Log("구매 완료");
        }

        public void Bind(ShopItem item, Sprite icon, Sprite currency)
        {
            completeImg.SetActive(false);
            canbuy = true;
            _button.interactable = true;

            currentItem = item;
            recommendImage.SetActive(item.id.Contains("Fish"));

            iconImage.sprite = icon;
            nameText.text = item.name.ToString();

            currencyIcon.sprite = currency;
            costText.text = item.cost.ToString();

            // TODO: Check Is currency enough
        }
    }
}