using UnityEngine;
using Sound = Runtime.ETC.Sound;

namespace Runtime.CH2.Main
{
    public class CH2Controller : MonoBehaviour
    {
        void Start()
        {
            Managers.Sound.Play(Sound.BGM, "CH2/BGM_#1_02");
        }
    }
}