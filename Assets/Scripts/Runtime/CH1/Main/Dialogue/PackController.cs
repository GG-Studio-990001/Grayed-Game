using Runtime.CH1.Main.Player;
using UnityEngine;

namespace Runtime.CH1.Main.Dialogue
{
    public class PackController : MonoBehaviour
    {
        // 기계어 번역팩 Only
        [SerializeField] private TopDownPlayer _player;
        [SerializeField] private SpriteRenderer _pack;

        public void GetPack()
        {
            _player.OnGet();
            _pack.gameObject.SetActive(true);
            Managers.Sound.Play(ETC.Sound.SFX, "CH1/GetItem_SFX");
        }

        public void FinishPack()
        {
            _pack.gameObject.SetActive(false);
            _player.Idle();
        }
    }
}