using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class RewardPipe : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out ArioReward ario))
            {
                ario.ExitReward();
            }
        }
    }
}
