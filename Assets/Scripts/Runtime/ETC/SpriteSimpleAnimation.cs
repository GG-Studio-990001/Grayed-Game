using UnityEngine;

namespace Runtime.ETC
{
    // ImageAnimation의 SpriteRenderer 버전
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteSimpleAnimation : MonoBehaviour
    {
        private SpriteRenderer _sprite;
        [SerializeField] private Sprite[] _animSprites;
        [SerializeField] private float _animTime = 0.25f;
        private int _animFrame = -1;

        public void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            InvokeRepeating(nameof(NextSprite), _animTime, _animTime);
        }

        public void NextSprite()
        {
            if (_animSprites.Length != 0)
            {
                _animFrame = ++_animFrame % _animSprites.Length;
                _sprite.sprite = _animSprites[_animFrame];
            }
        }
    }
}