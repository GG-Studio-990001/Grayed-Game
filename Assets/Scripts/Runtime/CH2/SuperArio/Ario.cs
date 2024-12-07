using Runtime.ETC;
using System;
using System.Collections;
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

        private Vector2 _startPos;
        private CapsuleCollider2D _col;
        private Animator _animator;
        private SpriteRenderer _spr;
        private Sprite _initSprite;
        private GameObject _pipe;
        
        private int _life = 1;
        private bool _isInvincible = false; // 무적 상태 여부
        private float _invincibleDuration = 1.0f; // 무적 지속 시간
        private float _blinkInterval = 0.1f; // 깜빡이는 간격
        private Color _originalColor; // 원래 색상 저장
        
        private void Start()
        {
            _pipe = transform.GetChild(0).gameObject;
            _spr = GetComponent<SpriteRenderer>();
            _col = GetComponent<CapsuleCollider2D>();
            _animator = GetComponent<Animator>();
            _initSprite = _spr.sprite;
            _startPos = transform.position;
            ArioManager.instance.OnPlay += InitData;
        }

        private void OnDestroy()
        {
            ArioManager.instance.OnPlay -= InitData;
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
            if (!ArioManager.instance.isPlay || ArioManager.instance.isPause)
                return;

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
            transform.position = _startPos;
            _isJump = false;
            _isTop = false;
            if (isPlay)
            {
                _life = 1;
                _animator.enabled = true;
                _pipe.SetActive(false);
            }
            else
            {
                _animator.enabled = false;
                _pipe.SetActive(true);
            }
        }
        
        private IEnumerator UseItemCoroutine()
        {
            _invincibleDuration = 20f;
            _isInvincible = true;
            _originalColor = _spr.color;

            float elapsedTime = 0f;

            while (elapsedTime < _invincibleDuration)
            {
                // 무지개 색상을 계산
                float hue = (elapsedTime % 1f) / 1f;
                _spr.color = Color.HSVToRGB(hue, 1f, 1f);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 무적 상태 종료 후 원래 색상 복구
            _spr.color = _originalColor;
            _isInvincible = false;
        }
        
        public void UseInvincibleItem()
        {
            if (_isInvincible || !ArioManager.instance.GetItem || !ArioManager.instance.isPlay)
                return;
            
            ArioManager.instance.ChangeItemSprite();
            StartCoroutine(UseItemCoroutine());
        }

        private IEnumerator InvincibilityCoroutine()
        {
            _invincibleDuration = 1f;
            _isInvincible = true; // 무적 상태 시작

            float elapsedTime = 0f;

            while (elapsedTime < _invincibleDuration)
            {
                // 스프라이트 깜빡임 (비활성화 / 활성화)
                _spr.enabled = !_spr.enabled;

                elapsedTime += _blinkInterval;
                yield return new WaitForSeconds(_blinkInterval);
            }

            _spr.enabled = true;
            _isInvincible = false;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(GlobalConst.ObstacleStr) && ArioManager.instance.isPlay && !_isInvincible)
            {
                _life--;
                ArioManager.instance.ChangeHeartUI(_life);
                StartCoroutine(InvincibilityCoroutine());
            }
            else if (other.CompareTag(GlobalConst.CoinStr) && ArioManager.instance.isPlay)
            {
                ArioManager.instance.GetCoin();
                other.gameObject.SetActive(false);
            }
        }
    }
}