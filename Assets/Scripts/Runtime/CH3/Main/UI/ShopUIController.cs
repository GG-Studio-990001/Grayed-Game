using UnityEngine;
using UnityEngine.Events;

namespace Runtime.CH3.Main
{
    public class ShopUIController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject shopRoot;

        [Header("Behaviour")]
        [SerializeField] private bool pauseGameOnOpen = true;
        [SerializeField] private bool disablePlayerInputOnOpen = true;

        [Header("Events")]
        [SerializeField] private UnityEvent onOpened;
        [SerializeField] private UnityEvent onClosed;

        private bool isOpen;
        private float cachedTimeScale = 1f;

        private void Awake()
        {
            if (shopRoot == null)
            {
                shopRoot = gameObject;
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

            if (pauseGameOnOpen)
            {
                cachedTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }

            if (disablePlayerInputOnOpen)
            {
                Managers.Data?.InGameKeyBinder?.PlayerInputDisable();
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

            if (pauseGameOnOpen)
            {
                Time.timeScale = cachedTimeScale;
            }

            if (disablePlayerInputOnOpen)
            {
                Managers.Data?.InGameKeyBinder?.PlayerInputEnable();
            }

            onClosed?.Invoke();
            isOpen = false;
        }
    }
}
