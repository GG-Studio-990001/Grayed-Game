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
        [SerializeField] private Item itemData; // 인벤토리에 추가할 아이템 데이터(SO)

        [Header("Drop Animation Settings")]
        [SerializeField] private float initialHeight = 1f;
        [SerializeField] private float bounceHeight = 0.5f;
        [SerializeField] private float dropDuration = 0.5f;
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
        private Inventory inventory;

        public string ItemId => itemId;
        public int Quantity => quantity;

        private void Start()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (playerTransform == null)
            {
                Debug.LogWarning("Player not found! Make sure the player has the 'Player' tag.");
            }

            if (inventory == null)
            {
                inventory = FindObjectOfType<Inventory>();
                if (inventory == null)
                {
                    Debug.LogWarning("Inventory not found in scene. OreItem will not be collectible into inventory.");
                }
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
            Vector3 spawnPos = startPosition + Vector3.up * initialHeight;
            transform.position = spawnPos;

            Vector3 targetPos = startPosition + dropDirection.normalized * dropDistance;
            targetPos.y = initialHeight;

            Sequence dropSequence = DOTween.Sequence();
            Vector3 midPoint = Vector3.Lerp(spawnPos, targetPos, 0.5f);
            midPoint.y = Mathf.Max(spawnPos.y, targetPos.y) + bounceHeight;
            
            Vector3[] path = new Vector3[]
            {
                spawnPos,
                midPoint,
                targetPos
            };

            dropSequence.Append(transform.DOPath(
                path,
                dropDuration,
                PathType.CatmullRom
            ).SetEase(dropEase));

            dropSequence.Join(transform.DORotate(
                new Vector3(0f, 360f, 0f),
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

                float distanceRatio = distanceToPlayer / initialDistance;
                float scale = Mathf.Lerp(minScale, 1f, distanceRatio);

                transform.Rotate(Vector3.up * 360f * Time.deltaTime, Space.World);

                yield return null;
            }
        }

        private void OnCollected()
        {
            if (inventory != null && itemData != null)
            {
                bool added = inventory.TryAdd(itemData, Mathf.Max(1, quantity));
                if (!added)
                {
                    // 가득 차면 수집 취소 (오브젝트 유지)
                    isBeingCollected = false;
                    isDropped = true;
                    return;
                }
            }

            // 수집 효과
            Sequence collectSequence = DOTween.Sequence();
            
            collectSequence.Append(transform.DOScale(Vector3.zero, 0.2f));
            collectSequence.Join(transform.DORotate(
                new Vector3(0, 720, 0),
                0.2f,
                RotateMode.FastBeyond360
            ));

            collectSequence.OnComplete(() => {
                Destroy(gameObject);
            });
        }

        public void SetQuantity(int amount)
        {
            quantity = amount;
        }
    }
}