using UnityEngine;
using Runtime.ETC;
using Runtime.CH3.Dancepace;

namespace Runtime.CH3.Dancepace
{
    public class AnswerNPC : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private Sprite upSprite;
        [SerializeField] private Sprite downSprite;
        [SerializeField] private Sprite leftSprite;
        [SerializeField] private Sprite rightSprite;
        private Sprite idleSprite;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            idleSprite = spriteRenderer.sprite;
        }

        public void PlayAnswerPose(EPoseType poseId)
        {
            switch (poseId)
            {
                case EPoseType.Up:spriteRenderer.sprite = upSprite; break;
                case EPoseType.Down: spriteRenderer.sprite = downSprite; break;
                case EPoseType.Left: spriteRenderer.sprite = leftSprite; break;
                case EPoseType.Right: spriteRenderer.sprite = rightSprite; break;
                default: spriteRenderer.sprite = idleSprite; break;
            }
        }

        public void SetIdle()
        {
            spriteRenderer.sprite = idleSprite;
        }
    }
} 