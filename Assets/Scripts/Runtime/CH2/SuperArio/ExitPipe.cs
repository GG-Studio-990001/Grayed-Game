using Runtime.ETC;
using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class ExitPipe : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out ArioStore ario))
            {
                Managers.Sound.Play(Sound.SFX, "SuperArio/CH2_SUB_SFX_14");
                ario.ExitStore();
            }
        }
    }
}
