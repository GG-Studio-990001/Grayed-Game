using Runtime.CH1.Main.Player;
using UnityEngine;

namespace Runtime.CH1.Main.Dialogue
{
    public class PackController : MonoBehaviour
    {
        [SerializeField] private TopDownPlayer _player;
        [SerializeField] private SpriteRenderer _pack;
        [SerializeField] private Sprite[] _packSprites = new Sprite[4];

        public void GetPack(int idx)
        {
            // 0 lucky / 1 slg / 2 translation / 3 visualNovel
            _player.OnGet();
            _pack.sprite = _packSprites[idx];
            _pack.gameObject.SetActive(true);
        }

        public void FinishPack()
        {
            _pack.gameObject.SetActive(false);
            _player.Idle();
        }
    }
}