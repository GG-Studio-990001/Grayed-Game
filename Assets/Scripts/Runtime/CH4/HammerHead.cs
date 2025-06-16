using UnityEngine;

namespace Runtime.CH4
{
    public class HammerHead : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D playerRb;
        [SerializeField] private float reboundForce = 10f;
        private Rigidbody2D rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log("충돌됨 with: " + collision.gameObject.name);

            if (collision.gameObject.CompareTag("Obstacle") && playerRb != null)
            {
                Vector2 contactPoint = collision.GetContact(0).point;
                Vector2 direction = ((Vector2)playerRb.position - contactPoint).normalized;
                playerRb.AddForce(direction * reboundForce, ForceMode2D.Impulse);
            }
        }
    }
}
