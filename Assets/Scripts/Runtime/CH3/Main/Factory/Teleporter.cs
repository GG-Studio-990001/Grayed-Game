using UnityEngine;
using Runtime.CH3.Main;

namespace Runtime.CH3.Main
{
    public class Teleporter : InteractableGridObject
    {
        [Header("Teleporter Settings")]
        [SerializeField] private Teleporter connectedTeleporter;
        [SerializeField] private bool isOneWay = false; // 단방향 텔레포트
        [SerializeField] private float teleportDelay = 0.1f; // 텔레포트 지연시간
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private bool autoTeleport = false; // 자동 텔레포트 비활성화 (E키로만 작동)
        [SerializeField] private float cooldownTime = 3f; // 텔레포트 후 쿨다운 시간 (2초에서 3초로 증가)
        [SerializeField] private float safeDistance = 3f; // 텔레포트 후 안전 거리 (1.5에서 3으로 증가)

        [Header("Visual Effects")]
        [SerializeField] private Color teleporterColor = Color.cyan;
        [SerializeField] private float pulseSpeed = 1f;
        [SerializeField] private float pulseIntensity = 0.2f;
        [SerializeField] private Color highlightColor = Color.blue; // 파란 테두리 색상
        [SerializeField] private float highlightIntensity = 0.8f; // 테두리 강도

        private bool isTeleporting = false;
        private float originalAlpha;
        private bool isPlayerInRange = false; // 플레이어가 범위 내에 있는지 확인
        private Color originalColor; // 원래 색상 저장
        private float lastTeleportTime = 0f; // 마지막 텔레포트 시간
        private GameObject lastTeleportedPlayer = null; // 마지막으로 텔레포트한 플레이어
        private bool isInCooldown = false; // 쿨다운 상태 추적

        public override void Initialize(Vector2Int gridPos)
        {
            base.Initialize(gridPos);
            
            if (spriteRenderer != null)
            {
                originalAlpha = spriteRenderer.color.a;
                originalColor = teleporterColor;
                spriteRenderer.color = teleporterColor;
            }

            // 연결된 텔레포트가 없으면 경고
            if (connectedTeleporter == null)
            {
                Debug.LogWarning($"텔레포트 {gameObject.name}에 연결된 텔레포트가 없습니다!");
            }
            else
            {
                Debug.Log($"텔레포트 {gameObject.name}이 {connectedTeleporter.gameObject.name}에 연결되었습니다.");
            }

            // 콜라이더 확인
            Collider collider = GetComponent<Collider>();
            if (collider == null)
            {
                Debug.LogError($"텔레포트 {gameObject.name}에 Collider가 없습니다!");
            }
            else if (!collider.isTrigger)
            {
                Debug.LogWarning($"텔레포트 {gameObject.name}의 Collider가 Trigger가 아닙니다. Is Trigger를 체크해주세요!");
            }
        }

        public override void OnInteract(GameObject interactor)
        {
            // 텔레포트 중이거나 쿨다운 중이면 상호작용 불가
            if (!canInteract || isTeleporting || connectedTeleporter == null || 
                Time.time - lastTeleportTime < cooldownTime || isInCooldown) return;

            // 같은 플레이어가 연속으로 텔레포트하는 것을 방지
            if (lastTeleportedPlayer == interactor && Time.time - lastTeleportTime < cooldownTime * 2f) return;

            // 플레이어인지 확인
            if (interactor.CompareTag("Player"))
            {
                if (showDebugInfo)
                {
                    Debug.Log($"플레이어가 텔레포트를 시도합니다: {gameObject.name}");
                }
                StartCoroutine(TeleportPlayer(interactor));
            }
        }

