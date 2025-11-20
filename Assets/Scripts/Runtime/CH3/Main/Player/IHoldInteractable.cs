using UnityEngine;

namespace Runtime.CH3.Main
{
    // 길게 누르기 상호작용 대상이 구현
    public interface IHoldInteractable : IInteractable
    {
        float HoldSeconds { get; }
        void OnHoldStart(GameObject interactor);
        void OnHoldProgress(GameObject interactor, float normalized01);
        void OnHoldCancel(GameObject interactor);
        void OnHoldComplete(GameObject interactor);
    }
}


