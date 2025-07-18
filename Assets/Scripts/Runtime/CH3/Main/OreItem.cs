using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace Runtime.CH3.Main
{
    public class OreItem : MonoBehaviour
    {
        [Header("Item Settings")]
        [SerializeField] private string itemId;
        [SerializeField] private int quantity = 1;

        [Header("Drop Animation Settings")]
        [SerializeField] private float initialHeight = 1f;
        [SerializeField] private float bounceHeight = 0.5f;
        [SerializeField] private float dropDuration = 0.5f;
        [SerializeField] private float groundOffset = 0.1f;
        [SerializeField] private Ease dropEase = Ease.OutBounce;

        [Header("Collection Settings")]
        [SerializeField] private float collectRadius = 3f;
        [SerializeField] private float moveSpeed = 15f;
        [SerializeField] private float accelerationRate = 4f;
        [SerializeField] private float collectDistance = 0.05f;
        [SerializeField] private float minScale = 0.3f;

        private bool isDropped = false;
        private bool isBeingCollected = false;
        private Transform playerTransform;
        private Vector3 velocity = Vector3.zero;

        public string ItemId => itemId;
        public int Quantity => quantity;

        private void Start()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (playerTransform == null)
            {
                Debug.LogWarning("Player not found! Make sure the player has the 'Player' tag.");
            }
        }

        private void Update()
        {
            if (isDropped && !isBeingCollected && playerTransform != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
                
                if (distanceToPlayer <= collectRadius)
                {
                    isBeingCollected = true;
                    StartCoroutine(CollectRoutine());
                }
            }
        }

        public void Drop(Vector3 startPosition, Vector3 dropDirection, float dropDistance)
        {
            // 시작 위치 (약간 위에서 시작)
            Vector3 spawnPos = startPosition + Vector3.up * initialHeight;
            transform.position = spawnPos;

            // 최종 착지 위치 계산 (바닥 높이 고려)
            Vector3 targetPos = startPosition + dropDirection * dropDistance;
            targetPos.y = startPosition.y + groundOffset;

            Sequence dropSequence = DOTween.Sequence();

            // 포물선 경로 생성
            Vector3[] path = new Vector3[]
            {
                spawnPos,
                Vector3.Lerp(spawnPos, targetPos, 0.5f) + Vector3.up * bounceHeight,
                targetPos
            };

            // 드롭 애니메이션
            dropSequence.Append(transform.DOPath(
                path,
                dropDuration,
                PathType.CatmullRom
            ).SetEase(dropEase));

            // 회전 효과
            dropSequence.Join(transform.DORotate(
                new Vector3(360f, 0f, 0f),
                dropDuration,
                RotateMode.FastBeyond360
            ));

            dropSequence.OnComplete(() => {
                isDropped = true;
                PlayLandingEffect();
            });
        }

        private void PlayLandingEffect()
        {
            // 착지 시 작은 스케일 효과
            transform.DOScale(
                transform.localScale * 1.2f,
                0.1f
            ).SetLoops(2, LoopType.Yoyo);

            // 파티클 시스템이 있다면 재생
            var particleSystem = GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
        }

        private IEnumerator CollectRoutine()
        {
            float currentSpeed = moveSpeed;
            float initialDistance = Vector3.Distance(transform.position, playerTransform.position);
            
            while (true)
            {
                if (playerTransform == null)
                    break;

                // 플레이어 방향으로 이동
                Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
                currentSpeed += accelerationRate * Time.deltaTime;
                
                // 부드러운 이동
                transform.position = Vector3.SmoothDamp(
                    transform.position,
                    playerTransform.position,
                    ref velocity,
                    0.3f,
                    currentSpeed
                );

                // 플레이어와의 거리 계산
                float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
                
                // 수집 거리에 도달하면 수집 완료
                if (distanceToPlayer < collectDistance)
                {
                    OnCollected();
                    break;
                }

                // 거리에 따른 크기 조절
                float distanceRatio = distanceToPlayer / initialDistance;
                float scale = Mathf.Lerp(minScale, 1f, distanceRatio);
                //transform.localScale = Vector3.one * scale;

                // 수집되는 동안 회전
                transform.Rotate(Vector3.up * 360f * Time.deltaTime, Space.World);

                yield return null;
            }
        }

        private void OnCollected()
        {
            // 수집 효과
            Sequence collectSequence = DOTween.Sequence();
            
            collectSequence.Append(transform.DOScale(Vector3.zero, 0.2f));
            collectSequence.Join(transform.DORotate(
                new Vector3(0, 720, 0),
                0.2f,
                RotateMode.FastBeyond360
            ));

            collectSequence.OnComplete(() => {
                // 여기에 아이템 획득 로직 추가 (인벤토리 시스템 등)
                Debug.Log($"Collected item: {itemId}, Quantity: {quantity}");
                Destroy(gameObject);
            });
        }

        // // 디버그용 기즈모
        // private void OnDrawGizmosSelected()
        // {
        //     // 수집 범위 표시
        //     Gizmos.color = Color.yellow;
        //     Gizmos.DrawWireSphere(transform.position, collectRadius);
        // }

        public void SetQuantity(int amount)
        {
            quantity = amount;
        }
    }
}