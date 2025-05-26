using Runtime.ETC;
using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class NpcItem : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out ArioReward ario))
            {
                gameObject.SetActive(false);
                Managers.Sound.Play(Sound.SFX, "SuperArio/CH2_SUB_SFX_31");
            }
        }
    }
}