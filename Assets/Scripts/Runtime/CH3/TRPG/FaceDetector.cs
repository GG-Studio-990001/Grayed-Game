using UnityEngine;

namespace Runtime.CH3.DnD
{
    public class FaceDetector : MonoBehaviour
    {
        [SerializeField] private DiceRoll _dice;

        private void OnTriggerStay(Collider other)
        {
            Debug.Log("_dice.name");
            if (other.CompareTag(_dice.name) && _dice.GetComponent<Rigidbody>().velocity == Vector3.zero)
            {
                Debug.Log("Detect");
                _dice.DiceFaceNum = int.Parse(other.name);
            }
        }
    }
}