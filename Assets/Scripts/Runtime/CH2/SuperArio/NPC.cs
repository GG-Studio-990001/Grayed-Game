using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH2.SuperArio
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] private DialogueRunner _dialogueRunner;
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out ArioReward ario))
            {
                _dialogueRunner.StartDialogue("NPC1");
            }
        }
    }
}