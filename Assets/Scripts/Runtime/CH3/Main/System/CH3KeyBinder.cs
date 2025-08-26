using Runtime.CH2.Main;
using Runtime.Common.View;
using UnityEngine;
using Yarn.Unity;
using Runtime.CH3.Main;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Runtime.CH3.Main
{
    public class CH3KeyBinder : MonoBehaviour
    {
        [SerializeField] private SettingsUIView _settingsUIView;
        [SerializeField] private LineView _luckyDialogue;
        [SerializeField] private QuaterViewPlayer _player;
        [Header("Inventory References")]
        [SerializeField] private InventoryUI _inventoryUI;
        [SerializeField] private Inventory _inventory;
        // removed local actions, using official InGameKeyBinder instead

        private void Start()
        {
            InitKeyBinding();
        }

        private void InitKeyBinding()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();

            Managers.Data.InGameKeyBinder.CH3PlayerKeyBinding(_player, this);
            Managers.Data.InGameKeyBinder.CH3UIKeyBinding(_settingsUIView, _luckyDialogue);
            // 인벤토리/단축바 바인딩은 CH3PlayerKeyBinding 내에서 처리됨

            _settingsUIView.OnSettingsOpen += () => Managers.Data.InGameKeyBinder.PlayerInputDisable();
            _settingsUIView.OnSettingsClose += () => Managers.Data.InGameKeyBinder.PlayerInputEnable();

            // 위에서 바인딩됨
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
            _inventory.TryConsumeAt(idx, 1);
        }

        private void OnDestroy()
        {
            // nothing
        }
    }
}