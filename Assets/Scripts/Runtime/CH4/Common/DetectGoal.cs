using UnityEngine;
using Runtime.ETC;

namespace Runtime.CH4
{
    public class DetectGoal : MonoBehaviour
    {
        [SerializeField] private CH4S2GameController gameController;
        [SerializeField] private GameObject _goalTxt;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Goal"))
            {
                Managers.Sound.Play(Sound.SFX, "Pacmom/Pacmom_SFX_14");

                if (gameController.NowLevel == 1)
                {
                    gameController.StartLevel(2);
                }
                else
                {
                    _goalTxt.SetActive(true);
                }
            }
        }

    }
}