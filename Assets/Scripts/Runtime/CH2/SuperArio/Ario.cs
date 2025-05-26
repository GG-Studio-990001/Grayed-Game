using Runtime.ETC;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using DG.Tweening;

namespace Runtime.CH2.SuperArio
{
    public class Ario : MonoBehaviour
    {
        [SerializeField] private float _jumpHeight;
        [SerializeField] private float _jumpSpeed;
        [SerializeField] private Sprite sitSprite;
        [SerializeField] private Sprite hitSprite;
        [SerializeField] private GameObject _pipe;
        public int life;

        private bool _isJump;
        private bool _isTop;

        private Vector2 _startPos;
        private CapsuleCollider2D _col;
        private Animator _animator;
        private SpriteRenderer _spr;
        private Sprite _initSprite;

        private bool _isInvincible = false; // 무적 상태 여부
        private float _invincibleDuration = 1.0f; // 무적 지속 시간
        private float _jumpBufferTimeRemaining = 0f; // 남은 점프 대기 시간
        private Color _originalColor; // 원래 색상 저장

        // 점프 버퍼 관련 변수
        private readonly float _blinkInterval = 0.1f; // 깜빡이는 간격
        private readonly float _jumpBufferTime = 0.2f; // 점프 입력 대기 시간

        private void Start()
        {
            _spr = GetComponent<SpriteRenderer>();
            _col = GetComponent<CapsuleCollider2D>();
            _animator = GetComponent<Animator>();
            _initSprite = _spr.sprite;
            _startPos = transform.position;
            _originalColor = _spr.color;
            ArioManager.Instance.OnPlay += InitData;
            ArioManager.Instance.OnEnterStore += DisablePipe;
        }

        private void OnDestroy()
        {
            ArioManager.Instance.OnPlay -= InitData;
            ArioManager.Instance.OnEnterStore -= DisablePipe;
        }

        private void FixedUpdate()
        {
            if (!ArioManager.Instance.IsPlay) return;

            // 점프 대기 시간 동안 점프를 실행할지 여부 결정
            if (_jumpBufferTimeRemaining > 0)
            {
                _jumpBufferTimeRemaining -= Time.fixedDeltaTime;
            }

            // 점프 처리
            TryJump();

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
                    transform.position =
                        Vector2.MoveTowards(transform.position, _startPos, _jumpSpeed * Time.fixedDeltaTime);
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

        public void DisablePipe(bool isDisable)
        {
            _pipe.SetActive(false);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!ArioManager.Instance.IsPlay || ArioManager.Instance.IsPause)
                return;

            Vector2 moveInput = context.ReadValue<Vector2>();

            if (context.performed)
            {
                // 점프 중일 때 아래 방향키를 눌렀을 때 앉기 상태로 전환
                if (moveInput.y < 0) // 아래쪽
                {
                    Managers.Sound.Play(Sound.SFX, "SuperArio/CH2_SUB_SFX_33");
                    if (_col.offset.y == 0)
                        _col.offset = new Vector2(0, -0.1f);
                    // 점프 중에도 앉기 상태로 변경
                    _animator.enabled = false;
                    _spr.sprite = sitSprite;
                }
                else if (moveInput.y > 0 && transform.position.y <= _startPos.y && !_isJump) // 위쪽 (점프)
                {
                    Managers.Sound.Play(Sound.SFX, "SuperArio/CH2_SUB_SFX_02");
                    _jumpBufferTimeRemaining = _jumpBufferTime; // 점프 입력을 버퍼에 저장
                }
            }
            else if (context.canceled) // 아래 방향키를 뗐을 때
            {
                _animator.enabled = true;
                _spr.sprite = _initSprite;

                // 아래 방향키를 뗀 후, 앉기 상태를 원래 상태로 되돌림
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
                _animator.enabled = true;
                _pipe.SetActive(false);
                gameObject.SetActive(true);
            }
            else
            {
                _animator.enabled = false;
                if (ArioManager.Instance.IsGameOver)
                    _pipe.SetActive(true);
            }
        }

        private IEnumerator UseItemCoroutine()
        {
            _invincibleDuration = 20f;
            _isInvincible = true;
            _originalColor = _spr.color;

            float elapsedTime = 0f;
            while (elapsedTime < _invincibleDuration - 3f) // 3초 전까지 무지개 색상 효과
            {
                float hue = (elapsedTime % 2f) / 2f;
                _spr.color = Color.HSVToRGB(hue, 1f, 1f);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 마지막 3초 동안 깜빡이는 효과
            float warningTime = 3f;
            float warningElapsedTime = 0f;
            while (warningElapsedTime < warningTime)
            {
                _spr.enabled = !_spr.enabled;
                warningElapsedTime += _blinkInterval;
                yield return new WaitForSeconds(_blinkInterval);
            }

            Managers.Sound.Play(Sound.BGM, "SuperArio/CH2_SUB_BGM_01");
            _spr.enabled = true;
            _spr.color = _originalColor;
            _isInvincible = false;
        }

        public void UseInvincibleItem()
        {
            if (_isInvincible || !ArioManager.Instance.HasItem || !ArioManager.Instance.IsPlay)
                return;

            ArioManager.Instance.UseItem();
            StartCoroutine(UseItemCoroutine());
        }

        public void EnterStoreAnimation()
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMoveY(transform.position.y - 2f, 1f).SetEase(Ease.Linear));
            sequence.AppendCallback(() =>
            {
                transform.position = _startPos;
            });
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
            if (other.CompareTag(GlobalConst.ObstacleStr) && ArioManager.Instance.IsPlay && !_isInvincible)
            {
                life--;
                _spr.sprite = hitSprite;
                ArioManager.Instance.ChangeHeartUI(life);
                StartCoroutine(InvincibilityCoroutine());
            }
            else if (other.CompareTag(GlobalConst.CoinStr) && ArioManager.Instance.IsPlay)
            {
                ArioManager.Instance.GetCoin();
                other.gameObject.SetActive(false);
                Managers.Sound.Play(Sound.SFX, "SuperArio/CH2_SUB_SFX_31");
            }
        }

        // 점프 버퍼를 처리하는 함수
        private void TryJump()
        {
            if (_jumpBufferTimeRemaining > 0 && transform.position.y <= _startPos.y)
            {
                _isJump = true;
                _jumpBufferTimeRemaining = 0f; // 점프 실행 후 버퍼를 리셋
            }
        }

        public void CancelInvincibleTime()
        {
            _spr.enabled = true;
            _spr.color = _originalColor;
            _isInvincible = false;
        }

        public IEnumerator RewardEnterAnimation(Transform door, bool istop = false)
        {
            // 바닥까지 이동
            yield return new WaitForSeconds(0.85f);
            if (istop)
                Managers.Sound.Play(Sound.SFX, "SuperArio/Ending/CH2_SUB_SFX_18");
            yield return new WaitForSeconds(0.15f);
            float duration = Vector2.Distance(transform.position, _startPos) / 2f;
            yield return transform.DOMove(_startPos, duration).SetEase(Ease.Linear).WaitForCompletion();
            yield return new WaitForSeconds(1f);

            // 입구까지 이동
            Managers.Sound.Play(Sound.SFX, "SuperArio/Opening/CH2_SUB_SFX_9_2");
            yield return transform.DOMoveX(door.position.x, 1.5f).SetEase(Ease.Linear).WaitForCompletion();
            yield return new WaitForSeconds(1f);
            gameObject.SetActive(false);
        }
    }
}