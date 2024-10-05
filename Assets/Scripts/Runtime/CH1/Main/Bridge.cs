using UnityEngine;

namespace Runtime.CH1.Main
{
    public class Bridge : MonoBehaviour
    {
        [SerializeField] private GameObject[] _bridges;

        public void CheckBridge()
        {
            if (Managers.Data.CH1.Scene >= 3)
            {
                if (Managers.Data.CH1.SLGBridgeRebuild == false)
                {
                    ActiveBrokenBridge();
                }
                else
                {
                    ActiveNormalBridge();
                }
            }
            else
            {
                ActiveNormalBridge();
            }
        }

        private void ActiveNormalBridge()
        {
            _bridges[0].SetActive(true);
            _bridges[1].SetActive(false);
        }

        public void ActiveBrokenBridge()
        {
            _bridges[0].SetActive(false);
            _bridges[1].SetActive(true);
        }
    }
}