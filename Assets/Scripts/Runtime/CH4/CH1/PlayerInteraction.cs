using System.Collections;
using UnityEngine;

namespace CH4.CH1
{
    // CH4.CH1의 코인, 먼지유령에 대한 인터렉션
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private ResourceController _coinController;
        private SpriteRenderer _spriteRenderer;
        private bool _godMode;

        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void CollisionWithCoin()
        {
            // 코인과 부딪힘
            _coinController.AddCoin();
        }

        public void CollisionWithGhostDust()
        {
            // 먼지와 부딪힘

            if (_godMode) return;

            _coinController.LoseCoin();
            StartCoroutine(nameof(StartGodMode));
        }

        private IEnumerator StartGodMode()
        {
            float duration = 3f;
            float blinkInterval = 0.2f;
            float elapsed = 0f;

            Color color = _spriteRenderer.color;
            bool isTransparent = false;

            _godMode = true;

            while (elapsed < duration)
            {
                isTransparent = !isTransparent;
                color.a = isTransparent ? 0.5f : 1f;
                _spriteRenderer.color = color;

                yield return new WaitForSeconds(blinkInterval);
                elapsed += blinkInterval;
            }

            color.a = 1f;
            _spriteRenderer.color = color;

            _godMode = false;
        }
    }
}