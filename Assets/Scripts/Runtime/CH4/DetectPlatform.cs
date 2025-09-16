using UnityEngine;

namespace Runtime.CH4
{
    public class DetectPlatform : MonoBehaviour
    {
        [SerializeField] private SwitchLocation switchLocation;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Platform"))
            {
                PlatformInfo info = other.GetComponent<PlatformInfo>();
                if (info != null)
                {
                    switchLocation.Teleport(info.TargetLocation, info.Idx);
                }
            }
        }
    }
}