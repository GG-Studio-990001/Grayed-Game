using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

namespace Runtime.CH3.Main
{
    public class InteractionManager : MonoBehaviour
    {
        [SerializeField] private float checkRadius = 3f;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private UnityEngine.UI.Image holdGaugeUI; // [Deprecated] 전역 게이지(미사용)
        
        private Transform playerTransform;
        private IHoldInteractable currentHold;
        private float holdElapsed;
        [SerializeField] private Camera worldCamera;

        private void Start()
        {
            playerTransform = transform;
            if (worldCamera == null) worldCamera = Camera.main;
        }

        public void TryInteract()
        {
            Collider[] colliders = Physics.OverlapSphere(playerTransform.position, checkRadius, interactableLayer);
    
            var interactable = colliders
                .Select(c => c.GetComponent<IInteractable>())
                .Where(i => i != null && i.CanInteract)
                .OrderBy(i =>
                {
                    var mb = i as MonoBehaviour;
                    return mb != null 
                        ? Vector3.Distance(playerTransform.position, mb.transform.position) 
                        : float.MaxValue;
                })
                .FirstOrDefault();

            if (interactable != null)
            {
                var mb = interactable as MonoBehaviour;
                if (mb != null)
                {
                    float distance = Vector3.Distance(playerTransform.position, mb.transform.position);
                    if (distance <= interactable.InteractionRange)
                    {
                        interactable.OnInteract(gameObject);
                    }
                }
            }
        }

        // 길게 누르기 진입
        public void BeginHold()
        {
            if (currentHold != null) return;
            var interactable = FindNearest<IHoldInteractable>();
            if (interactable == null) return;

            var mb = interactable as MonoBehaviour;
            if (mb == null) return;
            if (Vector3.Distance(playerTransform.position, mb.transform.position) > interactable.InteractionRange) return;

            currentHold = interactable;
            
            // 게이지 값을 유지하면서 진행률 맞추기
            float currentGaugeValue = 0f;
            if (interactable is Ore ore)
            {
                currentGaugeValue = ore.GetCurrentGaugeValue();
            }
            else if (interactable is Breakable breakable)
            {
                currentGaugeValue = breakable.GetCurrentGaugeValue();
            }
            holdElapsed = currentGaugeValue * interactable.HoldSeconds;
            
            currentHold.OnHoldStart(gameObject);
        }

        // 유지 중 호출
        public void UpdateHold()
        {
            if (currentHold == null) return;

            // 범위 이탈 시 즉시 취소
            var targetMb = currentHold as MonoBehaviour;
            if (targetMb == null)
            {
                CancelHold();
                return;
            }
            float currentDistance = Vector3.Distance(playerTransform.position, targetMb.transform.position);
            if (currentDistance > currentHold.InteractionRange)
            {
                CancelHold();
                return;
            }

            holdElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(holdElapsed / currentHold.HoldSeconds);
            // 대상에게 진행률 전달
            currentHold.OnHoldProgress(gameObject, t);
            if (t >= 1f)
            {
                currentHold.OnHoldComplete(gameObject);
                EndHoldInternal();
            }
        }

        // 취소
        public void CancelHold()
        {
            if (currentHold == null) return;
            // OnHoldProgress를 호출하지 않아서 게이지 값이 유지됨
            currentHold.OnHoldCancel(gameObject);
            EndHoldInternal();
        }

        private void EndHoldInternal()
        {
            currentHold = null;
            holdElapsed = 0f;
            // UpdateGauge(0f); // 게이지 값을 유지하기 위해 제거
        }

        private void UpdateGauge(float normalized)
        {
            if (holdGaugeUI != null)
            {
                holdGaugeUI.fillAmount = normalized;
                holdGaugeUI.enabled = normalized > 0f && normalized < 1f;
            }
        }

        private T FindNearest<T>() where T : class, IInteractable
        {
            Collider[] colliders = Physics.OverlapSphere(playerTransform.position, checkRadius, interactableLayer);
            return colliders
                .Select(c => c.GetComponent<T>())
                .Where(i => i != null && i.CanInteract)
                .OrderBy(i =>
                {
                    var mb = i as MonoBehaviour;
                    return mb != null
                        ? Vector3.Distance(playerTransform.position, mb.transform.position)
                        : float.MaxValue;
                })
                .FirstOrDefault();
        }

        public void BeginHoldAtCursor(Vector2 screenPosition)
        {
            if (currentHold != null) return;
            if (worldCamera == null) worldCamera = Camera.main;
            if (worldCamera == null) return;

            Ray ray = worldCamera.ScreenPointToRay(screenPosition);
            
            // 모든 히트된 오브젝트를 가져와서 가장 적절한 것을 선택
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f, interactableLayer);
            
            IHoldInteractable bestHold = null;
            float bestDistance = float.MaxValue;
            
            foreach (var hit in hits)
            {
                var hold = hit.collider.GetComponent<IHoldInteractable>();
                if (hold == null || !hold.CanInteract) continue;

                var mb = hold as MonoBehaviour;
                if (mb == null) continue;
                
                float distanceToPlayer = Vector3.Distance(playerTransform.position, mb.transform.position);
                if (distanceToPlayer > hold.InteractionRange) continue;
                
                // 플레이어와의 거리가 더 가까운 오브젝트를 우선 선택
                if (distanceToPlayer < bestDistance)
                {
                    bestHold = hold;
                    bestDistance = distanceToPlayer;
                }
            }
            
            if (bestHold != null)
            {
                currentHold = bestHold;
                
                // 게이지 값을 유지하면서 진행률 맞추기
                float currentGaugeValue = 0f;
                if (bestHold is Ore ore)
                {
                    currentGaugeValue = ore.GetCurrentGaugeValue();
                }
                else if (bestHold is Breakable breakable)
                {
                    currentGaugeValue = breakable.GetCurrentGaugeValue();
                }
                holdElapsed = currentGaugeValue * bestHold.HoldSeconds;
                
                currentHold.OnHoldStart(gameObject);
            }
        }

    }
}