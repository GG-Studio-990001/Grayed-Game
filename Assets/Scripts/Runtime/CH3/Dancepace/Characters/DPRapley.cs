using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.CH3.Dancepace
{
    public class DPRapley : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private Sprite _upSprite;
        [SerializeField] private Sprite _downSprite;
        [SerializeField] private Sprite _leftSprite;
        [SerializeField] private Sprite _rightSprite;
        [SerializeField] private Sprite _idleSprite;

        private SpriteRenderer _spriteRenderer;
        private Vector2 _currentDirection = Vector2.zero;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null)
            {
                _spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
        }

        private void Start()
        {
            SetIdleSprite();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 inputDirection = context.ReadValue<Vector2>();
            
            if (context.performed)
            {
                _currentDirection = inputDirection;
                UpdateSprite();
            }
            else if (context.canceled)
            {
                _currentDirection = Vector2.zero;
                SetIdleSprite();
            }
        }

        private void UpdateSprite()
        {
            if (_currentDirection == Vector2.zero)
            {
                SetIdleSprite();
                return;
            }

            // 가장 큰 값을 가진 방향을 선택
            float absX = Mathf.Abs(_currentDirection.x);
            float absY = Mathf.Abs(_currentDirection.y);

            if (absX > absY)
            {
                // 좌우 방향
                bool isRight = _currentDirection.x > 0;
                _spriteRenderer.flipX = !isRight;
                SetSprite(isRight ? _rightSprite : _leftSprite);
            }
            else
            {
                // 상하 방향
                _spriteRenderer.flipX = false;
                SetSprite(_currentDirection.y > 0 ? _upSprite : _downSprite);
            }
        }

        private void SetSprite(Sprite sprite)
        {
            if (sprite != null)
            {
                _spriteRenderer.sprite = sprite;
            }
        }

        private void SetIdleSprite()
        {
            if (_idleSprite != null)
            {
                _spriteRenderer.sprite = _idleSprite;
                _spriteRenderer.flipX = false;
            }
        }

        public void ResetState()
        {
            SetIdleSprite();
            _currentDirection = Vector2.zero;
        }
    }
} 