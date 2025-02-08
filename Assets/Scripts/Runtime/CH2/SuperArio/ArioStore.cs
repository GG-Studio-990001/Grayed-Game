using Runtime.ETC;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.CH2.SuperArio
{
    public class ArioStore : MonoBehaviour
    {
        [SerializeField] private SurfaceEffector2D _surface;
        [SerializeField] private StoreWall[] _storeWalls;
        [SerializeField] private GameObject[] _openWalls;
        [SerializeField] private GameObject[] _boxes;
        [SerializeField] private float _jumpForce = 12f;
        [SerializeField] private float _maxJumpHeight = 3f;
        [SerializeField] private float _fallMultiplier = 2.5f;
        [SerializeField] private float _lowJumpMultiplier = 2f;

        private Vector2 _initPos;
        private Rigidbody2D _rb;
        private SpriteRenderer spr;
        private bool _isGrounded;
        private bool _isJumping;
        private float _initialJumpPosition;
        private bool _isJumpHeld;
        private float _surfaceVelocityX;
        private Collider2D _collider;

        private void Start()
        {
            _initPos = transform.position;
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            spr = GetComponent<SpriteRenderer>();
            _rb.isKinematic = true;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            ArioManager.instance.OnEnterStore += EnterStore;
            ArioManager.instance.OpenStore += OpenWalls;
        }

        private void OnDestroy()
        {
            if (ArioManager.instance != null)
            {
                ArioManager.instance.OnEnterStore -= EnterStore;
                ArioManager.instance.OpenStore -= OpenWalls;
            }
        }

        private void EnterStore(bool isTrue)
        {
            _rb.isKinematic = false;
            gameObject.SetActive(true);
            foreach (var box in _boxes)
            {
                if (box.TryGetComponent(out IStoreBox storeBox))
                    storeBox.Check();
            }
        }

        public void ExitStore()
        {
            _isGrounded = false;
            spr.flipX = false;
            _rb.isKinematic = true;
            transform.position = _initPos;
            _surface.speed = 3.5f;
            
            foreach (var wall in _storeWalls)
            {
                wall.gameObject.SetActive(true);
            }

            foreach (var wall in _openWalls)
            {
                wall.gameObject.SetActive(false);
            }
            
            ArioManager.instance.ExitStore();
            gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (!ArioManager.instance.IsStore) return;

            // 떨어지는 중일 때만 지면 상태 해제
            if (_rb.velocity.y < -0.5f)
            {
                _isGrounded = false;
            }

            HandleMovement();
        }

        private void HandleMovement()
        {
            // 지면에 있을 때만 surface 속도 적용
            if (_isGrounded)
            {
                _rb.velocity = new Vector2(_surfaceVelocityX, _rb.velocity.y);
            }

            // 낙하 가속도
            if (_rb.velocity.y < 0)
            {
                _rb.velocity += Vector2.up * (Physics2D.gravity.y * (_fallMultiplier - 1) * Time.fixedDeltaTime);
            }
            // 낮은 점프 가속도
            else if (_rb.velocity.y > 0 && !_isJumpHeld)
            {
                _rb.velocity += Vector2.up * (Physics2D.gravity.y * (_lowJumpMultiplier - 1) * Time.fixedDeltaTime);
            }

            // 점프 중 이동
            if (_isJumping)
            {
                _rb.velocity = new Vector2(_surfaceVelocityX, _rb.velocity.y);

                if (transform.position.y >= _initialJumpPosition + _maxJumpHeight)
                {
                    _rb.velocity = new Vector2(_surfaceVelocityX, 0);
                }
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!ArioManager.instance.IsStore) return;
            
            Vector2 moveInput = context.ReadValue<Vector2>();

            // 즉시 점프 처리
            if (context.phase == InputActionPhase.Started && moveInput.y > 0)
            {
                if (_isGrounded)
                {
                    Jump();
                    _isJumpHeld = true;
                }
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                _isJumpHeld = false;
            }
        }

        private void Jump()
        {
            if (!_isGrounded) return;
            
            _isJumping = true;
            _isGrounded = false;
            _initialJumpPosition = transform.position.y;
            _rb.velocity = new Vector2(_surfaceVelocityX, _jumpForce);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out StoreWall wall))
            {
                _surfaceVelocityX = wall.IsLeft ? 3.5f : -3.5f;
                spr.flipX = !wall.IsLeft;
                if (_surface != null)
                    _surface.speed = _surfaceVelocityX;
                return;
            }

            // 박스와의 충돌 처리
            if (other.gameObject.CompareTag(GlobalConst.ObstacleStr))
            {
                foreach (ContactPoint2D contact in other.contacts)
                {
                    if (Mathf.Abs(contact.normal.x) > 0.5f)
                    {
                        float bounceForce = 5f;
                        Vector2 bounceDirection = contact.normal;
                        _rb.velocity = new Vector2(bounceDirection.x * bounceForce, _rb.velocity.y);
                    }
                }
                return;
            }

            // 지면 체크
            if (other.gameObject.TryGetComponent(out StoreGround ground))
            {
                // 이미 점프 중이면 지면 체크 스킵
                if (_isJumping && _rb.velocity.y > 0) return;

                foreach (ContactPoint2D contact in other.contacts)
                {
                    // 지면 체크 조건 완화
                    if (Vector2.Dot(contact.normal, Vector2.up) > 0.5f)
                    {
                        _isGrounded = true;
                        if (_rb.velocity.y <= 0)
                        {
                            _isJumping = false;
                        }

                        if (ground.TryGetComponent(out SurfaceEffector2D surfaceEffector))
                        {
                            _surfaceVelocityX = surfaceEffector.speed;
                            _surface = surfaceEffector;
                        }
                        return;
                    }
                }
            }
        }

        private void OpenWalls()
        {
            foreach (var wall in _storeWalls)
            {
                wall.gameObject.SetActive(false);
            }

            foreach (var wall in _openWalls)
            {
                wall.gameObject.SetActive(true);
            }
        }
    }
}