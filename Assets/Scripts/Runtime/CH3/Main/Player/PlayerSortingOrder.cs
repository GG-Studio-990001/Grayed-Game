using UnityEngine;

namespace Runtime.CH3.Main
{
    public class PlayerSortingOrder : SortingOrderObject
    {
        [Header("Overlap Detection")]
        [SerializeField] private float detectionRadius = 1f;
        [SerializeField] private LayerMask objectLayer; // 감지할 오브젝트 레이어
    
        private Vector3 _lastPosition;
        private Vector2Int _lastGridPosition;
        private PlayerGrid _playerGrid;
        private GridSystem _gridManager;

        protected override void Awake()
        {
            base.Awake();
            _lastPosition = transform.position;
            _playerGrid = GetComponent<PlayerGrid>();
            _gridManager = GridSystem.Instance;
        }

        private void Start()
        {
            // 초기 그리드 위치 기반 SortingOrder 설정
            if (_playerGrid != null && _gridManager != null)
            {
                Vector2Int gridPos = _playerGrid.GridPosition;
                if (gridPos != Vector2Int.zero)
                {
                    _lastGridPosition = gridPos;
                    UpdateSortingOrderFromGrid(gridPos);
                }
            }
        }

        protected void LateUpdate()
        {
            // 그리드 위치가 변경되었는지 확인
            if (_playerGrid != null && _gridManager != null)
            {
                Vector2Int currentGridPos = _gridManager.WorldToGridPosition(transform.position);
                if (_gridManager.IsValidGridPosition(currentGridPos) && currentGridPos != _lastGridPosition)
                {
                    _lastGridPosition = currentGridPos;
                    UpdateSortingOrderFromGrid(currentGridPos);
                }
            }
            
            // 위치가 변경되었을 때만 주변 오브젝트 체크
            if (_lastPosition != transform.position)
            {
                CheckNearbyObjects();
                _lastPosition = transform.position;
            }
        }

        private void UpdateSortingOrderFromGrid(Vector2Int gridPos)
        {
            // 그리드 y 좌표 기준으로 baseOrder 설정
            // 뒤(y가 작을수록)일수록 더 높은 정렬이 되도록 부호 반전
            SetBaseOrder(-gridPos.y);
        }

        private void CheckNearbyObjects()
        {
            // 주변 오브젝트 감지 후, 가장 가까운 대상만 기준으로 처리
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, objectLayer);

            SortingOrderObject closest = null;
            float closestSqrDist = float.MaxValue;

            foreach (var col in colliders)
            {
                // SortingOrderObject는 자식 오브젝트에 있을 수 있으므로 부모에서도 찾기
                var sortingObj = col.GetComponent<SortingOrderObject>() ?? col.GetComponentInParent<SortingOrderObject>();
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
                // SpriteRenderer는 자식 오브젝트에 있을 수 있으므로 GetComponentInChildren 사용
                var objSr = closest.GetComponent<SpriteRenderer>() ?? closest.GetComponentInChildren<SpriteRenderer>();
                var selfSr = GetComponent<SpriteRenderer>();
                if (objSr != null && selfSr != null)
                {
                    Vector3 directionToClosest = closest.transform.position - transform.position;
                    // 오브젝트가 플레이어 앞(z+)에 있으면 오브젝트보다 한 단계 위로, 뒤(z-)면 한 단계 아래로
                    int delta = directionToClosest.z > 0f ? 1 : -1;
                    selfSr.sortingOrder = objSr.sortingOrder + delta;
                }
            }
            else
            {
                // 주변 오브젝트가 없으면 그리드 기반 baseOrder 사용
                UpdateSortingOrder();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}