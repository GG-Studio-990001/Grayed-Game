using Runtime.ETC;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.CH3
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class QuaterViewPlayer : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5.0f; // 이동 속도
        [SerializeField] private Cinemachine.CinemachineVirtualCamera virtualCamera; // Cinemachine 가상 카메라
        
        private Vector2 _movementInput; // 현재 이동 입력
        private PlayerState _state = PlayerState.Idle; // 플레이어 상태
        private Rigidbody _rigidbody; // Rigidbody 컴포넌트

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>(); // Rigidbody 초기화
        }

        private void FixedUpdate()
        {
            // 플레이어 상태가 "Get"일 경우 이동하지 않음
            if (_state == PlayerState.Get)
                return;

            MovePlayer(); // 플레이어 이동 처리
        }

        private void MovePlayer()
        {
            // 카메라의 포워드 벡터와 오른쪽 벡터를 사용하여 이동 방향 계산
            Vector3 forward = virtualCamera.transform.forward;
            Vector3 right = virtualCamera.transform.right;

            // Y축의 영향을 제거하여 수평 이동만 고려
            forward.y = 0;
            right.y = 0;

            // 정규화하여 방향 벡터를 계산
            forward.Normalize();
            right.Normalize();

            // 입력에 따라 이동 방향 결정
            Vector3 moveDirection = forward * _movementInput.y + right * _movementInput.x;

            // 플레이어 이동
            _rigidbody.MovePosition(_rigidbody.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

            // 상태 업데이트
            _state = _movementInput != Vector2.zero ? PlayerState.Move : PlayerState.Idle;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _movementInput = context.ReadValue<Vector2>(); // 이동 입력 읽기
        }

        public void OnInteraction()
        {
            // 상호작용 로직 추가
        }

        public void PlayerIdle()
        {
            _state = PlayerState.Idle; // 플레이어 상태를 Idle로 설정
        }

        public void SetLastInput(Vector2 direction)
        {
            // 마지막 입력 방향 설정
        }
    }
}