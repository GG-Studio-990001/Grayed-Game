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
            if (context.performed && _isGrounded)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, _jumpForce);
            }
        }

        public void OnInteraction()
        {
            // _interaction.Interact(_movement.Direction);
        }

        private void Update()
        {
            _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);

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