using Runtime.Common.View;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.InputSystem;

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
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _player.GetComponent<InteractionManager>()?.BeginHoldAtCursor(Mouse.current.position.ReadValue());
                _mouseHolding = true;
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
            _inventoryUI.SelectHotbar(index);
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
            
            // 건축 아이템인 경우 건축 모드 시작
            if (slot.item.IsBuildingItem)
            {
                if (_buildingSystem != null)
                {
                    // 이미 건축 모드 중이면 종료
                    if (_buildingSystem.IsBuildingMode)
                    {
                        _buildingSystem.EndBuildingMode();
                    }
                    else
                    {
                        _buildingSystem.StartBuildingMode(slot.item, slotIndex);
                    }
                }
            }
            else
            {
                // 일반 아이템 사용
                _inventory.TryConsumeAt(slotIndex, 1);
            }
        }

        private void OnDestroy()
        {
        }
    }
}