using UnityEngine;

namespace Runtime.CH1.SubB.SLG
{
    public class SLGBottomUIHider : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _bottomUI;
        [SerializeField] private CanvasGroup _lineView;

        void Update()
        {
            _bottomUI.alpha = 1 - _lineView.alpha;
        }
    }
}
