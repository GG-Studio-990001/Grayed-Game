using UnityEngine;
using InputOld = UnityEngine.Input;

namespace Runtime.CH4
{
    public class JumpKing : MonoBehaviour
    {
        [SerializeField] private bool _isGrounded;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private PhysicsMaterial2D _normalMat, _BounceMat;
        [SerializeField] private bool _canJump = true;
        [SerializeField] private float _walkSpeed = 6;
        [SerializeField] private float _jumpValue = 0f;
        [SerializeField] private float _minJumpValue = 5f;
        [SerializeField] private float _maxJumpValue = 24f;
        [SerializeField] private float _jumpMul = 1.3f;

        private readonly float _legLen = 1.2f;
        private readonly Vector2 _feetSize = new(1.2f, 1f);

        private float _moveInput;
        private Rigidbody2D _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            HandeMovement();
            CheckGround();
            Charge();
            Jump();
        }

        private void HandeMovement()
        {
            _moveInput = InputOld.GetAxisRaw("Horizontal");

            if (_jumpValue == _minJumpValue && _isGrounded)
            {
                _rb.velocity = new Vector2(_moveInput * _walkSpeed, _rb.velocity.y);
            }
        }    

        private void CheckGround()
        {
            _isGrounded = Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y - _legLen),
                _feetSize, 0f, _groundMask);

            _rb.sharedMaterial = _isGrounded ? _normalMat : _BounceMat;
        }

        private void Charge()
        {
            if (InputOld.GetKey(KeyCode.Space) && _isGrounded && _canJump)
            {
                _jumpValue += 0.1f;
            }

            if (InputOld.GetKeyDown(KeyCode.Space) && _isGrounded && _canJump)
            {
                _rb.velocity = new Vector2(0f, _rb.velocity.y);
            }
        }

        private void Jump()
        {
            // 자동 점프
            if (_jumpValue >= _maxJumpValue && _isGrounded)
            {
                float tmpX = _moveInput * _walkSpeed * _jumpMul;
                float tmpY = _jumpValue;
                _rb.velocity = new Vector2(tmpX, tmpY);
                Invoke(nameof(ResetJump), 0.2f);
            }

            // 점프
            if (InputOld.GetKeyUp(KeyCode.Space))
            {
                if (_isGrounded)
                {
                    _rb.velocity = new Vector2(_moveInput * _walkSpeed * _jumpMul, _jumpValue);
                    _jumpValue = _minJumpValue;
                }
                _canJump = true;
            }
        }

        private void ResetJump()
        {
            _canJump = false;
            _jumpValue = _minJumpValue;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(new Vector2(transform.position.x, transform.position.y - _legLen), _feetSize);
        }
    }
}