        private System.Collections.IEnumerator TeleportPlayer(GameObject player)
        {
            isTeleporting = true;
            canInteract = false;
            isInCooldown = true;
            lastTeleportTime = Time.time; // 텔레포트 시작 시간 기록
            lastTeleportedPlayer = player; // 마지막 텔레포트한 플레이어 기록

            if (showDebugInfo)
            {
                Debug.Log($"텔레포트 시작: {gameObject.name} -> {connectedTeleporter.gameObject.name}");
            }

            // 텔레포트 효과 (페이드 아웃)
            yield return StartCoroutine(FadeOutEffect());

            // 플레이어를 연결된 텔레포트로 이동 (Y 값은 유지)
            Vector3 targetPosition = connectedTeleporter.transform.position;
            targetPosition.y = player.transform.position.y; // 플레이어의 Y 값 유지
            
            // 안전 거리만큼 떨어진 위치로 이동 (텔레포터에서 벗어나도록)
            // 두 텔레포터 사이의 방향을 계산
            Vector3 direction = (connectedTeleporter.transform.position - transform.position).normalized;
            if (direction == Vector3.zero)
            {
                direction = Vector3.right; // 기본 방향
            }
            targetPosition += direction * safeDistance;
            
            player.transform.position = targetPosition;

            // 연결된 텔레포트의 상호작용 비활성화 (단방향 방지)
            if (isOneWay)
            {
                connectedTeleporter.canInteract = false;
                connectedTeleporter.isInCooldown = true;
            }
            else
            {
                // 양방향 텔레포트의 경우 연결된 텔레포터도 쿨다운 적용
                connectedTeleporter.lastTeleportTime = Time.time;
                connectedTeleporter.canInteract = false;
                connectedTeleporter.isInCooldown = true;
                connectedTeleporter.lastTeleportedPlayer = player; // 연결된 텔레포터에도 같은 플레이어 기록
            }

            // 텔레포트 효과 (페이드 인)
            yield return StartCoroutine(connectedTeleporter.FadeInEffect());

            // 지연시간 후 상호작용 재활성화
            yield return new WaitForSeconds(teleportDelay);
            
            isTeleporting = false;
            canInteract = true;
            
            // 쿨다운 시간이 지난 후에만 완전히 재활성화
            StartCoroutine(EnableAfterCooldown());
            
            // 연결된 텔레포터는 쿨다운 시간이 지난 후에만 재활성화
            if (!isOneWay)
            {
                StartCoroutine(EnableConnectedTeleporterAfterCooldown());
            }

            if (showDebugInfo)
            {
                Debug.Log($"텔레포트 완료: {player.name} -> {connectedTeleporter.gameObject.name}");
            }
        }

        private System.Collections.IEnumerator EnableAfterCooldown()
        {
            yield return new WaitForSeconds(cooldownTime);
            isInCooldown = false;
        }

        private System.Collections.IEnumerator EnableConnectedTeleporterAfterCooldown()
        {
            yield return new WaitForSeconds(cooldownTime);
            if (connectedTeleporter != null)
            {
                connectedTeleporter.canInteract = true;
                connectedTeleporter.isInCooldown = false;
            }
        }

        private System.Collections.IEnumerator FadeOutEffect()
        {
            if (spriteRenderer == null) yield break;

            float duration = 0.3f;
            float elapsed = 0f;
            Color startColor = spriteRenderer.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(originalAlpha, 0f, elapsed / duration);
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }
        }

        public System.Collections.IEnumerator FadeInEffect()
        {
            if (spriteRenderer == null) yield break;

            float duration = 0.3f;
            float elapsed = 0f;
            Color startColor = spriteRenderer.color;
            startColor.a = 0f;
            spriteRenderer.color = startColor;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, originalAlpha, elapsed / duration);
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }
        }

        private void Update()
        {
            if (spriteRenderer == null) return;

            if (isTeleporting)
            {
                // 텔레포트 중일 때는 효과 없음
                return;
            }

            // 기본 펄스 효과
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity + 1f;
            Color currentColor = originalColor;
            currentColor.a = originalAlpha * pulse;
            spriteRenderer.color = currentColor;
        }

        private void OnDrawGizmos()
        {
            if (connectedTeleporter != null)
            {
                // 연결선 그리기
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, connectedTeleporter.transform.position);
                
                // 화살표 그리기
                Vector3 direction = (connectedTeleporter.transform.position - transform.position).normalized;
                Vector3 arrowStart = transform.position + direction * 0.5f;
                Vector3 arrowEnd = connectedTeleporter.transform.position - direction * 0.5f;
                
                Gizmos.DrawRay(arrowStart, direction * 0.3f);
                Gizmos.DrawRay(arrowStart, Quaternion.Euler(0, 0, 30) * -direction * 0.2f);
                Gizmos.DrawRay(arrowStart, Quaternion.Euler(0, 0, -30) * -direction * 0.2f);
            }
        }

        // 인스펙터에서 연결 설정
        public void SetConnectedTeleporter(Teleporter teleporter)
        {
            connectedTeleporter = teleporter;
        }

        // 단방향 설정
        public void SetOneWay(bool oneWay)
        {
            isOneWay = oneWay;
        }
    }
} 