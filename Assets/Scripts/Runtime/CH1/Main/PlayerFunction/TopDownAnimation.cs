using UnityEngine;

namespace Runtime.CH1.Main
{
    public class TopDownAnimation
    {
        private Animator _animator;
        private float _animationSpeed;
        
        private const string IsMoving = "IsMoving";
        private const string Horizontal = "Horizontal";
        private const string Vertical = "Vertical";
        
        public TopDownAnimation(Animator animator, float animationSpeed = 1.0f)
        {
            _animator = animator;
            _animationSpeed = animationSpeed;
            
            _animator.speed = _animationSpeed;
        }
        
        public void SetMovementAnimation(Vector2 movementInput)
        {
            // TODO 리터럴값 제거, 애니메이션 확장되는대로
            if (movementInput == Vector2.zero)
            {
                _animator.SetBool(IsMoving, false);
                return;
            }
            
            _animator.SetBool(IsMoving, true);
            
            _animator.SetFloat(Horizontal, movementInput.x);
            _animator.SetFloat(Vertical, movementInput.y);
        }
    }
}