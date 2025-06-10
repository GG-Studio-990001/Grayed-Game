using UnityEngine;

namespace Runtime.CH3.Dancepace
{
    public class PreviewNPC : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private Sprite upSprite;
        [SerializeField] private Sprite downSprite;
        [SerializeField] private Sprite leftSprite;
        [SerializeField] private Sprite rightSprite;
        [SerializeField] private Sprite idleSprite;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void PlayPreviewPose(string poseId)
        {
            switch (poseId)
            {
                case "Up": spriteRenderer.sprite = upSprite; break;
                case "Down": spriteRenderer.sprite = downSprite; break;
                case "Left": spriteRenderer.sprite = leftSprite; break;
                case "Right": spriteRenderer.sprite = rightSprite; break;
                default: spriteRenderer.sprite = idleSprite; break;
            }
        }

        public void SetIdle()
        {
            spriteRenderer.sprite = idleSprite;
        }
    }
} 