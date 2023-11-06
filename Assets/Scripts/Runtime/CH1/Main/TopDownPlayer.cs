using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.CH1.Main
{
    public class TopDownPlayer : MonoBehaviour
    {
        public float moveSpeed = 5.0f;
        private Vector2 movementInput;

        private void OnMove(InputValue value)
        {
            Vector2 inputVector = value.Get<Vector2>();
            if (inputVector.magnitude > 1f)
            {
                return;
            }

            movementInput = new Vector2(inputVector.x, inputVector.y);
        }

        private void Update()
        {
            // 플레이어 이동 호출
            MovePlayer();
        }

        private void MovePlayer()
        {
            // 이동 벡터를 정규화하여 속도 곱하기
            Vector2 movement = movementInput * moveSpeed * Time.deltaTime;

            // 현재 위치에 이동 벡터를 더하여 이동
            transform.Translate(movement);
        }
    }
}