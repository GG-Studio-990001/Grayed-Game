using System.Collections.Generic;
using UnityEngine;

namespace Runtime.CH3.Main
{
    public class PlayerSortingOrder : SortingOrderObject
    {
        [Header("Overlap Detection")]
        [SerializeField] private float detectionRadius = 1f;
        [SerializeField] private LayerMask objectLayer; // 감지할 오브젝트 레이어
    
        private Vector3 _lastPosition;

        protected override void Awake()
        {
            base.Awake();
            _lastPosition = transform.position;
        }

        protected void LateUpdate()
        {
            // 위치가 변경되었을 때만 체크
            if (_lastPosition != transform.position)
            {
                CheckNearbyObjects();
                _lastPosition = transform.position;
            }
        }

        private void CheckNearbyObjects()
        {
            // 주변 오브젝트 감지 후, 가장 가까운 대상만 기준으로 처리
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, objectLayer);

            SortingOrderObject closest = null;
            float closestSqrDist = float.MaxValue;

            foreach (var col in colliders)
            {
                var sortingObj = col.GetComponent<SortingOrderObject>();
                if (sortingObj == null) continue;

                float sqrDist = (sortingObj.transform.position - transform.position).sqrMagnitude;
                if (sqrDist < closestSqrDist)
                {
                    closestSqrDist = sqrDist;
                    closest = sortingObj;
                }
            }

            if (closest != null)
            {
                var objSr = closest.GetComponent<SpriteRenderer>();
                var selfSr = GetComponent<SpriteRenderer>();
                if (objSr != null && selfSr != null)
                {
                    Vector3 directionToClosest = closest.transform.position - transform.position;
                    // 오브젝트가 플레이어 앞(z+)에 있으면 오브젝트보다 한 단계 위로, 뒤(z-)면 한 단계 아래로
                    int delta = directionToClosest.z > 0f ? 1 : -1;
                    selfSr.sortingOrder = objSr.sortingOrder + delta;
                }
            }
        }

        // 디버그용 기즈모
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}