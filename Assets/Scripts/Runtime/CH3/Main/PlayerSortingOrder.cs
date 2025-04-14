using System.Collections.Generic;
using UnityEngine;

namespace Runtime.CH3.Main
{
    public class PlayerSortingOrder : SortingOrderObject
    {
        [Header("Overlap Detection")]
        [SerializeField] private float detectionRadius = 1f;
        [SerializeField] private LayerMask objectLayer; // 감지할 오브젝트 레이어
        [SerializeField] private int orderInFront = 10; // 앞에 있을 때의 추가 정렬값
    
        private readonly List<SortingOrderObject> _nearbyObjects = new List<SortingOrderObject>();
        private Vector3 _lastPosition;

        protected override void Awake()
        {
            base.Awake();
            _lastPosition = transform.position;
        }

        protected override void LateUpdate()
        {
            // 위치가 변경되었을 때만 체크
            if (_lastPosition != transform.position)
            {
                CheckNearbyObjects();
                _lastPosition = transform.position;
            }
        
            base.LateUpdate();
        }

        private void CheckNearbyObjects()
        {
            // 이전에 감지된 오브젝트들 초기화
            foreach (var obj in _nearbyObjects)
            {
                if (obj != null)
                {
                    obj.SetBaseOrder(0);
                }
            }
            _nearbyObjects.Clear();

            // 주변 오브젝트 감지
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, objectLayer);
        
            foreach (var col in colliders)
            {
                var sortingObj = col.GetComponent<SortingOrderObject>();
                if (sortingObj == null) continue;

                Vector3 directionToObject = col.transform.position - transform.position;
            
                // 플레이어가 오브젝트 뒤에 있는지 확인
                if (directionToObject.z > 0)
                {
                    // 플레이어가 오브젝트 뒤에 있으면 플레이어를 앞으로
                    SetBaseOrder(orderInFront);
                }
                else
                {
                    // 플레이어가 오브젝트 앞에 있으면 기본 정렬 순서 사용
                    SetBaseOrder(0);
                }

                _nearbyObjects.Add(sortingObj);
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