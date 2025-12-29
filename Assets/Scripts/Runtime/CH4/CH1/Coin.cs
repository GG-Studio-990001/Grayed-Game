using UnityEngine;
using System.Collections;

namespace CH4.CH1
{
    [RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D))]
    public class Coin : MonoBehaviour
    {
        // 코인에 부착
        private SpriteRenderer _spriteRenderer;
        private CircleCollider2D _collider;

        private void Start()
        {
            // 자식으로 옮겨서 껐다 켜는것도 ㄱㅊ긴 한데 구조 단순하니까 스크립트만 건들기로
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<CircleCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<PlayerInteraction>().CollisionWithCoin();
                CoinEaten();
            }
        }

        private void CoinEaten()
        {
            ActiveCoin(false);

            StartCoroutine(nameof(RespawnCoroutine));
        }

        private IEnumerator RespawnCoroutine()
        {
            float respawnTime = Random.Range(5f, 10f);
            yield return new WaitForSeconds(respawnTime);

            ActiveCoin(true);
            Debug.Log(name + " Respawn");
        }

        private void ActiveCoin(bool isActive)
        {
            _spriteRenderer.enabled = isActive;
            _collider.enabled = isActive;
        }
    }
}