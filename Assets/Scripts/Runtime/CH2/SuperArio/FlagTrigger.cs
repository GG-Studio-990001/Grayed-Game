using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class FlagTrigger : MonoBehaviour
    {
        [SerializeField] private bool isTop;
        [SerializeField] private GameObject otherTrigger;
        [SerializeField] private Transform doorTr;

        private bool _once;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Ario ario) && !_once)
            {
                _once = true;
                otherTrigger.SetActive(false);
                if (isTop)
                {
                    // 코인 50개 추가
                    ArioManager.Instance.GetCoin(20);
                }
                
                // isplay 멈춤
                ArioManager.Instance.TouchFlag();
                StartCoroutine(ario.RewardEnterAnimation(doorTr, isTop));
            }
        }

        private void OnEnable()
        {
            _once = false;
        }
    }
}