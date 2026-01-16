using Runtime.ETC;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.CH3.Main
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5.0f;
        [SerializeField] private Cinemachine.CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float interactDebounce = 0.2f;

        // 애니메이션 관련 추가
        private Animator _animator;
        private static readonly int HashMoveSpeed = Animator.StringToHash("MoveSpeed");
        private static readonly int HashDirX = Animator.StringToHash("DirX");
        private static readonly int HashDirY = Animator.StringToHash("DirY");

        private Vector2 _movementInput;
        private PlayerState _state = PlayerState.Idle;
        private PlayerGrid _gridObject;
        private Rigidbody _rigidbody;
        private GridSystem _gridManager;
        private InteractionManager _interactionManager;
        private float _lastInteractTime = -999f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _interactionManager = GetComponent<InteractionManager>();
            _gridManager = FindObjectOfType<GridSystem>();
            _gridObject = GetComponent<PlayerGrid>();

            // Animator 컴포넌트 가져오기 (자식 오브젝트에 있다면 GetComponentInChildren 사용)
            _animator = GetComponentInChildren<Animator>();

            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
            _rigidbody.useGravity = false;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            if (_gridObject == null) Debug.LogError("PlayerGridObject 컴포넌트가 없습니다!");
        }

        private void Start()
        {
            // (기존 스폰 좌표 로직 유지...)
            if (_gridManager != null && _gridManager.HasPlayerSpawn)
            {
                Vector3 spawnPos = _gridManager.GridToWorldPosition(_gridManager.PlayerSpawnGrid);
                if (_gridObject != null)
                    spawnPos.y = _gridObject.UseCustomY ? _gridObject.CustomY : transform.position.y;
                else
                    spawnPos.y = transform.position.y;

                if (_rigidbody != null)
                {
                    _rigidbody.velocity = Vector3.zero;
                    _rigidbody.position = spawnPos;
                }
                else
                    transform.position = spawnPos;
            }
        }

        private void FixedUpdate()
        {
            if (_state == PlayerState.Get) return;

            MovePlayer();
            UpdateAnimation(); // 애니메이션 파라미터 업데이트 호출
        }

        private void MovePlayer()
        {
            if (BuildingSystem.Instance != null && BuildingSystem.Instance.IsBuildingMode)
            {
                StopMovement();
                return;
            }

            Vector3 cameraForward = Vector3.ProjectOnPlane(virtualCamera.transform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.ProjectOnPlane(virtualCamera.transform.right, Vector3.up).normalized;

            Vector3 moveDirection = (cameraForward * _movementInput.y + cameraRight * _movementInput.x).normalized;

            if (moveDirection != Vector3.zero)
            {
                Vector3 targetPosition = transform.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
                Vector2Int targetGridPos = _gridManager.WorldToGridPosition(targetPosition);

                bool movingToAnotherCell = (_gridObject != null) && (targetGridPos != _gridObject.GridPosition);
                bool occupiedByImpassable = movingToAnotherCell && _gridManager.IsCellOccupiedByImpassableObject(targetGridPos);

                if (_gridManager.IsCellBlocked(targetGridPos) || occupiedByImpassable)
                {
                    StopMovement();
                    return;
                }

                _rigidbody.velocity = new Vector3(moveDirection.x * moveSpeed, _rigidbody.velocity.y, moveDirection.z * moveSpeed);
                _state = PlayerState.Move;
            }
            else
            {
                StopMovement();
            }
        }

        private void StopMovement()
        {
            Vector3 currentVelocity = _rigidbody.velocity;
            currentVelocity.x = 0f;
            currentVelocity.z = 0f;
            _rigidbody.velocity = currentVelocity;
            _state = PlayerState.Idle;
        }

        // 애니메이션 파라미터 갱신 로직
        private void UpdateAnimation()
        {
            if (_animator == null) return;

            // 1. 이동 속도 전달 (magnitude를 사용하여 0~1 사이 값 전달)
            // _movementInput.magnitude를 쓰면 입력의 강도에 따라 걷기/뛰기 블렌딩 가능
            float speed = (_state == PlayerState.Move) ? _movementInput.magnitude : 0f;
            _animator.SetFloat(HashMoveSpeed, speed);

            // 2. 방향 전달 (입력이 있을 때만 갱신하여 멈췄을 때 마지막 방향 유지)
            if (_movementInput != Vector2.zero)
            {
                _animator.SetFloat(HashDirX, _movementInput.x);
                _animator.SetFloat(HashDirY, _movementInput.y);
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _movementInput = context.ReadValue<Vector2>();
        }

        // (나머지 OnInteraction, OnInteractionHold 등 기존 코드 유지...)
        public void OnInteraction()
        {
            if (Time.time - _lastInteractTime < interactDebounce) return;
            _lastInteractTime = Time.time;
            _interactionManager.TryInteract();
        }

        public void OnInteractionHold(InputAction.CallbackContext context)
        {
            if (context.started) _interactionManager.BeginHold();
            else if (context.performed) _interactionManager.UpdateHold();
            else if (context.canceled) _interactionManager.CancelHold();
        }

        public void PlayerIdle()
        {
            _state = PlayerState.Idle;
        }

        public void SetLastInput(Vector2 direction)
        {
            if (_animator != null)
            {
                _animator.SetFloat(HashDirX, direction.x);
                _animator.SetFloat(HashDirY, direction.y);
            }
        }
    }
}