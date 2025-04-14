using UnityEngine;

namespace Runtime.CH3.Main
{
    public interface IInteractable
    {
        float InteractionRange { get; }
        bool CanInteract { get; }
        void OnInteract(GameObject interactor);
    }
}