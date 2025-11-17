using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using Runtime.ETC;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 연출구역 - 플레이어가 진입 시 타임라인을 활성화하는 오브젝트
    /// </summary>
    public class EventArea : GridObject
    {
        [Header("Event Settings")]
        [SerializeField] private GameObject timelineObject;
        [SerializeField] private bool triggerOnce = true;
        [SerializeField] private float triggerRadius = 1f;

        [Header("Event Callbacks")]
        [SerializeField] private UnityEvent onTriggered = new UnityEvent();

        [Header("Linked Events")]
        [SerializeField] private List<EventArea> linkedEvents = new List<EventArea>();

        [Header("Visual Settings")]
        [SerializeField] private Color gizmoColor = Color.blue;
        [SerializeField] private bool showGizmos = true;

        private bool hasTriggered = false;
        private bool isDisabledByLinkedEvent = false;
        private bool timelineInitialActiveState;
        private bool timelineStateCaptured = false;
        private Transform playerTransform;

        public event Action<EventArea> Triggered;

        protected override void Awake()
        {
            base.Awake();
            CaptureTimelineInitialState();
        }

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
            // 먼저 자기 자신에서 찾기
            Collider collider = GetComponent<Collider>();
            
            // 없으면 자식에서 찾기 (리소스 분리 구조 지원)
            if (collider == null)
            {
                collider = GetComponentInChildren<Collider>();
            }
            
            // GridObject가 있으면 spriteTransform에서 찾기
            if (collider == null)
            {
                var spriteTransform = base.GetSpriteTransform();
                if (spriteTransform != null && spriteTransform != transform)
                {
                    collider = spriteTransform.GetComponent<Collider>();
                }
            }
            
            // 여전히 없으면 최상단 오브젝트에 추가
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
            if (playerTransform == null || isDisabledByLinkedEvent || hasTriggered && triggerOnce) return;

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
            if (!CanTrigger()) return;

            // 타임라인 활성화
            if (timelineObject != null)
            {
                timelineObject.SetActive(true);

                // 타임라인 컴포넌트가 있다면 재생
                var timeline = timelineObject.GetComponent<PlayableDirector>();
                if (timeline != null)
                {
                    timeline.Play();
                }
            }

            hasTriggered = true;

            InvokeCallbacks();
            DisableLinkedEvents();
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
            isDisabledByLinkedEvent = false;

            if (!timelineStateCaptured)
            {
                CaptureTimelineInitialState();
            }

            if (timelineObject != null)
            {
                timelineObject.SetActive(timelineInitialActiveState);
            }

            EnableCollider();
        }

        private bool CanTrigger()
        {
            if (isDisabledByLinkedEvent) return false;
            if (hasTriggered && triggerOnce) return false;
            return true;
        }

        private void InvokeCallbacks()
        {
            Triggered?.Invoke(this);
            onTriggered?.Invoke();
        }

        private void DisableLinkedEvents()
        {
            if (linkedEvents == null || linkedEvents.Count == 0) return;

            foreach (EventArea linkedEvent in linkedEvents)
            {
                if (linkedEvent == null || linkedEvent == this) continue;
                linkedEvent.DisableFromLinkedEvent(this);
            }
        }

        private void DisableFromLinkedEvent(EventArea source)
        {
            if (isDisabledByLinkedEvent) return;

            isDisabledByLinkedEvent = true;
            hasTriggered = true;

            StopTimeline();
            DisableCollider();
        }

        private void CaptureTimelineInitialState()
        {
            if (timelineStateCaptured) return;

            timelineInitialActiveState = timelineObject != null && timelineObject.activeSelf;
            timelineStateCaptured = true;
        }

        private void StopTimeline()
        {
            if (timelineObject == null) return;

            var timeline = timelineObject.GetComponent<PlayableDirector>();
            if (timeline != null)
            {
                timeline.Stop();
            }

            timelineObject.SetActive(false);
        }

        private void DisableCollider()
        {
            // 먼저 자기 자신에서 찾기
            Collider collider = GetComponent<Collider>();
            
            // 없으면 자식에서 찾기
            if (collider == null)
            {
                collider = GetComponentInChildren<Collider>();
            }
            
            // GridObject가 있으면 spriteTransform에서 찾기
            if (collider == null)
            {
                var spriteTransform = base.GetSpriteTransform();
                if (spriteTransform != null && spriteTransform != transform)
                {
                    collider = spriteTransform.GetComponent<Collider>();
                }
            }
            
            if (collider != null)
            {
                collider.enabled = false;
            }
        }

        private void EnableCollider()
        {
            // 먼저 자기 자신에서 찾기
            Collider collider = GetComponent<Collider>();
            
            // 없으면 자식에서 찾기
            if (collider == null)
            {
                collider = GetComponentInChildren<Collider>();
            }
            
            // GridObject가 있으면 spriteTransform에서 찾기
            if (collider == null)
            {
                var spriteTransform = base.GetSpriteTransform();
                if (spriteTransform != null && spriteTransform != transform)
                {
                    collider = spriteTransform.GetComponent<Collider>();
                }
            }
            
            if (collider == null)
            {
                SetupCollider();
                return;
            }

            collider.enabled = true;
        }
        public void PlaySamepleBGM()
        {
            Managers.Sound.Play(Sound.SFX, "Dancepace/New/CH3_SUB_BGM_WAVE_Intro_Outro", true);
        }
    }
}
