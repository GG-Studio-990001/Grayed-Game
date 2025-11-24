using Runtime.Common.View;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.InputSystem;
using Runtime.Input;

namespace Runtime.CH3.Main
{
    public class CH3KeyBinder : MonoBehaviour
    {
        [SerializeField] private SettingsUIView _settingsUIView;
        [SerializeField] private LineView _luckyDialogue;
        [SerializeField] private PlayerController _player;
        [SerializeField] private CH3Dialogue _ch3Dialogue;
        [Header("Inventory References")]
        [SerializeField] private InventoryUI _inventoryUI;
        [SerializeField] private Inventory _inventory;
        [SerializeField] private BuildingSystem _buildingSystem;

        private void Start()
        {
            // BuildingSystem 참조 자동 찾기 (안정성 향상)
            if (_buildingSystem == null)
            {
                _buildingSystem = BuildingSystem.Instance;
            }
            
            InitKeyBinding();
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();

            Managers.Data.InGameKeyBinder.CH3PlayerKeyBinding(_player, this);
            
            if (_ch3Dialogue != null)
            {
                Managers.Data.InGameKeyBinder.CH3UIKeyBinding(_settingsUIView, _ch3Dialogue);
            }
            else
            {
                Managers.Data.InGameKeyBinder.CH3UIKeyBinding(_settingsUIView, _luckyDialogue);
            }

            _settingsUIView.OnSettingsOpen += () => Managers.Data.InGameKeyBinder.PlayerInputDisable();
            _settingsUIView.OnSettingsClose += () => Managers.Data.InGameKeyBinder.PlayerInputEnable();
        }

        // UI 클릭 기반 홀드 지원 (마우스 왼쪽 버튼)
        private bool _mouseHolding;
        private void Update()
        {
            if (Mouse.current == null) return;
            
            // 건축 모드 중이면 상호작용 처리하지 않음 (BuildingSystem에서 처리)
            if (_buildingSystem != null && _buildingSystem.IsBuildingMode)
            {
                return;
            }
            
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                var interactionManager = _player.GetComponent<InteractionManager>();
                if (interactionManager != null)
                {
                    Vector2 mousePos = Mouse.current.position.ReadValue();
                    
                    // 길게 누르기 시도
                    bool hadHoldBefore = interactionManager.HasCurrentHold();
                    interactionManager.BeginHoldAtCursor(mousePos);
                    
                    // 길게 누르기가 시작되지 않았으면 일반 상호작용 시도
                    if (!hadHoldBefore && !interactionManager.HasCurrentHold())
                    {
                        interactionManager.TryInteractAtCursor(mousePos);
                    }
                    
                    _mouseHolding = true;
                }
            }
            if (_mouseHolding)
            {
                _player.GetComponent<InteractionManager>()?.UpdateHold();
            }
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                _player.GetComponent<InteractionManager>()?.CancelHold();
                _mouseHolding = false;
            }
        }

        // 세부 동작은 여기서 정의
        public void HotbarSelect(int index)
        {
            // UI 선택 업데이트 (빌드모드 처리는 SelectHotbar에서 호출됨)
            _inventoryUI.SelectHotbar(index);
        }
        
        /// <summary>
        /// 핫바 선택 후 빌드모드 처리 (InventoryUI에서 호출)
        /// </summary>
        public void OnHotbarSelected(int index)
        {
            // 선택된 슬롯의 아이템 확인
            var slot = _inventory.GetSlot(index);
            
            if (slot != null && slot.item != null)
            {
                // 건축 아이템이면 빌드모드 자동 진입
                if (slot.item.IsBuildingItem)
                {
                    if (_buildingSystem != null && !_buildingSystem.IsBuildingMode)
                    {
                        _buildingSystem.StartBuildingMode(slot.item, index);
                    }
                }
                else
                {
                    // 다른 아이템 선택 시 빌드모드 취소
                    if (_buildingSystem != null && _buildingSystem.IsBuildingMode)
                    {
                        _buildingSystem.EndBuildingMode();
                    }
                }
            }
            else
            {
                // 빈 슬롯 선택 시 빌드모드 취소
                if (_buildingSystem != null && _buildingSystem.IsBuildingMode)
                {
                    _buildingSystem.EndBuildingMode();
                }
            }
        }

        public void InventoryToggle()
        {
            _inventoryUI.ToggleInventory();
        }

        public void HotbarUse()
        {
            int idx = _inventoryUI.GetSelectedHotbarIndex();
            UseItem(idx);
        }
        
        /// <summary>
        /// 특정 슬롯의 아이템 사용
        /// </summary>
        public void UseItem(int slotIndex)
        {
            var slot = _inventory.GetSlot(slotIndex);
            
            if (slot == null || slot.item == null)
            {
                return;
            }
            
            // 인벤토리가 열려있으면 닫기
            if (_inventoryUI != null && _inventoryUI.IsInventoryOpen())
            {
                _inventoryUI.ToggleInventory();
            }
            
            // 건축 아이템은 핫바 선택 시 자동으로 빌드모드 진입하므로 E 사용 시 처리하지 않음
            if (slot.item.IsBuildingItem)
            {
                return;
            }
            
            // 일반 아이템 사용
            _inventory.TryConsumeAt(slotIndex, 1);
        }

        private void OnDestroy()
        {
        }
    }
}