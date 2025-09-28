using UnityEngine;

namespace Runtime.CH4
{
    public class DetectGoal : MonoBehaviour
    {
        [SerializeField] private CH4Stage2GameController gameController;
        [SerializeField] private GameObject _goalTxt;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Goal"))
            {
                if (gameController.NowLevel == 1)
                {
                    gameController.StartLevel2();
                }
                else
                {
                    _goalTxt.SetActive(true);
                }
            }
        }

    }
}