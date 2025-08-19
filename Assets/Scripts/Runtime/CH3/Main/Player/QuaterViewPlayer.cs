using Runtime.CH3.Main;
using Runtime.ETC;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.CH3
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class QuaterViewPlayer : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5.0f;
        [SerializeField] private Cinemachine.CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float interactDebounce = 0.2f; // 상호작용 디바운스 시간

        private Vector2 _movementInput;
        private PlayerState _state = PlayerState.Idle;
        private PlayerGridObject _gridObject;
        private Rigidbody _rigidbody;
        private GridManager _gridManager;
        private InteractionManager _interactionManager;
        private float _lastInteractTime = -999f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _interactionManager = GetComponent<InteractionManager>();
            _gridManager = FindObjectOfType<GridManager>();
            _gridObject = GetComponent<PlayerGridObject>();

            // Rigidbody 설정 수정
            _rigidbody.constraints =
                RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY; // Y축 위치도 고정
            _rigidbody.useGravity = false; // 중력 비활성화
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            if (_gridManager != null)
            {
                transform.position = _gridManager.GetCenterPosition(transform);
            }

            // 디버그: 컴포넌트 확인
            if (_gridObject == null)
            {
                Debug.LogError("PlayerGridObject 컴포넌트가 없습니다!");
            }
        }

        private void FixedUpdate()
        {
            if (_state == PlayerState.Get)
                return;

            MovePlayer();
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
                // 목표 위치 계산
                Vector3 targetPosition = transform.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
                
                // 목표 위치가 차단된 셀인지 확인
                Vector2Int targetGridPos = _gridManager.WorldToGridPosition(targetPosition);
                if (_gridManager.IsCellBlocked(targetGridPos))
                {
                    // 이동이 불가능하면 정지
                    Vector3 currentVelocity = _rigidbody.velocity;
                    currentVelocity.x = 0f;
                    currentVelocity.z = 0f;
                    _rigidbody.velocity = currentVelocity;
                    _state = PlayerState.Idle;
                    
                    return;
                }

                // 이동 속도 적용
                Vector3 targetVelocity = moveDirection * moveSpeed;
                targetVelocity.y = _rigidbody.velocity.y; // 수직 속도 유지

                // 부드러운 이동을 위해 Velocity 사용
                _rigidbody.velocity = targetVelocity;

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
            _movementInput = context.ReadValue<Vector2>();
        }

        public void OnInteraction()
        {
            // 길게 누름/중복 입력에 대한 디바운스
            if (Time.time - _lastInteractTime < interactDebounce)
                return;
            _lastInteractTime = Time.time;

            _interactionManager.TryInteract();
        }

        public void PlayerIdle()
        {
            _state = PlayerState.Idle;
        }

        public void SetLastInput(Vector2 direction)
        {
            // 마지막 입력 방향 설정
        }
    }
}