using UnityEngine;

namespace Runtime.CH4
{
    public class HammerHead : MonoBehaviour
    {
        [Header("플레이어 Rigidbody2D")]
        [SerializeField] private Rigidbody2D playerRb;

        [Header("반작용 힘 크기")]
        [SerializeField] private float reboundForce = 10f;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // 1. 충돌 위치 방향 벡터 구하기
            Vector2 contactPoint = collision.GetContact(0).point;
            Vector2 hammerHeadPos = transform.position;
            Vector2 toPlayer = (Vector2)playerRb.position - hammerHeadPos;

            // 2. 방향 벡터 정규화해서 force 적용
            Vector2 forceDir = toPlayer.normalized;

            // 3. 플레이어에게 반작용 힘 적용 (Impulse)
            playerRb.AddForce(forceDir * reboundForce, ForceMode2D.Impulse);

            // 4. (선택) 디버그
            Debug.Log($"충돌 반작용 적용! 방향: {forceDir}, 힘: {reboundForce}");
        }
    }
}
