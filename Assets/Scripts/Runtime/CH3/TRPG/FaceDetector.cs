using UnityEngine;

namespace Runtime.CH3.DnD
{
    public class FaceDetector : MonoBehaviour
    {
        [SerializeField] private DiceRoll _dice;

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(_dice.name) && _dice.GetComponent<Rigidbody>().velocity == Vector3.zero)
            {
                _dice.DiceFaceNum = (int.Parse(other.name) == 999 ? 0 : int.Parse(other.name));
            }
        }
    }
}