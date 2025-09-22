using UnityEngine;

namespace Runtime.CH3.Main
{
    public class NPC : InteractableGridObject
    {
        [SerializeField] private string npcName;
        [SerializeField] private string dialogueText;
        
        [Header("Grid Position Settings")]
        [SerializeField] private bool initializeToGridPosition = true; // 초기 스폰을 GridPosition에 맞춤

        public override void Initialize(Vector2Int gridPos)
        {
            base.Initialize(gridPos);
            
            // 초기 배치: 인스펙터의 gridPosition을 우선 적용
            if (initializeToGridPosition && gridManager != null)
            {
                // BaseGridObject가 gridPosition을 유지하고 있으므로 그 좌표로 월드 위치 재배치
                Vector2Int targetGrid = gridPosition == Vector2Int.zero ? gridPos : gridPosition;
                Vector3 world = GetWorldPositionForGrid(targetGrid);
                transform.position = world;
                UpdateGridPosition();
            }
        }

        public override void OnInteract(GameObject interactor)
        {
            if (!canInteract) return;
            Debug.Log($"{npcName}: {dialogueText}");
            // 여기에 대화 시스템 연동 코드 추가
        }
    }
}