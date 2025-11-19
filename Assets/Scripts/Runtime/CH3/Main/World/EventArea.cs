using UnityEngine;
using Runtime.CH3.Main;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 연출구역 - 플레이어가 진입 시 타임라인을 활성화하는 오브젝트
    /// </summary>
    public class EventArea : Structure
    {
        [Header("Event Settings")]
        [SerializeField] private GameObject timelineObject;
        [SerializeField] private bool triggerOnce = true;
        [SerializeField] private float triggerRadius = 1f;
        [SerializeField] private bool showDebugInfo = true;

        [Header("Visual Settings")]
        [SerializeField] private Color gizmoColor = Color.blue;
        [SerializeField] private bool showGizmos = true;

        private bool hasTriggered = false;
        private Transform playerTransform;

        public override void Initialize(Vector2Int gridPos)
        {
            base.Initialize(gridPos);
            
            // 플레이어 찾기
            FindPlayer();
            
            // 콜라이더 설정
            SetupCollider();
        }

        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning($"EventArea {gameObject.name}: Player를 찾을 수 없습니다!");
            }
        }

        private void SetupCollider()
        {
            Collider collider = GetComponent<Collider>();
            if (collider == null)
            {
                // Sphere Collider 추가
                collider = gameObject.AddComponent<SphereCollider>();
            }

            if (collider is SphereCollider sphereCollider)
            {
                sphereCollider.radius = triggerRadius;
                sphereCollider.isTrigger = true;
            }
        }

        private void Update()
        {
            if (playerTransform == null || hasTriggered && triggerOnce) return;

            // 거리 기반 트리거 (콜라이더가 없을 경우를 대비)
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= triggerRadius)
            {
                TriggerEvent();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                TriggerEvent();
            }
        }

        private void TriggerEvent()
        {
            if (hasTriggered && triggerOnce) return;

            if (showDebugInfo)
            {
                Debug.Log($"EventArea {gameObject.name}: 이벤트 트리거됨!");
            }

            // 타임라인 활성화
            if (timelineObject != null)
            {
                timelineObject.SetActive(true);
                
                // 타임라인 컴포넌트가 있다면 재생
                var timeline = timelineObject.GetComponent<UnityEngine.Playables.PlayableDirector>();
                if (timeline != null)
                {
                    timeline.Play();
                }
            }
            else
            {
                Debug.LogWarning($"EventArea {gameObject.name}: Timeline Object가 설정되지 않았습니다!");
            }

            hasTriggered = true;
        }

        private void OnDrawGizmos()
        {
            if (!showGizmos) return;

            // EventArea는 파란색으로 표시
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.1f, 0.4f);
            
            // 중앙에 E 표시
            Gizmos.color = Color.blue;
            Vector3 center = transform.position + Vector3.up * 0.2f;
            float size = 0.15f;
            // E 모양 그리기
            Gizmos.DrawLine(center + new Vector3(-size, 0, size), center + new Vector3(-size, 0, -size)); // 왼쪽 세로선
            Gizmos.DrawLine(center + new Vector3(-size, 0, size), center + new Vector3(size, 0, size)); // 위쪽 가로선
            Gizmos.DrawLine(center + new Vector3(-size, 0, 0), center + new Vector3(size, 0, 0)); // 중간 가로선
            Gizmos.DrawLine(center + new Vector3(-size, 0, -size), center + new Vector3(size, 0, -size)); // 아래쪽 가로선
            
            // 트리거 반경 표시 (기존 기능 유지)
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
            Gizmos.DrawWireSphere(transform.position, triggerRadius);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
            Gizmos.DrawSphere(transform.position, triggerRadius);
        }

        // 에디터에서 호출할 수 있는 메서드들
        public void SetTimelineObject(GameObject timeline)
        {
            timelineObject = timeline;
        }

        public void SetTriggerRadius(float radius)
        {
            triggerRadius = radius;
            SetupCollider();
        }

        public void ResetTrigger()
        {
            hasTriggered = false;
        }
    }
}
