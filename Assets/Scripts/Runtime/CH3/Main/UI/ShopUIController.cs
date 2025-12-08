using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Runtime.CH3.Main
{
    public class ShopUIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject shopRoot;
        [SerializeField] private ShopSelectionPanel selectionPanel;
        [SerializeField] private ShopCreatePanel createPanel;

        [Header("Behaviour")]
        [SerializeField] private bool pauseGameOnOpen = true;
        [SerializeField] private bool disablePlayerInputOnOpen = true;

        [Header("Events")]
        [SerializeField] private UnityEvent onOpened;
        [SerializeField] private UnityEvent onClosed;

        private bool isOpen;
        private float cachedTimeScale = 1f;
        private bool hasPausedTime = false;

        public bool IsOpen => isOpen;
        public ShopSelectionPanel SelectionPanel => selectionPanel;
        public ShopCreatePanel CreatePanel => createPanel;

        private void Awake()
        {
            if (shopRoot == null)
            {
                shopRoot = gameObject;
            }

            // 패널 자동 찾기 (Inspector에서 설정하지 않은 경우)
            if (selectionPanel == null)
            {
                selectionPanel = GetComponentInChildren<ShopSelectionPanel>(true);
            }
            if (createPanel == null)
            {
                createPanel = GetComponentInChildren<ShopCreatePanel>(true);
            }

            if (shopRoot != null)
            {
                isOpen = shopRoot.activeSelf;
                if (isOpen)
                {
                    CloseShopInternal();
                }
            }
        }

        public void ToggleShop()
        {
            if (isOpen)
            {
                CloseShop();
            }
            else
            {
                OpenShop();
            }
        }

        public void OpenShop()
        {
            if (isOpen)
            {
                return;
            }

            if (shopRoot != null)
            {
                shopRoot.SetActive(true);
            }

            // 패널 활성화 (두 패널 모두 표시)
            // ShowAllPanels()가 createPanel.Show()를 호출하여 아이템 로드 보장
            ShowAllPanels();

            // 버튼 이벤트 바인딩 (상점이 열릴 때마다 확실히 설정)
            SetupButtonCallbacks();

            if (pauseGameOnOpen)
            {
                cachedTimeScale = Time.timeScale;
                Time.timeScale = 0f;
                hasPausedTime = true;
            }

            if (disablePlayerInputOnOpen)
            {
                Managers.Data?.InGameKeyBinder?.PlayerInputDisable();
            }

            // EventSystem의 자동 선택 해제 (Space 키로 버튼이 클릭되는 것을 방지)
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }

            // 상점이 열릴 때 최신 상태로 업데이트
            // Show()에서 아이템이 로드되므로 여기서는 Refresh만 호출
            if (createPanel != null)
            {
                // Show()에서 이미 로드되므로 Refresh만 호출
                createPanel.RefreshSoldOutStates();
            }

            onOpened?.Invoke();
            isOpen = true;
        }

        public void CloseShop()
        {
            if (!isOpen)
            {
                return;
            }

            CloseShopInternal();
        }

        private void CloseShopInternal()
        {
            if (shopRoot != null)
            {
                shopRoot.SetActive(false);
            }

            if (pauseGameOnOpen && hasPausedTime)
            {
                Time.timeScale = cachedTimeScale;
                hasPausedTime = false;
            }

            if (disablePlayerInputOnOpen)
            {
                Managers.Data?.InGameKeyBinder?.PlayerInputEnable();
            }

            onClosed?.Invoke();
            isOpen = false;
        }

        /// <summary>
        /// 선택 패널 표시
        /// </summary>
        public void ShowSelectionPanel()
        {
            if (selectionPanel != null)
            {
                selectionPanel.Show();
            }
            if (createPanel != null)
            {
                createPanel.Hide();
            }
        }

        /// <summary>
        /// 생성 패널 표시
        /// </summary>
        public void ShowCreatePanel()
        {
            if (createPanel != null)
            {
                createPanel.Show();
            }
            if (selectionPanel != null)
            {
                selectionPanel.Hide();
            }
        }

        /// <summary>
        /// 두 패널 모두 표시 (동시에 보여야 하는 경우)
        /// </summary>
        public void ShowAllPanels()
        {
            if (selectionPanel != null)
            {
                selectionPanel.Show();
            }
            if (createPanel != null)
            {
                createPanel.Show();
            }
        }

        private void Start()
        {
            SetupButtonCallbacks();
        }

        private void SetupButtonCallbacks()
        {
            if (selectionPanel != null && createPanel != null)
            {
                // 제작 버튼: 아이템 제작만 수행 (상점 닫지 않음)
                selectionPanel.SetCreateButtonCallback(() => createPanel.TryCraftSelectedItem());
                
                // 취소 버튼: 상점 닫기
                selectionPanel.SetCancelButtonCallback(() => CloseShop());
            }
        }

        private void OnEnable()
        {
            // createPanel이 활성화되어 있을 때만 갱신
            if (createPanel != null && createPanel.gameObject.activeInHierarchy)
            {
                createPanel.RefreshSoldOutStates();
            }
        }

        private void OnDestroy()
        {
            // 씬 전환 시 Time.timeScale 복구
            if (hasPausedTime && pauseGameOnOpen)
            {
                Time.timeScale = cachedTimeScale;
            }
        }
    }
}
