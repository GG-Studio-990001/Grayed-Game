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
            // 카메라 기준 이동 방향 계산
            Vector3 cameraForward = Vector3.ProjectOnPlane(virtualCamera.transform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.ProjectOnPlane(virtualCamera.transform.right, Vector3.up).normalized;

            // 입력값을 기반으로 이동 방향 계산
            Vector3 moveDirection = (cameraForward * _movementInput.y + cameraRight * _movementInput.x).normalized;
    
            if (moveDirection != Vector3.zero)
            {
                // 이동 속도 적용
                Vector3 targetVelocity = moveDirection * moveSpeed;
                targetVelocity.y = _rigidbody.velocity.y; // 수직 속도 유지
        
                // 부드러운 이동을 위해 Velocity 사용
                _rigidbody.velocity = targetVelocity;
        
                // 이동 방향으로 캐릭터 회전
                transform.forward = moveDirection;
        
                _state = PlayerState.Move;
            }
            else
            {
                // 정지 시 수평 속도를 0으로 설정
                Vector3 currentVelocity = _rigidbody.velocity;
                currentVelocity.x = 0f;
                currentVelocity.z = 0f;
                _rigidbody.velocity = currentVelocity;
        
                _state = PlayerState.Idle;
            }
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