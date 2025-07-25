using DG.Tweening;
using Runtime.ETC;
using System.Collections;
using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class EnterPipe : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float _animationDuration = 0.5f;
        [SerializeField] private float _offsetY = 1.0f;
        [SerializeField] private bool _isEndPipe = false;

        public bool _canEnter;
        private Vector3 _initialPosition;
        private Vector3 _targetPosition;

        private void Awake()
        {
            _initialPosition = transform.position;
            _targetPosition = transform.position =
                new Vector3(_initialPosition.x, _initialPosition.y - _offsetY, _initialPosition.z);
            transform.position = _targetPosition;
        }

        private void OnEnable()
        {
            // 애니메이션 시작
            StartEnterAnimation();
        }

        private void OnDisable()
        {
            DOTween.Kill(transform);
            transform.position = _targetPosition;
        }

        private void StartEnterAnimation()
        {
            if (!_isEndPipe)
            {
                transform.DOMoveY(_initialPosition.y, _animationDuration)
                    .SetEase(Ease.OutBounce); // 애니메이션 이징
            }
            else
            {
                transform.DOMoveY(_initialPosition.y + 0.25f, _animationDuration)
                .SetEase(Ease.OutBounce); // 애니메이션 이징
            }
        }

        private IEnumerator EnterAnimCoroutine(GameObject ario)
        {
            Managers.Sound.Play(Sound.SFX, "SuperArio/CH2_SUB_SFX_14");
            ArioManager.Instance.StopGame();
            ario.transform.DOMove(ario.transform.position = transform.position + Vector3.up, 0f);
            ario.transform.DOMove(transform.position, 0.5f);
            yield return new WaitForSeconds(1f);
            ario.SetActive(false);
            ario.SetActive(true);
            ario.GetComponent<Ario>().CancelInvincibleTime();
            ArioManager.Instance.EnterStore();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Ario ario))
            {
                if (_canEnter)
                {
                    StartCoroutine(EnterAnimCoroutine(ario.gameObject));
                }
            }
        }
    }
}