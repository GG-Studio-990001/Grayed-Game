using UnityEngine;

namespace Runtime.CH2.Main
{
    public class CH2Controller : MonoBehaviour
    {
        [SerializeField] private TurnController _turnController;

        private void Start()
        {
            if (Managers.Data.Chapter == 1)
                Managers.Data.Chapter = 2;

            _turnController.GetInitialLocation();
        }
    }
}