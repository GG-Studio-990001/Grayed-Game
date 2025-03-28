using UnityEngine;

namespace Runtime.CH3.DnD
{
    public class FaceDetector : MonoBehaviour
    {
        [SerializeField] private DiceRoll _dice;

        private void OnTriggerStay(Collider other)
        {
            if (_dice.GetComponent<Rigidbody>().velocity == Vector3.zero)
            {
                Debug.Log("Detect");
                _dice.DiceFaceNum = int.Parse(other.name);
            }
        }
    }
}