using UnityEngine;

namespace Runtime.CH3.Main.Factory
{
    public class Mineral : InteractableGridObject
    {
        [SerializeField] private string mineralType;
        [SerializeField] private int remainingResources = 3;
        
        public override void OnInteract(GameObject interactor)
        {
            if (!canInteract) return;
            
            remainingResources--;
            Debug.Log($"채광 중: {mineralType}, 남은 자원: {remainingResources}");
            
            if (remainingResources <= 0)
            {
                Remove();
            }
        }
    }
}