using UnityEngine;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 길게 누르기 상호작용 대상이 구현하는 인터페이스
    /// </summary>
    public interface IHoldInteractable : IInteractable
    {
        float HoldSeconds { get; }
        void OnHoldStart(GameObject interactor);
        void OnHoldProgress(GameObject interactor, float normalized01);
        void OnHoldCancel(GameObject interactor);
        void OnHoldComplete(GameObject interactor);
        
        /// <summary>
        /// 현재 게이지 값을 반환합니다 (0~1)
        /// </summary>
        float GetCurrentGaugeValue();
        
        /// <summary>
        /// 게이지 값을 초기화합니다
        /// </summary>
        void ResetGauge();
    }
}


