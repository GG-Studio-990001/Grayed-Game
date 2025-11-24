using UnityEngine;
using Runtime.Input;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 건축물 관리 UI - 건축물 상호작용 시 표시되는 관리 창
    /// </summary>
    public class BuildingManagementUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject managementPanel;
        
        private Producer _currentProducer;
        
        private void Awake()
        {
            if (managementPanel != null)
            {
                managementPanel.SetActive(false);
            }
        }
        
        /// <summary>
        /// 건축물 관리창 표시
        /// </summary>
        public void Show(Producer producer)
        {
            if (producer == null)
            {
                Debug.LogWarning("Producer가 null입니다!");
                return;
            }
            
            _currentProducer = producer;
            
            if (managementPanel != null)
            {
                managementPanel.SetActive(true);
            }
            
            // 플레이어 이동 비활성화
            if (Managers.Data != null && Managers.Data.InGameKeyBinder != null)
            {
                Managers.Data.InGameKeyBinder.PlayerInputDisable();
            }
            
            // TODO: 건축물 정보 표시 구현
            // TODO: 생산 정보 표시 구현
            // TODO: 업그레이드/관리 기능 구현
        }
        
        /// <summary>
        /// 건축물 관리창 숨김
        /// </summary>
        public void Hide()
        {
            _currentProducer = null;
            
            if (managementPanel != null)
            {
                managementPanel.SetActive(false);
            }
            
            // 플레이어 이동 활성화
            if (Managers.Data != null && Managers.Data.InGameKeyBinder != null)
            {
                Managers.Data.InGameKeyBinder.PlayerInputEnable();
            }
            
            // TODO: UI 정리 로직 구현
        }
        
        /// <summary>
        /// 현재 관리 중인 건축물 반환
        /// </summary>
        public Producer GetCurrentProducer()
        {
            return _currentProducer;
        }
        
        /// <summary>
        /// 관리창이 표시 중인지 확인
        /// </summary>
        public bool IsVisible()
        {
            return managementPanel != null && managementPanel.activeSelf;
        }
    }
}

