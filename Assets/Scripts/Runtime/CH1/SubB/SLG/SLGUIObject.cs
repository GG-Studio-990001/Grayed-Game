using Runtime.CH1.Main.Dialogue;
using Runtime.CH1.Main.Interface;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

namespace Runtime.CH1.SubB.SLG
{
    public class SlguiObject : MonoBehaviour, IInteractive
    {
        public UnityEvent onInteract;

        public bool Interact(Vector2 direction = default)
        {
            onInteract?.Invoke();

            Ch1DialogueController Ch1DC = GameObject.FindObjectOfType<Ch1DialogueController>();
            if (Ch1DC != null)
            {
                if (Managers.Data.Scene < 4)
                {
                    // 임시로 막아두기
                    Ch1DC.StartCh1MainDialogue("SLG_Block");
                }
                else
                {
                    Ch1DC.StartCh1MainDialogue("SLG_Start");
                }
            }
            return true;
        }
    
    }
}