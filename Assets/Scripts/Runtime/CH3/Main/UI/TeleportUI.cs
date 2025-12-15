using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Runtime.InGameSystem;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 텔레포트 지역 선택 UI 컨트롤러
    /// </summary>
    public class TeleportUI : MonoBehaviour
    {
        public static TeleportUI Instance { get; private set; }

        [Header("UI Elements")]
        [SerializeField] private GameObject regionListPanel;
        [SerializeField] private Transform regionListContent;
        [SerializeField] private GameObject regionButtonPrefab;
        [SerializeField] private Button cancelButton;

        [Header("Region Names")]
        [SerializeField] private string baseCampName = "베이스캠프";
        [SerializeField] private string farmerName = "파머의 농장";
        [SerializeField] private string dollarName = "달러의 동굴";
        [SerializeField] private string michaelName = "미카엘의 지옥";
        [SerializeField] private string cancelText = "취소";

        private Teleporter currentTeleporter;
        private TeleportRegion currentRegion;
        private Dictionary<TeleportRegion, Button> regionButtonMap = new Dictionary<TeleportRegion, Button>();
        private Dictionary<TeleportRegion, string> regionNames = new Dictionary<TeleportRegion, string>();
        private bool isShowing = false;
        private bool buttonsInitialized = false;

        public Teleporter CurrentTeleporter => currentTeleporter;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializeRegionNames();
            
            if (regionListPanel != null)
            {
                regionListPanel.SetActive(false);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.AddListener(OnCancelClicked);
                TextMeshProUGUI cancelButtonText = cancelButton.GetComponentInChildren<TextMeshProUGUI>();
                if (cancelButtonText != null)
                {
                    cancelButtonText.text = cancelText;
                }
            }
        }

        private void InitializeRegionNames()
        {
            regionNames[TeleportRegion.BaseCamp] = baseCampName;
            regionNames[TeleportRegion.Farmer] = farmerName;
            regionNames[TeleportRegion.Dollar] = dollarName;
            regionNames[TeleportRegion.Michael] = michaelName;
        }

        private void Start()
        {
            InitializeAllRegionButtons();
        }

        private void OnDestroy()
        {
            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveListener(OnCancelClicked);
            }
        }

        private void InitializeAllRegionButtons()
        {
            if (buttonsInitialized) return;
            if (regionButtonPrefab == null || regionListContent == null) return;

            foreach (TeleportRegion region in System.Enum.GetValues(typeof(TeleportRegion)))
            {
                GameObject buttonObj = Instantiate(regionButtonPrefab, regionListContent);
                Button button = buttonObj.GetComponent<Button>();
                if (button == null)
                {
                    button = buttonObj.GetComponentInChildren<Button>();
                }

                if (button == null)
                {
                    Destroy(buttonObj);
                    continue;
                }

                buttonObj.SetActive(false);
                button.interactable = true;

                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = GetRegionName(region);
                    buttonText.raycastTarget = false;
                }

                Image buttonImage = buttonObj.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.raycastTarget = true;
                }

                if (button.targetGraphic == null && buttonImage != null)
                {
                    button.targetGraphic = buttonImage;
                }

                button.onClick.RemoveAllListeners();
                TeleportRegion targetRegion = region;
                button.onClick.AddListener(() => OnRegionSelected(targetRegion));

                regionButtonMap[region] = button;
            }

            buttonsInitialized = true;
        }

        public void ShowRegionList(TeleportRegion currentRegion, Teleporter teleporter)
        {
            if (TeleporterManager.Instance == null) return;

            if (isShowing && this.currentTeleporter == teleporter) return;

            this.currentTeleporter = teleporter;
            this.currentRegion = currentRegion;

            List<TeleportRegion> availableRegions = TeleporterManager.Instance.GetAvailableRegions(currentRegion);
            if (availableRegions.Count == 0) return;

            if (regionListPanel != null)
            {
                regionListPanel.SetActive(true);
            }

            UpdateRegionButtons(availableRegions);
            isShowing = true;
            AdjustListSize(availableRegions.Count);
        }

        private void UpdateRegionButtons(List<TeleportRegion> availableRegions)
        {
            if (!buttonsInitialized)
            {
                InitializeAllRegionButtons();
            }

            HashSet<TeleportRegion> availableSet = new HashSet<TeleportRegion>(availableRegions);

            foreach (var kvp in regionButtonMap)
            {
                if (kvp.Value != null)
                {
                    bool shouldBeActive = availableSet.Contains(kvp.Key);
                    kvp.Value.gameObject.SetActive(shouldBeActive);
                    if (shouldBeActive)
                    {
                        kvp.Value.interactable = true;
                    }
                }
            }
        }

        private string GetRegionName(TeleportRegion region)
        {
            return regionNames.TryGetValue(region, out string name) ? name : region.ToString();
        }

        private void AdjustListSize(int itemCount)
        {
            if (regionListContent == null) return;

            RectTransform contentRect = regionListContent.GetComponent<RectTransform>();
            if (contentRect == null) return;

            VerticalLayoutGroup layoutGroup = regionListContent.GetComponent<VerticalLayoutGroup>();
            if (layoutGroup != null) return;

            float buttonHeight = 50f;
            float spacing = 10f;
            float totalHeight = (buttonHeight + spacing) * itemCount - spacing;
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
        }

        private void OnRegionSelected(TeleportRegion targetRegion)
        {
            if (currentTeleporter == null) return;

            SetAllButtonsInteractable(false);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                currentTeleporter.TeleportToRegion(targetRegion, player);
            }

            HideRegionList();
        }

        private void SetAllButtonsInteractable(bool interactable)
        {
            foreach (var button in regionButtonMap.Values)
            {
                if (button != null)
                {
                    button.interactable = interactable;
                }
            }
        }

        private void OnCancelClicked()
        {
            HideRegionList();
        }

        public void HideRegionList()
        {
            if (regionListPanel != null)
            {
                regionListPanel.SetActive(false);
            }
            isShowing = false;
            currentTeleporter = null;
        }

        public bool IsShowing => isShowing;
    }
}

