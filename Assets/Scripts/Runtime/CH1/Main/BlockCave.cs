using Runtime.ETC;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH1.Main
{
    public class BlockCave : MonoBehaviour
    {
        [SerializeField] private DialogueRunner dialogueRunner;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (Managers.Data.CH1.Scene >= 2)
            {
                gameObject.SetActive(false);
                return;
            }

            if (other.CompareTag(GlobalConst.PlayerStr))
            {
                dialogueRunner.StartDialogue("BlockCave");
            }
        }
    }
}