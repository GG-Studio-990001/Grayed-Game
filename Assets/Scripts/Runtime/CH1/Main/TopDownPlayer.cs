using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.CH1.Main
{
    public class TopDownPlayer : MonoBehaviour
    {
        // TODO 이거 데이터로 빼야함
        [SerializeField] private float moveSpeed = 5.0f;
        [SerializeField] private float animSpeed = 0.5f;
        
        private Vector2 _movementInput;
        private TopDownMovement _movement;
        private TopDownAnimation _animation;
        
        private void Awake()
        {
            _movement = new TopDownMovement(moveSpeed, transform);
            
            if (GetComponent<Animator>() == null)
            {
                throw new NullReferenceException("Animator is null");
            }
            _animation = new TopDownAnimation(GetComponent<Animator>(), animSpeed);
        }

        private void OnMove(InputValue value)
        {
            _movementInput = value.Get<Vector2>();
        }
        
        private void Update()
        {
            _animation.SetMovementAnimation(_movementInput);
        }

        private void FixedUpdate()
        {
            _movement.Move(_movementInput);
        }
        
    }
}