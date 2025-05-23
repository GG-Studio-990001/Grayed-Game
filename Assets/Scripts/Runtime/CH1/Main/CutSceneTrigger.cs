using Runtime.ETC;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH1
{
    public class CutSceneTrigger : MonoBehaviour
    {
        [SerializeField] private DialogueRunner dialogueRunner;
        [SerializeField] private int _idx;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(GlobalConst.PlayerStr))
                return;

            switch (_idx)
            {
                case 1:
                    if (Managers.Data.CH1.Scene == 0)
                        dialogueRunner.StartDialogue("S1");
                    break;
                case 2:
                    if (Managers.Data.CH1.Scene == 2)
                        dialogueRunner.StartDialogue("S3");
                    break;
                case 3:
                    if (Managers.Data.CH1.Scene == 3 && Managers.Data.CH1.SceneDetail == 0)
                        dialogueRunner.StartDialogue("S3_1");
                    break;
                case 4:
                    if (Managers.Data.CH1.Scene == 3 && Managers.Data.CH1.SceneDetail == 1)
                        dialogueRunner.StartDialogue("S4");
                    break;
                default:
                    Debug.LogError("Invalid CutSceneTrigger Idx");
                    break;
            }
        }
    }
}