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
            {
                // 먼저 자기 자신에서 찾기
                spriteRenderer = GetComponent<SpriteRenderer>();
                
                // 없으면 자식에서 찾기 (리소스 분리 구조 지원)
                if (spriteRenderer == null)
                {
                    spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                }
                
                // GridObject가 있으면 spriteTransform 참조 사용
                if (spriteRenderer == null)
                {
                    // GridObject는 최상단 오브젝트에 있으므로 부모에서 찾기
                    var gridObject = GetComponentInParent<GridObject>();
                    if (gridObject != null)
                    {
                        // GridObject의 SpriteTransform을 통해 접근
                        var spriteTransform = GetSpriteTransformFromGridObject(gridObject);
                        if (spriteTransform != null)
                        {
                            spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// GridObject의 spriteTransform을 가져옵니다
        /// </summary>
        private Transform GetSpriteTransformFromGridObject(GridObject gridObject)
        {
            // GridObject의 public 메서드 사용
            return gridObject.GetSpriteTransform();
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