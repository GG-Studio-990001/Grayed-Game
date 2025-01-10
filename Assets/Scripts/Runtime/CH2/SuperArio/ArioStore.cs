using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
        private bool _isGrounded;
        private bool _isJumping;
        private float _initialJumpPosition;
        private bool _isJumpHeld;
        private float _surfaceVelocityX;
        private Coroutine _coroutine;

        private void Start()
        {
            _initPos = transform.position;
            _rb = GetComponent<Rigidbody2D>();
            _rb.isKinematic = true;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            ArioManager.instance.OnEnterStore += EnterStore;
            ArioManager.instance.OpenStore += OpenWalls;
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
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
            
            _rb.isKinematic = true;
            transform.position = _initPos;
            // 벽 복구
            foreach (StoreWall storeWall in _storeWalls)
            {
                storeWall.gameObject.SetActive(true);
            }

            foreach (GameObject openWall in _openWalls)
            {
                openWall.gameObject.SetActive(false);
            }
            
            ArioManager.instance.ExitStore();
            gameObject.SetActive(false);
        }

        private void FixedUpdate()
        {
            if (!ArioManager.instance.IsStore) return;

            if (_isGrounded)
            {
                _surfaceVelocityX = _surface.speed;
            }
            
            if (_rb.velocity.y < 0)
            {
                _rb.velocity += Vector2.up * (Physics2D.gravity.y * (_fallMultiplier - 1) * Time.fixedDeltaTime);
            }
            else if (_rb.velocity.y > 0 && !_isJumpHeld)
            {
                _rb.velocity += Vector2.up * (Physics2D.gravity.y * (_lowJumpMultiplier - 1) * Time.fixedDeltaTime);
            }

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
            if (!ArioManager.instance.IsStore)
                return;
            
            Vector2 moveInput = context.ReadValue<Vector2>();

            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    if (moveInput.y > 0 && _isGrounded && !_isJumping)
                    {
                        Jump();
                        _isJumpHeld = true;
                    }
                    break;
                case InputActionPhase.Canceled:
                    _isJumpHeld = false;
                    break;
            }
        }

        private void Jump()
        {
            _isJumping = true;
            _isGrounded = false;
            _initialJumpPosition = transform.position.y;
            _rb.velocity = new Vector2(_surfaceVelocityX, _jumpForce);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out StoreWall wall))
            {
                if (wall.IsLeft)
                {
                    _surface.speed = 3.5f;
                    _surfaceVelocityX = 3.5f;
                }
                else
                {
                    _surface.speed = -3.5f;
                    _surfaceVelocityX = -3.5f;
                }
            }
            else if (other.gameObject.TryGetComponent(out StoreGround ground))
            {
                foreach (ContactPoint2D contact in other.contacts)
                {
                    if (Vector2.Dot(contact.normal, Vector2.up) > 0.5f)
                    {
                        if (contact.point.y < transform.position.y)
                        {
                            _isGrounded = true;
                            if (_rb.velocity.y <= 0)
                            {
                                _isJumping = false;
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            if (!gameObject.activeInHierarchy) return;
            
            if (other.gameObject.TryGetComponent(out StoreGround ground))
            {
                _coroutine = StartCoroutine(GroundedBufferRoutine());
            }
        }

        private IEnumerator GroundedBufferRoutine()
        {
            yield return new WaitForSeconds(0.1f);
            if (!_isJumping)
            {
                _isGrounded = false;
            }
        }

        private void OnDisable()
        {
            if(_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        private void OpenWalls()
        {
            foreach (StoreWall storeWall in _storeWalls)
            {
                storeWall.gameObject.SetActive(false);
            }

            foreach (GameObject openWall in _openWalls)
            {
                openWall.gameObject.SetActive(true);
            }
        }
    }
}