using Runtime.ETC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.CH2.SuperArio
{
    public class ArioStore : MonoBehaviour
    {
        [SerializeField] private SurfaceEffector2D _surface;
        [SerializeField] private GameObject[] _openWalls;
        [SerializeField] private GameObject[] _boxes;
        [SerializeField] private float _jumpForce = 12f;
        [SerializeField] private float _fallMultiplier = 2.5f;

        private Vector2 _initPos;
        private Rigidbody2D _rb;
        private SpriteRenderer spr;
        private bool _isGrounded;
        private bool _isJumping;
        private float _surfaceVelocityX;

        private void Start()
        {
            _initPos = transform.position;
            _rb = GetComponent<Rigidbody2D>();
            spr = GetComponent<SpriteRenderer>();
            _rb.isKinematic = true;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            ArioManager.Instance.OnEnterStore += EnterStore;
            ArioManager.Instance.OpenStore += OpenWalls;
        }

        private void OnDestroy()
        {
            if (ArioManager.Instance != null)
            {
                ArioManager.Instance.OnEnterStore -= EnterStore;
                ArioManager.Instance.OpenStore -= OpenWalls;
            }
        }

        private void EnterStore(bool isTrue)
        {
            _rb.isKinematic = false;
            gameObject.SetActive(true);
            foreach (var box in _boxes)
            {
                if (box.TryGetComponent(out ItemBox itemBox))
                    itemBox.Check();
            }

            StartCoroutine(EnterSoundDelay());
        }

        private IEnumerator EnterSoundDelay()
        {
            yield return new WaitForSeconds(0.5f);
            Managers.Sound.Play(Sound.SFX, "SuperArio/CH2_SUB_SFX_15");
        }

        public void ExitStore()
        {
            _isGrounded = false;
            spr.flipX = false;
            _rb.isKinematic = true;
            _rb.velocity = Vector2.zero;
            transform.position = _initPos;
            _surface.speed = 3.5f;
            ArioManager.Instance.ExitStore();
            StartCoroutine(CloseWalls());
        }

        private void FixedUpdate()
        {
            if (!ArioManager.Instance.IsStore) return;

            HandleMovement();
        }

        private void HandleMovement()
        {
            if (_isGrounded)
            {
                _rb.velocity = new Vector2(_surfaceVelocityX, _rb.velocity.y);
                _isJumping = false; // 지면에 있을 때 점프 상태 종료
            }

            if (_rb.velocity.y < 0)
            {
                _rb.velocity += Vector2.up * (Physics2D.gravity.y * (_fallMultiplier - 1) * Time.fixedDeltaTime);
            }

            if (_isJumping)
            {
                // 점프 중에는 수직 속도를 변경하지 않음
                if (_rb.velocity.y <= 0)
                {
                    _isJumping = false; // 점프 상태 종료
                }
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!ArioManager.Instance.IsStore) return;

            Vector2 moveInput = context.ReadValue<Vector2>();

            // 즉시 점프 처리
            if (context.phase == InputActionPhase.Started && moveInput.y > 0)
            {
                if (_isGrounded) // 지면에 있을 때만 점프
                {
                    Managers.Sound.Play(Sound.SFX, "SuperArio/CH2_SUB_SFX_02");
                    Jump();
                }
            }
        }

        private void Jump()
        {
            if (!_isGrounded) return;

            _isJumping = true;
            _isGrounded = false;
            _rb.velocity = new Vector2(_surfaceVelocityX, _jumpForce); // 고정된 점프 힘으로 점프
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out StoreWall wall))
            {
                var tempVelocity = wall.IsLeft ? 3.5f : -3.5f;
                if (Mathf.Approximately(tempVelocity, _surfaceVelocityX))
                    return;
                _surfaceVelocityX = tempVelocity;
                Managers.Sound.Play(Sound.SFX, "SuperArio/CH2_SUB_SFX_16");
                spr.flipX = !wall.IsLeft;
                if (_surface != null)
                    _surface.speed = _surfaceVelocityX;
                return;
            }

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

            if (other.gameObject.TryGetComponent(out StoreGround ground))
            {
                foreach (ContactPoint2D contact in other.contacts)
                {
                    if (Vector2.Dot(contact.normal, Vector2.up) > 0.25f)
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
            foreach (var wall in _openWalls)
            {
                wall.GetComponent<StoreWall>().OpenWall();
            }
        }

        private IEnumerator CloseWalls()
        {
            yield return new WaitForSeconds(1f);
            foreach (var wall in _openWalls)
            {
                wall.GetComponent<StoreWall>().CloseWall();
            }
            gameObject.SetActive(false);
        }
    }
}