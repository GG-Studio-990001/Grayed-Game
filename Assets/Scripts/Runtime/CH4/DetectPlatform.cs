using UnityEngine;

namespace Runtime.CH4
{
    public class DetectPlatform : MonoBehaviour
    {
        [SerializeField] private SwitchLocation switchLocation;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Platform"))
            {
                PlatformInfo info = other.gameObject.GetComponent<PlatformInfo>();
                if (info != null)
                {
                    switchLocation.Teleport(info.TargetLocation);
                }
            }
        }
    }
}