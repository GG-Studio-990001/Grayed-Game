using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 상점 선택 패널 관리 - 아이템 선택 UI
    /// </summary>
    public class ShopSelectionPanel : MonoBehaviour
    {
        [Header("UI References (Auto-found)")]
        private GameObject topSection; // Top GameObject
        private GameObject bottomSection; // Bottom GameObject
        private GameObject buttonsSection; // Buttons GameObject
        private GameObject selectItemImage; // SelectItemImage GameObject

        private TextMeshProUGUI itemTitle; // Top > ItemTitle
        private GameObject gridSizeBorder; // Top > GridSizeBorder
        private GameObject countBorder; // Top > CountBorder

        private TextMeshProUGUI[] bottomItemCountTexts; // Bottom > BottomItemCountIText들 (4개)
        private Image[] bottomItemImages; // Bottom > 각 재료 슬롯의 Image들 (4개)

        public Button createButton; // Buttons > CreateButton
        private Button cancelButton; // Buttons > CancleButton

        private void Awake()
        {
            InitializeReferences();
        }

        private void InitializeReferences()
        {
            // 메인 섹션 찾기
            topSection = transform.Find("Top")?.gameObject;
            bottomSection = transform.Find("Bottom")?.gameObject;
            buttonsSection = transform.Find("Buttons")?.gameObject;
            selectItemImage = transform.Find("SelectItemImage")?.gameObject;

            // Top 섹션 요소 찾기
            if (topSection != null)
            {
                itemTitle = topSection.transform.Find("ItemTitle")?.GetComponent<TextMeshProUGUI>();
                gridSizeBorder = topSection.transform.Find("GridSizeBorder")?.gameObject;
                countBorder = topSection.transform.Find("CountBorder")?.gameObject;
            }

            // Bottom 섹션 요소 찾기 (4개)
            if (bottomSection != null)
            {
                var bottomTexts = new List<TextMeshProUGUI>();
                var bottomImages = new List<Image>();
                
                var consumeNames = new[] { "Consume", "Consume_1", "Consume_2", "Consume_3" };
                
                foreach (var consumeName in consumeNames)
                {
                    var consumeObj = bottomSection.transform.Find(consumeName);
                    if (consumeObj != null)
                    {
                        var textObj = consumeObj.Find("Text (TMP)");
                        if (textObj == null)
                        {
                            textObj = consumeObj.Find("Text");
                        }
                        
                        if (textObj != null)
                        {
                            bottomTexts.Add(textObj.GetComponent<TextMeshProUGUI>());
                        }
                        else
                        {
                            bottomTexts.Add(null);
                        }
                        
                        var image = consumeObj.GetComponent<Image>();
                        bottomImages.Add(image);
                    }
                    else
                    {
                        bottomTexts.Add(null);
                        bottomImages.Add(null);
                    }
                }
                
                bottomItemCountTexts = bottomTexts.ToArray();
                bottomItemImages = bottomImages.ToArray();
            }

            // Buttons 찾기
            if (buttonsSection != null)
            {
                createButton = buttonsSection.transform.Find("CreateButton")?.GetComponent<Button>();
                cancelButton = buttonsSection.transform.Find("CancleButton")?.GetComponent<Button>();
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            InitializeReferences();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetSelectedItem(Sprite itemSprite)
        {
            if (selectItemImage != null && selectItemImage.TryGetComponent<Image>(out var image))
            {
                image.sprite = itemSprite;
            }
        }

        public void ClearSelection()
        {
            if (selectItemImage != null && selectItemImage.TryGetComponent<Image>(out var image))
            {
                image.sprite = null;
            }
        }

        public void SetItemTitle(string title)
        {
            if (itemTitle != null)
            {
                itemTitle.text = title;
            }
        }

        public void SetBottomItemCount(int index, string countText)
        {
            if (bottomItemCountTexts != null && index >= 0 && index < bottomItemCountTexts.Length && bottomItemCountTexts[index] != null)
            {
                bottomItemCountTexts[index].text = countText;
            }
        }

        public void SetBottomItemImage(int index, Sprite itemIcon)
        {
            if (bottomItemImages == null || bottomItemImages.Length == 0)
            {
                InitializeReferences();
                if (bottomItemImages == null || bottomItemImages.Length == 0)
                {
                    Debug.LogWarning("ShopSelectionPanel: bottomItemImages 초기화 실패!");
                    return;
                }
            }
            
            if (index < 0 || index >= bottomItemImages.Length)
            {
                Debug.LogWarning($"ShopSelectionPanel: 인덱스 {index}가 범위를 벗어났습니다! (배열 크기: {bottomItemImages.Length})");
                return;
            }
            
            if (bottomItemImages[index] == null)
            {
                Debug.LogWarning($"ShopSelectionPanel: bottomItemImages[{index}]가 null입니다! 재초기화 시도...");
                InitializeReferences();
                if (bottomItemImages == null || index >= bottomItemImages.Length || bottomItemImages[index] == null)
                {
                    return;
                }
            }
            
            if (itemIcon != null)
            {
                bottomItemImages[index].sprite = itemIcon;
                bottomItemImages[index].color = new Color(1, 1, 1, 1);
            }
            else
            {
                bottomItemImages[index].sprite = null;
                bottomItemImages[index].color = new Color(1, 1, 1, 0);
            }
        }

        public void SetCreateButtonCallback(System.Action callback)
        {
            if (createButton != null)
            {
                createButton.onClick.RemoveAllListeners();
                if (callback != null)
                {
                    createButton.onClick.AddListener(() => callback());
                }
            }
        }

        public void SetCancelButtonCallback(System.Action callback)
        {
            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveAllListeners();
                if (callback != null)
                {
                    cancelButton.onClick.AddListener(() => callback());
                }
            }
        }
    }
}

