using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Runtime.ETC;

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

        [SerializeField] private GameObject[] spotlightObject;

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
            // InGameKeyBinder의 Player 입력 상태 확인
            if (!Managers.Data.InGameKeyBinder.IsPlayerInputEnabled()) 
            {
                Debug.Log("DPRapley: PlayerInputDisabled");
                return;
            }
            
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

            Vector2 newDirection = Vector2.zero;
            if (absX > absY)
            {
                // 좌우 방향
                bool isRight = _currentDirection.x > 0;
                SetSprite(isRight ? _rightSprite : _leftSprite);
                newDirection = isRight ? Vector2.right : Vector2.left;
            }
            else
            {
                // 상하 방향
                SetSprite(_currentDirection.y > 0 ? _upSprite : _downSprite);
                newDirection = _currentDirection.y > 0 ? Vector2.up : Vector2.down;
            }

            // 매번 사운드 재생
            PlayDirectionSound(newDirection);
        }

        private void PlayDirectionSound(Vector2 direction)
        {
            if (direction == Vector2.up)
            {
                Managers.Sound.Play(Sound.SFX, "Dancepace/New/CH3_SUB_BGM_WAVE_SFX_C_W");
            }
            else if (direction == Vector2.down)
            {
                Managers.Sound.Play(Sound.SFX, "Dancepace/New/CH3_SUB_BGM_WAVE_SFX_F_S");
            }
            else if (direction == Vector2.left)
            {
                Managers.Sound.Play(Sound.SFX, "Dancepace/New/CH3_SUB_BGM_WAVE_SFX_E_A");
            }
            else if (direction == Vector2.right)
            {
                Managers.Sound.Play(Sound.SFX, "Dancepace/New/CH3_SUB_BGM_WAVE_SFX_G_D");
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
            }
        }

        public void ResetState()
        {
            SetIdleSprite();
            _currentDirection = Vector2.zero;
        }



        public void StartSpotlightSequence(float totalTime)
        {
            if (spotlightObject == null || spotlightObject.Length == 0) return;
            StopAllCoroutines();
            StartCoroutine(SpotlightSequenceCoroutine(totalTime));
        }

        private IEnumerator SpotlightSequenceCoroutine(float totalTime)
        {
            int count = spotlightObject.Length;
            float interval = totalTime / count;

            // 모두 끄기
            foreach (var obj in spotlightObject)
                if (obj != null) obj.SetActive(false);

            for (int i = 0; i < count; i++)
            {
                if (spotlightObject[i] != null)
                    spotlightObject[i].SetActive(true);

                if (i < count - 1)
                    yield return new WaitForSeconds(interval);
            }
        }

        public void HideSpotlight()
        {
            foreach (var obj in spotlightObject)
                if (obj != null) obj.SetActive(false);
        }
    }
} 