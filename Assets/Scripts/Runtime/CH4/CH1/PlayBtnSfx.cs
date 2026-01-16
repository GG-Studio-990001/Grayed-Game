using UnityEngine;
using UnityEngine.UI;
using Runtime.ETC;

namespace Runtime.CH4.CH1
{
    [RequireComponent(typeof(Button))]
    public class PlayBtnSfx : MonoBehaviour
    {
        public enum ButtonType
        {
            JellyStoreItem,
            Exchange,
            PurchaseKey,
            Normal,
            Exit,
        }

        [SerializeField] private ButtonType _sfxType;
        private Button _button;

        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(PlayClickSound);
        }

        private void PlayClickSound()
        {
            switch (_sfxType)
            {
                case ButtonType.JellyStoreItem:
                    Managers.Sound.Play(Sound.SFX, "CH4/CH4_SFX_04_Buy");
                    break;
                case ButtonType.Exchange:
                    Managers.Sound.Play(Sound.SFX, "CH1/Puzzle_Puff_SFX_23");
                    break;
                case ButtonType.PurchaseKey:
                    Managers.Sound.Play(Sound.SFX, "CH1/GetItem_SFX");
                    break;
                case ButtonType.Normal:
                    Managers.Sound.Play(Sound.SFX, "Setting/SFX_Setting_UI_Basic_Click");
                    break;
                case ButtonType.Exit:
                    Managers.Sound.Play(Sound.SFX, "Setting/SFX_Setting_UI_Back_Click");
                    break;
            }
        }
    }
}
