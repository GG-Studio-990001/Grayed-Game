using UnityEngine;

namespace Runtime.CH4
{
    public class DetectGoal : MonoBehaviour
    {
        [SerializeField] private GameObject _goalTxt;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Goal"))
            {
                _goalTxt.SetActive(true);
            }
        }

    }
}