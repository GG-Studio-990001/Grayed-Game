using Runtime.Common.View;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.InputSystem;
using Runtime.Input;
using Runtime.CH3.Main;

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
            
            // ShopUIController 참조 초기화
            if (_shopUIController == null)
            {
                _shopUIController = FindObjectOfType<ShopUIController>();
            }
            
            InitKeyBinding();
            SetupDialogueHotbarControl();
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();

            Managers.Data.InGameKeyBinder.CH3PlayerKeyBinding(_player, this);
            
            // ESC 키 처리: 인벤토리가 열려있으면 닫기, 아니면 세팅창 토글
            // CH3UIKeyBinding에서 기본 GameSetting 바인딩을 제거하고 커스텀 처리
            if (_ch3Dialogue != null)
            {
                SetupCustomUIKeyBinding(_ch3Dialogue);
            }
            else
            {
                SetupCustomUIKeyBinding(_luckyDialogue);
            }

            _settingsUIView.OnSettingsOpen += () => Managers.Data.InGameKeyBinder.PlayerInputDisable();
            _settingsUIView.OnSettingsClose += () => Managers.Data.InGameKeyBinder.PlayerInputEnable();
        }

        private void SetupCustomUIKeyBinding(object dialogue)
        {
            var gameOverControls = Managers.Data.InGameKeyBinder.GetGameOverControls();
            if (gameOverControls == null) return;

            gameOverControls.UI.Enable();
            
            // ESC 키: 인벤토리 우선, 없으면 세팅창
            gameOverControls.UI.GameSetting.performed += _ => HandleEscapeKey();
            
            // 대화 입력
            if (dialogue is CH3Dialogue ch3Dialogue)
            {
                gameOverControls.UI.DialogueInput.performed += _ => ch3Dialogue.OnDialogueInput();
            }
            else if (dialogue is LineView lineView)
            {
                gameOverControls.UI.DialogueInput.performed += _ => lineView.OnContinueClicked();
            }
        }

        private void SetupDialogueHotbarControl()
        {
            // CH3Dialogue에서 DialogueRunner 찾기
            DialogueRunner runner = null;
            if (_ch3Dialogue != null)
            {
                runner = _ch3Dialogue.GetComponent<DialogueRunner>();
            }
            
            // 없으면 씬에서 찾기
            if (runner == null)
            {
                runner = FindObjectOfType<DialogueRunner>();
            }
            
            if (runner == null) return;
            
            // 대화 시작 시 핫바 끄기 및 툴팁 숨기기
            runner.onDialogueStart.AddListener(() =>
            {
                if (_inventoryUI != null && _inventoryUI.GetHotbarRootTransform() != null)
                {
                    _inventoryUI.GetHotbarRootTransform().gameObject.SetActive(false);
                }
                InventoryTooltip.Hide();
            });
            
            // 대화 완료 시 핫바 켜기 및 툴팁 숨기기
            runner.onDialogueComplete.AddListener(() =>
            {
                if (_inventoryUI != null && _inventoryUI.GetHotbarRootTransform() != null)
                {
                    _inventoryUI.GetHotbarRootTransform().gameObject.SetActive(true);
                }
                InventoryTooltip.Hide();
            });
        }

        private void HandleEscapeKey()
        {
            // 인벤토리가 열려있으면 닫기
            if (_inventoryUI != null && _inventoryUI.IsInventoryOpen())
            {
                _inventoryUI.ToggleInventory();
                return;
            }
            
            // 세팅창이 열려있으면 닫기 (GameSettingToggle에서 처리)
            // 아무것도 안 열려있으면 세팅창 열기
            _settingsUIView.GameSettingToggle();
        }

        // UI 클릭 기반 홀드 지원 (마우스 왼쪽 버튼)
        private bool _mouseHolding;
        private ShopUIController _shopUIController;
        
        private void Update()
        {
            if (Mouse.current == null) return;
            
            // 상점창이 열려있으면 상호작용 무시
            if (_shopUIController != null && _shopUIController.IsOpen)
            {
                // 상점창이 열려있으면 홀드 상태도 초기화
                if (_mouseHolding)
                {
                    _player.GetComponent<InteractionManager>()?.CancelHold();
                    _mouseHolding = false;
                }
                return;
            }
            
            // 건축 모드 중이면 상호작용 처리하지 않음 (BuildingSystem에서 처리)
            // 빌드모드에서는 좌클릭이 건설로 사용되므로 여기서 가로채지 않음
            if (_buildingSystem != null && _buildingSystem.IsBuildingMode)
            {
                // 빌드모드에서는 홀드 상태도 초기화
                if (_mouseHolding)
                {
                    _player.GetComponent<InteractionManager>()?.CancelHold();
                    _mouseHolding = false;
                }
                return;
            }
            
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                // UI 위에 마우스가 있으면 상호작용 무시 (UI 버튼 클릭 우선)
                if (UnityEngine.EventSystems.EventSystem.current != null && 
                    UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                var interactionManager = _player.GetComponent<InteractionManager>();
                if (interactionManager != null)
                {
                    Vector2 mousePos = Mouse.current.position.ReadValue();
                    
                    if (TeleportUI.Instance != null && TeleportUI.Instance.IsShowing)
                    {
                        return;
                    }
                    
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
            // 인벤토리가 열려있으면 핫바 선택 무시
            if (_inventoryUI != null && _inventoryUI.IsInventoryOpen())
            {
                return;
            }
            
            // 건축 모드일 때 핫바 클릭 무시
            if (_buildingSystem != null && _buildingSystem.IsBuildingMode)
            {
                return;
            }
            
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
                    if (_buildingSystem != null)
                    {
                        // 이미 빌드모드이고 다른 아이템이면 업데이트, 아니면 새로 시작
                        if (_buildingSystem.IsBuildingMode)
                        {
                            // 다른 건축 아이템이면 빌드모드 업데이트
                            if (_buildingSystem.GetCurrentBuildingItem() != slot.item)
                            {
                                _buildingSystem.StartBuildingMode(slot.item, index);
                            }
                        }
                        else
                        {
                            _buildingSystem.StartBuildingMode(slot.item, index);
                        }
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