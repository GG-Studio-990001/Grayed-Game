using Runtime.ETC;
using Runtime.Interface;
using UnityEngine;

namespace Runtime.CH1.Main.Player
{
    public class TopDownAnimation : IAnimation
    {
        private readonly Animator _animator;
        private static readonly int Moving = Animator.StringToHash(IsMoving);
        private static readonly int Getting = Animator.StringToHash(IsGetting);
        private static readonly int Horizontal1 = Animator.StringToHash(Horizontal);
        private static readonly int Vertical1 = Animator.StringToHash(Vertical);

        private const string IsMoving = "IsMoving";
        private const string IsGetting = "IsGetting";
        private const string Horizontal = "Horizontal";
        private const string Vertical = "Vertical";
        
        public TopDownAnimation(Animator animator, float animationSpeed = 1.0f)
        {
            _animator = animator;
            _animator.speed = animationSpeed;
        }

        public bool SetAnimation(string stateName, Vector2 direction = default)
        {
            switch (stateName)
            {
                case GlobalConst.IdleStr:
                    _animator.SetBool(Moving, false);
                    _animator.SetBool(Getting, false);
                    _animator.SetFloat(Horizontal1, direction.x);
                    _animator.SetFloat(Vertical1, direction.y);
                    break;
                case GlobalConst.MoveStr:
                    _animator.SetBool(Moving, true);
                    _animator.SetFloat(Horizontal1, direction.x);
                    _animator.SetFloat(Vertical1, direction.y);
                    break;
                case nameof(PlayerState.Get):
                    Debug.Log("겟");
                    _animator.SetBool(Moving, false);
                    _animator.SetBool(Getting, true);
                    _animator.SetFloat(Horizontal1, 0);
                    _animator.SetFloat(Vertical1, 0);
                    break;
                default:
                    Debug.LogError("Invalid PlayerState");
                    return false;
            }
            return true;
        }
    }
}