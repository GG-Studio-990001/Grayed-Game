using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.CH2.SuperArio
{
    public class Ario : MonoBehaviour
    {
        [SerializeField] private float _jumpHeight;
        [SerializeField] private float _jumpSpeed;
        [SerializeField] private Sprite sitSprite;

        private bool _isJump;
        private bool _isTop;
        private bool _isPause;

        private Vector2 _startPos;
        private CapsuleCollider2D _col;
        private Animator _animator;
        private SpriteRenderer _spr;
        private Sprite _initSprite;
        private int _life = 1;

        private void Start()
        {
            _spr = GetComponent<SpriteRenderer>();
            _col = GetComponent<CapsuleCollider2D>();
            _animator = GetComponent<Animator>();
            _initSprite = _spr.sprite;
            _startPos = transform.position;
            ArioManager.instance.onPlay += InitData;
        }

        private void FixedUpdate()
        {
            if (!ArioManager.instance.isPlay) return;

            if (_isJump)
            {
                if (transform.position.y <= _jumpHeight - 0.1f && !_isTop)
                {
                    transform.position = Vector2.Lerp(transform.position,
                        new Vector2(transform.position.x, _jumpHeight), _jumpSpeed * Time.fixedDeltaTime);
                }
                else
                {
                    _isTop = true;
                }

                if (transform.position.y > _startPos.y && _isTop)
                {
                    transform.position = Vector2.MoveTowards(transform.position, _startPos, _jumpSpeed * Time.fixedDeltaTime);
                }
            }

            // 땅에 닿았을 때 점프 관련 상태만 초기화
            if (transform.position.y <= _startPos.y)
            {
                _isJump = false;
                _isTop = false;
                transform.position = _startPos;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!ArioManager.instance.isPlay || _isPause) return;

            Vector2 moveInput = context.ReadValue<Vector2>();

            if (context.performed)
            {
                if (moveInput.y > 0 && transform.position.y <= _startPos.y && !_isJump) // 위쪽 (점프)
                {
                    _isJump = true;
                }
                else if (moveInput.y < 0 && !_isJump) // 아래쪽
                {
                    if (_col.offset.y == 0)
                        _col.offset = new Vector2(0, -0.1f);
                    _animator.enabled = false;
                    _spr.sprite = sitSprite;
                }
            }
            else if (context.canceled) // 아래 방향키를 뗐을 때
            {
                _animator.enabled = true;
                _spr.sprite = _initSprite;
                if (_col.offset.y != 0)
                    _col.offset = new Vector2(0, 0);
            }
        }

        private void InitData(bool isPlay)
        {
            if (isPlay)
            {
                transform.position = _startPos;
                _isJump = false;
                _isTop = false;
                _animator.enabled = true;
            }
            else
            {
                _life = 1;
                _animator.enabled = false;
                _isJump = false;
            }
        }

        public void PauseKeyInput()
        {
            _isPause = !_isPause;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Obstacle") && ArioManager.instance.isPlay)
            {
                _life--;
                ArioManager.instance.ChangeHeartUI(_life);
            }
            else if (other.CompareTag("Coin") && ArioManager.instance.isPlay)
            {
                ArioManager.instance.GetCoin();
                other.gameObject.SetActive(false);
            }
        }
    }
}