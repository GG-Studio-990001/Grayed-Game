using UnityEngine;
using Sound = Runtime.ETC.Sound;

namespace Runtime.CH2.Main
{
    public class CH2Controller : MonoBehaviour
    {
        [SerializeField] private TurnController _turnController;

        private void Start()
        {
            Managers.Sound.Play(Sound.BGM, "CH2/BGM_#1_02");

            _turnController.GetInitialLocation();
        }
    }
}