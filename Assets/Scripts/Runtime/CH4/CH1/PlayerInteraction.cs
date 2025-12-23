using UnityEngine;

namespace CH4.CH1
{
    // CH4.CH1의 코인, 먼지유령에 대한 인터렉션
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private CoinController _coinController;

        public void CollisionWithCoin()
        {
            // 코인과 부딪힘
            _coinController.AddCoin();
        }

        public void CollisionWithGhostDust()
        {
            // 먼지와 부딪힘
            _coinController.LoseCoin();
            // TODO: 2초 동안 무적
        }
    }
}