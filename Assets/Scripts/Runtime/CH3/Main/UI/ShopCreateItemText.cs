using UnityEngine;
using UnityEngine.UI;
using Runtime.ETC;

namespace Runtime.CH3.Main
{
    public class ShopCreateItemText : MonoBehaviour
    {
        private Transform backgroundTransform;
        private Transform soldOutTransform;
        private TMPro.TextMeshProUGUI itemNameText;
        private Button button;
        private Image image;
        private CH3_LevelData levelData;

        public Button Button => button;
        public CH3_LevelData LevelData => levelData;

        private void Awake()
        {
            InitializeReferences();
            SetupButton();
            DisableChildRaycastTargets(gameObject);
        }

        private void InitializeReferences()
        {
            backgroundTransform = CH3Utils.FindChildByNameIgnoreCase(transform, "BackGround");
            
            var itemNameTextTransform = CH3Utils.FindChildByNameIgnoreCase(transform, "ItemNameText");
            if (itemNameTextTransform == null)
            {
                itemNameTextTransform = CH3Utils.FindChildByNameIgnoreCase(transform, "Text (TMP)");
            }
            itemNameText = itemNameTextTransform?.GetComponent<TMPro.TextMeshProUGUI>();

            soldOutTransform = CH3Utils.FindChildByNameIgnoreCase(transform, "SoldOut");
        }

        private void SetupButton()
        {
            image = Utils.GetOrAddComponent<Image>(gameObject);
            image.color = new Color(1, 1, 1, 0);
            image.raycastTarget = true;

            button = Utils.GetOrAddComponent<Button>(gameObject);
            button.targetGraphic = image;
            button.interactable = true;
        }

        private void DisableChildRaycastTargets(GameObject parent)
        {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                var child = parent.transform.GetChild(i);
                
                var childImage = child.GetComponent<Image>();
                if (childImage != null)
                {
                    childImage.raycastTarget = false;
                }

                var textMesh = child.GetComponent<TMPro.TextMeshProUGUI>();
                if (textMesh != null)
                {
                    textMesh.raycastTarget = false;
                }

                DisableChildRaycastTargets(child.gameObject);
            }
        }

        public void SetSelected(bool selected)
        {
            if (backgroundTransform != null)
            {
                backgroundTransform.gameObject.SetActive(selected);
            }
        }

        public void SetItemName(string name)
        {
            if (itemNameText != null)
            {
                itemNameText.text = name ?? string.Empty;
            }
        }

        public void SetSoldOut(bool soldOut)
        {
            if (soldOutTransform != null)
            {
                soldOutTransform.gameObject.SetActive(soldOut);
            }
            // 버튼은 항상 활성화 (매진되어도 선택 및 재료 확인은 가능)
            // 제작 버튼은 ShopCreatePanel.UpdateCreateButtonState()에서 별도로 제어
            if (button != null)
            {
                button.interactable = true;
            }
        }

        public void SetLevelData(CH3_LevelData data)
        {
            levelData = data;
        }
    }
}

