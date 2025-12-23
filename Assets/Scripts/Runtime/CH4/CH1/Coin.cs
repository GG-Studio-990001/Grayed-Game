using UnityEngine;

namespace CH4.CH1
{
    public class Coin : MonoBehaviour
    {
        // 코인에 부착
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<CoinController>().AddCoin();
                Destroy(gameObject);
            }
        }
    }
}