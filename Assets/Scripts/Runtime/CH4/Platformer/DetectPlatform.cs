using UnityEngine;

namespace Runtime.CH4
{
    public class DetectPlatform : MonoBehaviour
    {
        [SerializeField] private CH4Stage2GameController gameController;
        [SerializeField] private SwitchLocation[] switchLocation;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Platform"))
            {
                PlatformInfo info = other.GetComponent<PlatformInfo>();
                if (info != null)
                {
                    switchLocation[gameController.NowLevel - 1].Teleport(info.TargetLocation, info.Idx);
                }
            }
        }
    }
}