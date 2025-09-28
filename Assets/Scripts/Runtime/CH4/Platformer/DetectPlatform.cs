using UnityEngine;

namespace Runtime.CH4
{
    public class DetectPlatform : MonoBehaviour
    {
        [SerializeField] private CH4Stage2GameController gameController;
        [SerializeField] private SwitchLocation switchLocation;
        [SerializeField] private SwitchLocation2 switchLocation2;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Platform"))
            {
                PlatformInfo info = other.GetComponent<PlatformInfo>();
                if (info != null)
                {
                    if (gameController.NowLevel == 1)
                        switchLocation.Teleport(info.TargetLocation, info.Idx);
                    else
                        switchLocation2.Teleport(info.TargetLocation, info.Idx);
                }
            }
        }
    }
}