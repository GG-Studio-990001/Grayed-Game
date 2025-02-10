using DG.Tweening;
using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class EnterPipe : MonoBehaviour
    {
        [Header("Animation Settings")] [SerializeField]
        private float _animationDuration = 0.5f;

        [SerializeField] private float _offsetY = 1.0f;

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
            transform.DOMoveY(_initialPosition.y, _animationDuration)
                .SetEase(Ease.OutBounce); // 애니메이션 이징
        }
    }
}