using SLGDefines;
using UnityEngine;
using System.Collections;

namespace Runtime.CH1.SubB.SLG
{
    public class NewArrow : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        [SerializeField] private Transform[] _buildings;
        [SerializeField] private Rigidbody2D _rb; // Rigidbody2D (화살표 이동을 부드럽게 처리)
        [SerializeField] private SpriteRenderer _sr;
        [SerializeField] private float _arrowDistance = 1.15f;
        [SerializeField] private float _rotationSmoothTime = 0.05f; // 회전 보간 속도

        private Transform _target;
        private float _currentVelocity; // SmoothDampAngle 속도 저장용
        private Coroutine _arrowRoutine; // 코루틴 핸들러

        private void FixedUpdate()
        {
            ShowArrowTo();
        }

        public void SetTarget(SLGBuildingType type)
        {
            if (type == SLGBuildingType.Bridge)
                _target = _buildings[0];
            else if (type == SLGBuildingType.MamagoCompany)
                _target = _buildings[1];
            else if (type == SLGBuildingType.DollarStatue)
                _target = _buildings[2];
            else
            {
                Debug.Log("Cannot Set Target");
                return;
            }

            // 새로운 목표 설정 시 즉시 위치 & 회전값 반영
            UpdateArrowRotationInstantly();

            // 기존 코루틴이 실행 중이면 중지하고 새로운 코루틴 시작
            if (_arrowRoutine != null)
                StopCoroutine(_arrowRoutine);
            _arrowRoutine = StartCoroutine(ArrowActivationRoutine());
        }

        private IEnumerator ArrowActivationRoutine()
        {
            yield return StartCoroutine(FadeArrow(0f, 1f, 0.4f));

            yield return new WaitForSeconds(1.2f);

            yield return StartCoroutine(FadeArrow(1f, 0f, 0.4f));
        }

        private IEnumerator FadeArrow(float startAlpha, float targetAlpha, float duration)
        {
            float elapsedTime = 0f;
            Color color = _sr.color;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
                _sr.color = color;
                yield return null;
            }

            color.a = targetAlpha;
            _sr.color = color;
        }

        private void ShowArrowTo()
        {
            if (_target == null) return;

            Vector3 direction = (_target.position - _player.position).normalized;
            Vector3 desiredPosition = _player.position + direction * _arrowDistance;

            // 위치 이동 부드럽게 보간
            _rb.MovePosition(desiredPosition);

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float currentAngle = _rb.rotation;
            float smoothedAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref _currentVelocity, _rotationSmoothTime);

            // 회전 부드럽게 보간
            _rb.MoveRotation(smoothedAngle);
        }

        private void UpdateArrowRotationInstantly()
        {
            if (_target == null) return;

            Vector3 direction = (_target.position - _player.position).normalized;
            Vector3 newPosition = _player.position + direction * _arrowDistance;

            // 목표 변경 시 즉시 위치 반영
            _rb.position = newPosition;

            // 목표 변경 시 즉시 회전값 반영
            float newAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _rb.rotation = newAngle;
        }
    }
}