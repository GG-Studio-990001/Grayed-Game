using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace Runtime.CH3.Main
{
    public class Ore : InteractableGridObject
    {
        [Header("Mining Settings")]
        [SerializeField] private int maxMiningCount = 3;
        [SerializeField] private Sprite[] miningStageSprites;

        [Header("Item Drop Settings")]
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private int minDropCount = 1;
        [SerializeField] private int maxDropCount = 3;
        [SerializeField] private float dropRadius = 1f;

        private int currentMiningCount;
        //private SpriteRenderer spriteRenderer;
        private MineralSpawnZone spawnZone;
        private List<GameObject> droppedItems = new List<GameObject>();

        public override void Initialize(Vector2Int gridPos)
        {
            base.Initialize(gridPos);
            spriteRenderer = GetComponent<SpriteRenderer>();
            spawnZone = FindObjectOfType<MineralSpawnZone>();
            currentMiningCount = maxMiningCount;
            UpdateSprite();

            // 초기에는 콜라이더 비활성화 (애니메이션 완료 후 활성화)
            var collider = GetComponent<Collider>();
            if (collider != null) collider.enabled = false;
        }

        public override void OnInteract(GameObject interactor)
        {
            if (!canInteract) return;
            
            currentMiningCount--;
            if (currentMiningCount <= 0)
            {
                StartCoroutine(MiningCompleteSequence(interactor));
            }
            else
            {
                UpdateSprite();
                PlayMiningEffect();
            }
        }

        private void UpdateSprite()
        {
            if (miningStageSprites != null && miningStageSprites.Length > 0)
            {
                int spriteIndex = Mathf.Clamp(
                    miningStageSprites.Length - currentMiningCount,
                    0,
                    miningStageSprites.Length - 1
                );
                spriteRenderer.sprite = miningStageSprites[spriteIndex];
            }
        }

        private void PlayMiningEffect()
        {
            // 채굴 효과 (흔들림)
            transform.DOShakePosition(0.3f, 0.1f, 10, 90, false, true);
            
            // 파티클 효과 (있다면)
            var particleSystem = GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
        }

        private IEnumerator MiningCompleteSequence(GameObject interactor)
        {
            canInteract = false;

            // 아이템 드롭
            SpawnItems();
            
            // 페이드 아웃 애니메이션
            yield return StartCoroutine(FadeOutMineral());
            
            // 광물 제거 (아이템들은 자체적으로 수집됨)
            if (spawnZone != null)
            {
                spawnZone.OnMineralRemoved(objectType);
            }
            Remove();
        }

        private void SpawnItems()
        {
            int dropCount = Random.Range(minDropCount, maxDropCount + 1);
            float angleStep = 360f / dropCount;
            Vector3 mineralBasePosition = transform.position; // 광물의 바닥 위치

            for (int i = 0; i < dropCount; i++)
            {
                // 방사형으로 드롭 방향 계산 (약간의 랜덤성 추가)
                float angle = i * angleStep + Random.Range(-15f, 15f);
                Vector3 dropDirection = Quaternion.Euler(0, 0, angle) * Vector3.right;
                float distance = Random.Range(0.5f, dropRadius);

                // 아이템 생성 (광물의 바닥 위치에서 시작)
                GameObject item = Instantiate(itemPrefab, mineralBasePosition, Quaternion.identity);
                OreItem oreItem = item.GetComponent<OreItem>();
        
                if (oreItem != null)
                {
                    oreItem.Drop(mineralBasePosition, dropDirection, distance);
                    droppedItems.Add(item);
                }
            }
        }

        private IEnumerator FadeOutMineral()
        {
            // 광물 페이드 아웃 및 스케일 감소
            Sequence fadeSequence = DOTween.Sequence();
            
            fadeSequence.Join(spriteRenderer.DOFade(0f, 0.5f));
            fadeSequence.Join(transform.DOScale(Vector3.zero, 0.5f));
            
            yield return fadeSequence.WaitForCompletion();
        }

        public override void Remove()
        {
            if (spawnZone != null)
            {
                spawnZone.OnMineralRemoved(objectType);
            }
            base.Remove();
        }

        // 디버그용 기즈모
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, dropRadius);
        }
    }
}