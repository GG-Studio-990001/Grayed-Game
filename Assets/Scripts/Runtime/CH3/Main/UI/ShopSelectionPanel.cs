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
        private Transform topSection;
        private Transform bottomSection;
        private Transform buttonsSection;
        private Transform selectItemImage;

        private TextMeshProUGUI itemTitle;
        private TextMeshProUGUI gridSizeText;
        private TextMeshProUGUI countText;
        private TextMeshProUGUI[] bottomItemCountTexts;
        private Image[] bottomItemImages;
        private Image selectItemImageComponent;
        public Button createButton;
        private Button cancelButton;
        
        // Color 상수 캐싱
        private static readonly Color OpaqueWhite = new Color(1, 1, 1, 1);
        private static readonly Color TransparentWhite = new Color(1, 1, 1, 0);

        private void Awake()
        {
            InitializeReferences();
        }

        private void InitializeReferences()
        {
            if (topSection == null)
                topSection = transform.Find("Top");
            if (bottomSection == null)
                bottomSection = transform.Find("Bottom");
            if (buttonsSection == null)
                buttonsSection = transform.Find("Buttons");
            if (selectItemImage == null)
                selectItemImage = transform.Find("SelectItemImage");

            // Top 섹션 요소 찾기
            if (topSection != null)
            {
                if (itemTitle == null)
                    itemTitle = topSection.Find("ItemTitle")?.GetComponent<TextMeshProUGUI>();
                
                var gridSizeBorder = topSection.Find("GridSizeBorder");
                if (gridSizeBorder != null && gridSizeText == null)
                {
                    gridSizeText = gridSizeBorder.GetChild(0).GetComponent<TextMeshProUGUI>();
                }
                
                var countBorder = topSection.Find("CountBorder");
                if (countBorder != null && countText == null)
                {
                    countText = countBorder.GetChild(0).GetComponent<TextMeshProUGUI>();
                }
            }

            // Bottom 섹션 요소 찾기 (배열이 비어있는 경우에만)
            if (bottomSection != null && (bottomItemCountTexts == null || bottomItemCountTexts.Length == 0))
            {
                var bottomTexts = new List<TextMeshProUGUI>();
                var bottomImages = new List<Image>();
                
                var consumeNames = new[] { "Consume", "Consume_1", "Consume_2", "Consume_3" };
                
                foreach (var consumeName in consumeNames)
                {
                    var consumeObj = bottomSection.Find(consumeName);
                    if (consumeObj != null)
                    {
                        // TextMeshProUGUI 찾기
                        TextMeshProUGUI textComponent = null;
                        var textObj = consumeObj.Find("Text (TMP)");
                        if (textObj != null)
                        {
                            textObj.TryGetComponent<TextMeshProUGUI>(out textComponent);
                        }
                        else
                        {
                            // "Text (TMP)"를 찾지 못했으면 "Text" 시도
                            textObj = consumeObj.Find("Text");
                            if (textObj != null)
                            {
                                textObj.TryGetComponent<TextMeshProUGUI>(out textComponent);
                            }
                        }
                        bottomTexts.Add(textComponent);
                        
                        // Image 컴포넌트 가져오기
                        consumeObj.TryGetComponent<Image>(out var image);
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
                if (createButton == null)
                    createButton = buttonsSection.Find("CreateButton")?.GetComponent<Button>();
                if (cancelButton == null)
                    cancelButton = buttonsSection.Find("CancleButton")?.GetComponent<Button>();
            }
            
            // selectItemImage의 Image 컴포넌트 캐싱
            if (selectItemImage != null && selectItemImageComponent == null)
            {
                selectItemImage.TryGetComponent<Image>(out var image);
                selectItemImageComponent = image;
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            if (itemTitle == null || gridSizeText == null || countText == null || 
                bottomItemCountTexts == null || bottomItemImages == null ||
                createButton == null || selectItemImageComponent == null)
            {
                InitializeReferences();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetSelectedItem(Sprite itemSprite)
        {
            if (selectItemImageComponent != null)
            {
                selectItemImageComponent.sprite = itemSprite;
            }
        }

        public void ClearSelection()
        {
            if (selectItemImageComponent != null)
            {
                selectItemImageComponent.sprite = null;
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
            // 초기화 확인 (한 번만)
            if (bottomItemImages == null || bottomItemImages.Length == 0)
            {
                InitializeReferences();
                if (bottomItemImages == null || bottomItemImages.Length == 0)
                {
                    Debug.LogWarning("ShopSelectionPanel: bottomItemImages 초기화 실패!");
                    return;
                }
            }
            
            // 인덱스 범위 체크
            if (index < 0 || index >= bottomItemImages.Length)
            {
                Debug.LogWarning($"ShopSelectionPanel: 인덱스 {index}가 범위를 벗어났습니다! (배열 크기: {bottomItemImages.Length})");
                return;
            }
            
            // null 체크
            if (bottomItemImages[index] == null)
            {
                Debug.LogWarning($"ShopSelectionPanel: bottomItemImages[{index}]가 null입니다!");
                return;
            }
            
            // 스프라이트 및 색상 설정 (Color 상수 사용)
            bottomItemImages[index].sprite = itemIcon;
            bottomItemImages[index].color = itemIcon != null ? OpaqueWhite : TransparentWhite;
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

        public void SetGridSizeText(string text)
        {
            if (gridSizeText != null)
            {
                gridSizeText.text = text;
            }
        }

        public void SetCountText(string text)
        {
            if (countText != null)
            {
                countText.text = text;
            }
        }
    }
}

