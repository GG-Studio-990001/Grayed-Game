using UnityEngine;
using Runtime.CH3.Main;

namespace Runtime.CH3.Main
{
    // Structure 역할(그리드 배치/차단) + 상호작용 가능 오브젝트
    public class Teleporter : Structure, IInteractable
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRange = 2f;
        private bool canInteract = true;

        [Header("Teleporter Settings")]
        [SerializeField] private Teleporter connectedTeleporter;
        [SerializeField] private float teleportDelay = 0.1f;
        [SerializeField] private bool showDebugInfo = true;
        [SerializeField] private float cooldownTime = 3f;

        private bool isTeleporting = false;
        private float lastTeleportTime = 0f;
        private bool isInCooldown = false;

        public override void Initialize(Vector2Int gridPos)
        {
            base.Initialize(gridPos);

            // no visual initialization

            // 연결된 텔레포트가 없으면 경고
            if (connectedTeleporter == null)
            {
                Debug.LogWarning($"텔레포트 {gameObject.name}에 연결된 텔레포트가 없습니다!");
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

        // IInteractable 구현
        public float InteractionRange => interactionRange;
        public bool CanInteract => canInteract;

        public void OnInteract(GameObject interactor)
        {
            // 텔레포트 중이거나 쿨다운 중이면 상호작용 불가
            if (!canInteract || isTeleporting || connectedTeleporter == null ||
                Time.time - lastTeleportTime < cooldownTime || isInCooldown) return;

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

            if (showDebugInfo)
            {
                Debug.Log($"텔레포트 시작: {gameObject.name} -> {connectedTeleporter.gameObject.name}");
            }

            // 플레이어를 연결된 텔레포터의 GridPosition에서 y만 -1 한 GridPosition으로 이동
            Vector3 targetPosition = Vector3.zero;
            if (GridSystem.Instance != null)
            {
                var gridMgr = GridSystem.Instance;
                Vector2Int destGrid = connectedTeleporter.GridPosition;
                destGrid.y -= 1; // GridPosition의 y를 -1
                targetPosition = gridMgr.GridToWorldPosition(destGrid);
                targetPosition.y = player.transform.position.y;
            }
            else
            {
                Debug.LogError("GridManager 없음");
            }

            var rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.position = targetPosition;
            }
            else
            {
                player.transform.position = targetPosition;
            }

            // 연결된 텔레포터에도 동일 쿨다운 적용
            connectedTeleporter.lastTeleportTime = Time.time;
            connectedTeleporter.canInteract = false;
            connectedTeleporter.isInCooldown = true;

            // 짧은 연출 지연만 주고, 재활성화는 쿨다운이 끝난 뒤에만
            yield return new WaitForSeconds(teleportDelay);

            isTeleporting = false;

            // 쿨다운 시간이 지난 후에만 완전히 재활성화
            StartCoroutine(EnableAfterCooldown());

            // 연결된 텔레포터도 동일 쿨다운 후 재활성화
            StartCoroutine(EnableConnectedTeleporterAfterCooldown());

            if (showDebugInfo)
            {
                Debug.Log($"텔레포트 완료: {player.name} -> {connectedTeleporter.gameObject.name}");
            }
        }

        private System.Collections.IEnumerator EnableAfterCooldown()
        {
            yield return new WaitForSeconds(cooldownTime);
            isInCooldown = false;
            canInteract = true; // 시작점 텔레포터도 다시 상호작용 가능하도록 설정
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

    }
}