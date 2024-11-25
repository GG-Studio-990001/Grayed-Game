using Runtime.CH2.Dialogue;
using UnityEngine;

namespace Runtime.CH2
{
    public class DialogueAutoAdvanceChecker : MonoBehaviour
    {
        [SerializeField] private CH2Dialogue _dialogue;

        void OnEnable()
        {
            _dialogue.StartAutoDialogue();
        }
    }
}