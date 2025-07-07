using UnityEngine;

namespace Runtime.CH4
{
    public class PlatformerAnim : MonoBehaviour
    {
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void UpdateAnim(Vector2 moveInput, bool isGrounded)
        {
            float speed = isGrounded ? Mathf.Abs(moveInput.x) : 0f; // 공중이면 Speed = 0

            _animator.SetFloat("Speed", speed);

            if (moveInput.x != 0)
            {
                _animator.SetInteger("Facing", moveInput.x > 0 ? 1 : -1);
            }
        }
    }
}