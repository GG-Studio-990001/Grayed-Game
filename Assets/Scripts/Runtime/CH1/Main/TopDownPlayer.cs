using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.CH1.Main
{
    public class TopDownPlayer : MonoBehaviour
    {
        // TODO 이거 데이터로 빼야함
        [SerializeField] private float moveSpeed = 5.0f;
        
        private TopDownMovement _movement;
        private Vector2 _movementInput;

        private void Awake()
        {
            _movement = new TopDownMovement(moveSpeed, transform);
        }

        private void OnMove(InputValue value)
        {
            _movementInput = value.Get<Vector2>();
        }

        private void FixedUpdate()
        {
            _movement.Move(_movementInput);
        }
        
    }
}