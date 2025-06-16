using UnityEngine;
using InputOld = UnityEngine.Input;

namespace Runtime.CH4
{
    public class JumpKing : MonoBehaviour
    {
        [SerializeField] private float _walkSpeed;
        [SerializeField] private bool _isGrounded;
        [SerializeField] private LayerMask _groundMask;

        [SerializeField] private PhysicsMaterial2D _normalMat, _BounceMat;
        [SerializeField] private bool _canJump = true;
        [SerializeField] private float _jumpValue = 0.0f;

        private float _legLen = 1.5f;
        private Vector2 _feetSize = new(1.4f, 0.4f);

        private float _moveInput;
        private Rigidbody2D _rb;


        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            _moveInput = InputOld.GetAxisRaw("Horizontal");

            if (_jumpValue == 0f && _isGrounded)
            {
                _rb.velocity = new Vector2(_moveInput * _walkSpeed, _rb.velocity.y);
            }

            _isGrounded = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - _legLen),
                _feetSize, 0f, _groundMask);

            if (!_isGrounded)
            {
                _rb.sharedMaterial = _BounceMat;
            }
            else
            {
                _rb.sharedMaterial = _normalMat;
            }

            if (InputOld.GetKey(KeyCode.Space) && _isGrounded && _canJump)
            {
                _jumpValue += 0.1f;
            }

            if (InputOld.GetKeyDown(KeyCode.Space) && _isGrounded && _canJump)
            {
                _rb.velocity = new Vector2(0f, _rb.velocity.y);
            }

            if (_jumpValue >= 20f && _isGrounded)
            {
                float tmpX = _moveInput * _walkSpeed;
                float tmpY = _jumpValue;
                _rb.velocity = new Vector2(tmpX, tmpY);
                Invoke(nameof(ResetJump), 0.2f);
            }

            if (InputOld.GetKeyUp(KeyCode.Space))
            {
                if (_isGrounded)
                {
                    _rb.velocity = new Vector2(_moveInput * _walkSpeed, _jumpValue);
                    _jumpValue = 0f;
                }
                _canJump = true;
            }
        }

        private void ResetJump()
        {
            _canJump = false;
            _jumpValue = 0f;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(new Vector2(transform.position.x, transform.position.y - _legLen), _feetSize);
        }
    }
}