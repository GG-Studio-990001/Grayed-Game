using UnityEngine;
using DG.Tweening;

namespace Runtime.CH3.Dancepace
{
    public class HeartEffect : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float floatDistance = 2f;
        [SerializeField] private float floatDuration = 1.5f;
        [SerializeField] private float fadeOutDuration = 1f;
        [SerializeField] private float randomOffset = 0.5f;

        private SpriteRenderer spriteRenderer;
        private Sequence animationSequence;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
        }

        public void PlayEffect()
        {
            // 이전 애니메이션 중지
            animationSequence?.Kill();

            // 초기 상태 설정
            transform.localPosition = Vector3.zero;
            Color startColor = spriteRenderer.color;
            startColor.a = 1f;
            spriteRenderer.color = startColor;

            // 랜덤한 방향으로 약간 이동
            Vector3 randomDirection = new Vector3(
                Random.Range(-randomOffset, randomOffset),
                Random.Range(-randomOffset, randomOffset),
                0f
            );

            // 애니메이션 시퀀스 생성
            animationSequence = DOTween.Sequence()
                .Append(transform.DOLocalMoveY(floatDistance, floatDuration)
                    .SetEase(Ease.OutQuad))
                .Join(transform.DOLocalMoveX(randomDirection.x, floatDuration)
                    .SetEase(Ease.OutQuad))
                .Join(spriteRenderer.DOFade(0f, fadeOutDuration)
                    .SetEase(Ease.InQuad))
                .OnComplete(() => Destroy(gameObject));
        }

        private void OnDisable()
        {
            animationSequence?.Kill();
        }
    }
} 