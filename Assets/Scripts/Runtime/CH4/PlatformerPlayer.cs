using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.CH4
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlatformerPlayer : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _jumpForce = 12f;

        [Header("Ground Check")]
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private float _groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask _groundLayer;

        [Header("Anim")]
        [SerializeField] private PlatformerAnim _anim;

        [Header("Improvements")]
        [SerializeField] private float _coyoteTime = 0.1f;
        private float _coyoteTimer;
        [SerializeField] private float _jumpBufferTime = 0.1f;
        private float _jumpBufferTimer;

        private Rigidbody2D _rb;
        private Vector2 _moveInput;
        private bool _isGrounded;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _jumpBufferTimer = _jumpBufferTime; // 점프 입력 기억해놓기
            }
        }

        public void OnInteraction()
        {
            // _interaction.Interact(_movement.Direction);
        }

        private void Update()
        {
            _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);

            // 코요테 타이머
            if (_isGrounded)
                _coyoteTimer = _coyoteTime;
            else
                _coyoteTimer -= Time.deltaTime;

            // 점프 버퍼 타이머
            if (_jumpBufferTimer > 0)
                _jumpBufferTimer -= Time.deltaTime;

            // 버퍼 점프 조건
            if (_jumpBufferTimer > 0 && _coyoteTimer > 0)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
                _coyoteTimer = 0f;
                _jumpBufferTimer = 0f;
            }

            _anim.UpdateAnim(_moveInput, _isGrounded);
        }


        private void FixedUpdate()
        {
            _rb.velocity = new Vector2(_moveInput.x * _moveSpeed, _rb.velocity.y);
        }

        private void OnDrawGizmosSelected()
        {
            if (_groundCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
            }
        }
    }
}