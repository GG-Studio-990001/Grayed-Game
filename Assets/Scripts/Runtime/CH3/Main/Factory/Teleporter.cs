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
        [SerializeField] private bool autoTeleport = true; // 자동 텔레포트 (닿기만 하면 작동)

        [Header("Visual Effects")]
        [SerializeField] private Color teleporterColor = Color.cyan;
        [SerializeField] private float pulseSpeed = 1f;
        [SerializeField] private float pulseIntensity = 0.2f;

        private SpriteRenderer spriteRenderer;
        private bool isTeleporting = false;
        private float originalAlpha;

        public override void Initialize(Vector2Int gridPos)
        {
            base.Initialize(gridPos);
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (spriteRenderer != null)
            {
                originalAlpha = spriteRenderer.color.a;
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

        // 자동 텔레포트를 위한 OnTriggerEnter
        private void OnTriggerEnter(Collider other)
        {
            if (!autoTeleport || isTeleporting || connectedTeleporter == null) return;

            // 플레이어인지 확인
            if (other.CompareTag("Player"))
            {
                Debug.Log($"플레이어가 텔레포트에 닿았습니다: {gameObject.name}");
                StartCoroutine(TeleportPlayer(other.gameObject));
            }
        }

        public override void OnInteract(GameObject interactor)
        {
            if (!canInteract || isTeleporting || connectedTeleporter == null) return;

            // 플레이어인지 확인
            if (interactor.CompareTag("Player"))
            {
                StartCoroutine(TeleportPlayer(interactor));
            }
        }

        private System.Collections.IEnumerator TeleportPlayer(GameObject player)
        {
            isTeleporting = true;
            canInteract = false;

            if (showDebugInfo)
            {
                Debug.Log($"텔레포트 시작: {gameObject.name} -> {connectedTeleporter.gameObject.name}");
            }

            // 텔레포트 효과 (페이드 아웃)
            yield return StartCoroutine(FadeOutEffect());

            // 플레이어를 연결된 텔레포트로 이동
            Vector3 targetPosition = connectedTeleporter.transform.position;
            player.transform.position = targetPosition;

            // 연결된 텔레포트의 상호작용 비활성화 (단방향 방지)
            if (isOneWay)
            {
                connectedTeleporter.canInteract = false;
            }

            // 텔레포트 효과 (페이드 인)
            yield return StartCoroutine(connectedTeleporter.FadeInEffect());

            // 지연시간 후 상호작용 재활성화
            yield return new WaitForSeconds(teleportDelay);
            
            isTeleporting = false;
            canInteract = true;

            if (showDebugInfo)
            {
                Debug.Log($"텔레포트 완료: {player.name} -> {connectedTeleporter.gameObject.name}");
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
            // 펄스 효과
            if (spriteRenderer != null && !isTeleporting)
            {
                float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity + 1f;
                Color currentColor = spriteRenderer.color;
                currentColor.a = originalAlpha * pulse;
                spriteRenderer.color = currentColor;
            }
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