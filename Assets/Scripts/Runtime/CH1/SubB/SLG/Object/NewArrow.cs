using SLGDefines;
using UnityEngine;
using System.Collections;

namespace Runtime.CH1.SubB.SLG
{
    public class NewArrow : MonoBehaviour
    {
        [SerializeField] private Transform _player;
        [SerializeField] private Transform[] _buildings;
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private SpriteRenderer _sr;
        [SerializeField] private float _arrowDistance = 1.15f;
        [SerializeField] private float _rotationSmoothTime = 0.05f;
        [SerializeField] private GameObject[] _map = new GameObject[3];

        private Transform _target;
        private float _arrowTime = 2f;
        private float _fadeTime = 0.4f;
        private float _currentVelocity;
        private Coroutine _arrowRoutine;
        private bool _isRotating = false;

        private void FixedUpdate()
        {
            if (!_isRotating)
                ShowArrowTo();
        }

        public void SetTarget(SLGBuildingType type)
        {
            if (!_map[0].activeSelf && !_map[1].activeSelf && !_map[2].activeSelf)
            {
                StartOrbitingArrow();
                Debug.Log("SLG 화살표 특수상황 - 공전 시작");
                return;
            }

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

            UpdateArrowRotationInstantly();

            if (_arrowRoutine != null)
                StopCoroutine(_arrowRoutine);
            _arrowRoutine = StartCoroutine(ArrowActivationRoutine());
        }

        private IEnumerator ArrowActivationRoutine()
        {
            yield return StartCoroutine(FadeArrow(0f, 1f, _fadeTime));
            yield return new WaitForSeconds(_arrowTime - _fadeTime * 2);
            yield return StartCoroutine(FadeArrow(1f, 0f, _fadeTime));
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

            _rb.MovePosition(desiredPosition);

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float currentAngle = _rb.rotation;
            float smoothedAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref _currentVelocity, _rotationSmoothTime);

            _rb.MoveRotation(smoothedAngle);
        }

        private void UpdateArrowRotationInstantly()
        {
            if (_target == null) return;

            Vector3 direction = (_target.position - _player.position).normalized;
            Vector3 newPosition = _player.position + direction * _arrowDistance;

            _rb.position = newPosition;

            float newAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _rb.rotation = newAngle;
        }

        public void StartOrbitingArrow()
        {
            if (_arrowRoutine != null)
                StopCoroutine(_arrowRoutine);
            _arrowRoutine = StartCoroutine(OrbitArrow());
        }

        private IEnumerator OrbitArrow()
        {
            _isRotating = true;

            // 화살표 초기 위치를 플레이어의 오른쪽에서 시작
            transform.position = _player.position + new Vector3(_arrowDistance, 0, 0);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            _sr.color = new Color(1f, 1f, 1f, 0f); // 시작 시 투명

            float orbitTime = _arrowTime;
            float elapsedTime = 0f;
            float angleSpeed = 720f / orbitTime; // 2바퀴 (720도) / 2초

            while (elapsedTime < orbitTime)
            {
                elapsedTime += Time.deltaTime;

                // 현재 진행된 각도 계산 (0도에서 시작)
                float angle = (elapsedTime / orbitTime) * 720f * Mathf.Deg2Rad; // 2바퀴(720도)

                // 공전하면서 플레이어와의 거리 유지
                Vector3 newPos = _player.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * _arrowDistance;
                _rb.MovePosition(newPos);

                // 화살표가 항상 플레이어 반대 방향을 가리키도록 회전 (오른쪽 기준)
                Vector3 direction = (_player.position - newPos).normalized;
                float newAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;
                _rb.MoveRotation(newAngle);

                // 페이드인/페이드아웃을 2초 내에 진행
                float alpha = Mathf.Lerp(0f, 1f, elapsedTime / _fadeTime);
                if (elapsedTime > orbitTime - _fadeTime)
                {
                    alpha = Mathf.Lerp(1f, 0f, (elapsedTime - (orbitTime - _fadeTime)) / _fadeTime);
                }
                _sr.color = new Color(1f, 1f, 1f, alpha);

                yield return null;
            }

            _isRotating = false;
        }
    }
}