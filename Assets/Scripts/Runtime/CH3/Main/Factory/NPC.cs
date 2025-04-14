using UnityEngine;

namespace Runtime.CH3.Main
{
    public class NPC : InteractableGridObject
    {
        [SerializeField] private string npcName;
        [SerializeField] private string dialogueText;

        public override void OnInteract(GameObject interactor)
        {
            if (!canInteract) return;
            Debug.Log($"{npcName}: {dialogueText}");
            // 여기에 대화 시스템 연동 코드 추가
        }
    }
}