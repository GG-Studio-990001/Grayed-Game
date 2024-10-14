using Runtime.ETC;
using UnityEngine;
using SLGDefines;

namespace Runtime.CH1.Main
{
    public class SLGPack : MonoBehaviour
    {
        public void ActiveLuckyPack()
        {
            gameObject.SetActive(Managers.Data.CH1.SLGProgressData == SLGProgress.None);
        }
    }
}