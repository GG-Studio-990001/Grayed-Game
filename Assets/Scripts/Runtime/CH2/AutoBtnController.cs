using Runtime.CH2.Dialogue;
using UnityEngine;

namespace Runtime.CH2
{
    public class AutoBtnController : MonoBehaviour
    {
        [SerializeField] private CH2Dialogue _dialogue;

        void OnEnable()
        {
            Debug.Log("일어남");
            _dialogue.StartAutoDialogue();
        }
    }
}