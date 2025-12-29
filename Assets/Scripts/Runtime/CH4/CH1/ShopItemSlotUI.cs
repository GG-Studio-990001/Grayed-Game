using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CH4.CH1
{
    public class ShopItemSlotUI : MonoBehaviour
    {
        [SerializeField] private GameObject recommendImage; // 추천 뱃지
        [SerializeField] private Image iconImage;           // 아이템 아이콘
        [SerializeField] private TextMeshProUGUI nameText;  // 이름 텍스트
        [SerializeField] private Image currencyIcon;        // 재화 아이콘
        [SerializeField] private TextMeshProUGUI costText;  // 재화 개수 텍스트

        public void Bind(ShopItem item, Sprite icon, Sprite currency)
        {
            recommendImage.SetActive(item.id.Contains("Fish"));

            iconImage.sprite = icon;
            nameText.text = item.name.ToString();

            currencyIcon.sprite = currency;
            costText.text = item.cost.ToString();
        }
    }
}