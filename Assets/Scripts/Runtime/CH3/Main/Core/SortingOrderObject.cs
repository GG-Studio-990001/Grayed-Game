using System.Collections;
using UnityEngine;

namespace Runtime.CH3.Main
{
    public class SortingOrderObject : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private int baseOrder = 0;
    
        protected virtual void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected virtual void UpdateSortingOrder()
        {
            if (spriteRenderer == null) return;
        
            int targetOrder = baseOrder;
        
            SmoothTransitionOrder(targetOrder);
            spriteRenderer.sortingOrder = targetOrder;

        }

        public void SetBaseOrder(int order)
        {
            baseOrder = order;
            UpdateSortingOrder();
        }
    
        // SortingOrderObject 클래스에 추가
        private void SmoothTransitionOrder(int targetOrder)
        {
            StartCoroutine(SmoothTransitionCoroutine(targetOrder));
        }

        private IEnumerator SmoothTransitionCoroutine(int targetOrder)
        {
            float duration = 0.2f; // 전환 시간
            float elapsed = 0;
            int startOrder = spriteRenderer.sortingOrder;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
        
                spriteRenderer.sortingOrder = Mathf.RoundToInt(
                    Mathf.Lerp(startOrder, targetOrder, t)
                );
        
                yield return null;
            }

            spriteRenderer.sortingOrder = targetOrder;
        }
    }
}