using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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

        public void PlayPose(string poseId)
        {
            switch (poseId)
            {
                case "Up":
                    SetSprite(_upSprite);
                    break;
                case "Down":
                    SetSprite(_downSprite);
                    break;
                case "Left":
                    SetSprite(_leftSprite);
                    break;
                case "Right":
                    SetSprite(_rightSprite);
                    break;
                default:
                    SetIdleSprite();
                    break;
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
                SetSprite(isRight ? _rightSprite : _leftSprite);
            }
            else
            {
                // 상하 방향
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