using UnityEngine;
using UnityEngine.Events;

namespace Runtime.CH1.Main.Stage
{
    public class Stage : MonoBehaviour
    {
        [field:SerializeField] public GameObject StageObject { get; set; }
        [field: SerializeField] public int StageNumber { get; set; }

        [Header("Stage Extension")]
        [SerializeField] private PolygonCollider2D confiner2D;
        [SerializeField] private StageMover[] stageMovers;
        public UnityEvent onStageEnable;
        
        public void StageSettings(Ch1StageChanger stageChanger)
        {
            if (stageMovers is null) return;
            
            foreach (var stageMover in stageMovers)
            {
                stageMover.StageChanger = stageChanger;
            }
        }
        
        public PolygonCollider2D GetStageCollider()
        {
            return confiner2D;
        }
        
        public bool IsActivate()
        {
            return StageObject.activeSelf;
        }
        
        public void Enable()
        {
            StageObject.SetActive(true);
            onStageEnable?.Invoke();
        }
        
        public void Disable()
        {
            StageObject.SetActive(false);
        }
    }
}
