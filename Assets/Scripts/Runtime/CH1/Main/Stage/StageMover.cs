using Runtime.ETC;
using UnityEngine;

namespace Runtime.CH1.Main.Stage
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class StageMover : MonoBehaviour
    {
        [SerializeField] private int moveStageNumber;
        [SerializeField] private Vector2 spawnPosition;
        
        public Ch1StageChanger StageChanger { get; set; }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(GlobalConst.PlayerStr))
            {
                StageChanger.SwitchStage(moveStageNumber, spawnPosition);
            }
        }
    }
}
