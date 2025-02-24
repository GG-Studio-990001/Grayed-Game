using UnityEngine;

namespace Runtime.CH3.Main
{
    public class CH3Controller : MonoBehaviour
    {
        private void Start()
        {
            if (Managers.Data.Chapter == 2)
                Managers.Data.Chapter = 3;
        }
    }
}
