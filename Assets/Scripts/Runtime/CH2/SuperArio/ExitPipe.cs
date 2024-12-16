using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class ExitPipe : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out ArioStore ario))
            {
                ario.ExitStore();
            }
        }
    }
}
