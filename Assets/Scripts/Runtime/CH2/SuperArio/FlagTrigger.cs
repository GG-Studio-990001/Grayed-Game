using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class FlagTrigger : MonoBehaviour
    {
        [SerializeField] private bool isTop;
        [SerializeField] private GameObject otherTrigger;
        [SerializeField] private Transform doorTr;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Ario ario))
            {
                otherTrigger.SetActive(false);
                if (isTop)
                {
                    // 코인 50개 추가
                }
                
                // isplay 멈춤
                ArioManager.instance.TouchFlag();
                StartCoroutine(ario.RewardAnimation(doorTr));
            }
        }
    }
}
