using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

namespace Runtime.CH3.Main
{
    public class Breakable : InteractableGridObject, IHoldInteractable
    {
        [Header("UI")]
        [SerializeField] private float autoHideDelay = 5f; // 자동 숨김 시간

        [Header("Mining Settings")]
        [SerializeField] private int maxMiningCount = 3;
        [SerializeField] private Sprite[] miningStageSprites;

        [Header("Item Drop Settings")]
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private int minDropCount = 1;
        [SerializeField] private int maxDropCount = 3;
        [SerializeField] private float dropRadius = 1f;

        [Header("Interaction Settings")]
        [SerializeField] private bool enableColliderOnStart = true; // 시작 시 콜라이더 활성화 여부
        [SerializeField] private bool debugInteraction = false; // 디버그 정보 출력 여부
        [SerializeField] private bool resetGaugeOnCancel = false; // 취소 시 게이지 초기화 여부

        [Header("Grid Position Settings")]
        [SerializeField] private bool initializeToGridPosition = true; // 초기 스폰을 GridPosition에 맞춤

        private int currentMiningCount;
        private List<GameObject> droppedItems = new List<GameObject>();
        private float lastInteractionTime;
        private Canvas uiCanvas; // 자동 찾기
        private Slider holdGauge; // 자동 찾기
        private Collider oreCollider; // 콜라이더 참조

        protected override void Start()
        {
            base.Start();
            Initialize(gridPosition);
        }

        public override void Initialize(Vector2Int gridPos)
        {
            base.Initialize(gridPos);

            // 초기 배치: 인스펙터의 gridPosition을 우선 적용
            if (initializeToGridPosition && gridManager != null)
            {
                // BaseGridObject가 gridPosition을 유지하고 있으므로 그 좌표로 월드 위치 재배치
                Vector2Int targetGrid = gridPosition == Vector2Int.zero ? gridPos : gridPosition;
                Vector3 world = GetWorldPositionForGrid(targetGrid);
                transform.position = world;
                UpdateGridPosition();
            }


            GridSystem.Instance.SetCellBlocked(gridPosition, true);
            GridSystem.Instance.SetCellOccupied(gridPosition, true, gameObject);

            spriteRenderer = GetComponent<SpriteRenderer>();
            // spawnZone = FindObjectOfType<MineralSpawnZone>(); // GridSystem으로 통합됨
            currentMiningCount = maxMiningCount;
            UpdateSprite();

            // 콜라이더 참조 저장
            oreCollider = GetComponent<Collider>();

            // Canvas 자동 찾기
            uiCanvas = GetComponentInChildren<Canvas>(true);
            if (uiCanvas != null)
            {
                uiCanvas.gameObject.SetActive(false);
            }

            // Slider 자동 찾기
            holdGauge = GetComponentInChildren<UnityEngine.UI.Slider>(true);
            if (holdGauge == null)
            {
                holdGauge = GetComponent<UnityEngine.UI.Slider>();
            }

            // 콜라이더 활성화 설정
            if (oreCollider != null)
            {
                if (enableColliderOnStart)
                {
                    oreCollider.enabled = true;
                    if (debugInteraction)
                    {
                        Debug.Log($"[Ore] 콜라이더 활성화됨: {gameObject.name} at {transform.position}");
                    }
                }
                else
                {
                    // 애니메이션 완료 후 활성화하는 경우를 위한 지연 활성화
                    StartCoroutine(EnableColliderAfterDelay(1f));
                }
            }
            else if (debugInteraction)
            {
                Debug.LogWarning($"[Ore] 콜라이더가 없습니다: {gameObject.name}");
            }
        }

        private IEnumerator EnableColliderAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (oreCollider != null)
            {
                oreCollider.enabled = true;
                if (debugInteraction)
                {
                    Debug.Log($"[Ore] 지연 후 콜라이더 활성화됨: {gameObject.name}");
                }
            }
        }

        public override void OnInteract(GameObject interactor) { /* 연타 채집 비활성화 */ }

        // IHoldInteractable 구현: 2초 홀드로 1회 채굴
        public float HoldSeconds => 2f;
        public void OnHoldStart(GameObject interactor)
        {
            lastInteractionTime = Time.time;
            if (uiCanvas != null)
            {
                uiCanvas.gameObject.SetActive(true);
                if (debugInteraction)
                {
                    Debug.Log($"[Ore] UI 활성화됨: {gameObject.name}");
                }
            }
            if (holdGauge != null)
            {
                holdGauge.gameObject.SetActive(true);
                if (debugInteraction)
                {
                    Debug.Log($"[Ore] 게이지 활성화됨: {gameObject.name}, 현재 값: {holdGauge.value}");
                }
            }
        }
        public void OnHoldProgress(GameObject interactor, float normalized01)
        {
            lastInteractionTime = Time.time;
            if (holdGauge != null)
            {
                holdGauge.value = Mathf.Clamp01(normalized01);
            }
        }
        public void OnHoldCancel(GameObject interactor)
        {
            // 취소 시에는 UI를 숨기지 않고, 5초 후 자동 숨김 로직에 맡김
            // 게이지 값은 유지됨 (resetGaugeOnCancel 옵션에 따라)
            if (resetGaugeOnCancel)
            {
                if (holdGauge != null)
                {
                    holdGauge.value = 0f;
                }
            }
            // UI는 Update 메서드에서 5초 후 자동으로 숨겨짐
        }
        public void OnHoldComplete(GameObject interactor)
        {
            if (!canInteract) return;
            currentMiningCount--;
            if (currentMiningCount <= 0)
            {
                StartCoroutine(MiningCompleteSequence(interactor));
                
                // 채굴이 완전히 끝났을 때만 UI와 게이지를 초기화
                if (uiCanvas != null)
                {
                    uiCanvas.gameObject.SetActive(false);
                }
                if (holdGauge != null)
                {
                    holdGauge.value = 0f;
                    holdGauge.gameObject.SetActive(false);
                }

                if (debugInteraction)
                {
                    Debug.Log($"[Ore] 채굴 완료로 UI 숨김: {gameObject.name}");
                }
            }
            else
            {
                UpdateSprite();
                PlayMiningEffect();
                
                // 채굴이 아직 남아있으면 UI는 숨기되 게이지는 유지
                if (uiCanvas != null)
                {
                    uiCanvas.gameObject.SetActive(false);
                }
                if (holdGauge != null)
                {
                    // 게이지 값은 유지하고 UI만 숨김
                    holdGauge.gameObject.SetActive(false);
                }

                if (debugInteraction)
                {
                    Debug.Log($"[Ore] 부분 채굴 완료, 게이지 유지: {gameObject.name}, 현재 값: {holdGauge?.value}");
                }
            }
        }

        private void Update()
        {
            // 5초간 입력 없으면 자동 숨김
            if (uiCanvas != null && uiCanvas.gameObject.activeSelf)
            {
                if (Time.time - lastInteractionTime > autoHideDelay)
                {
                    uiCanvas.gameObject.SetActive(false);
                    if (holdGauge != null)
                    {
                        // 자동 숨김 시에도 게이지 값 유지 (옵션에 따라)
                        if (resetGaugeOnCancel)
                        {
                            holdGauge.value = 0f;
                        }
                        holdGauge.gameObject.SetActive(false);
                    }

                    if (debugInteraction)
                    {
                        Debug.Log($"[Ore] 5초 후 자동 숨김: {gameObject.name}");
                    }
                }
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

            // 콜라이더 비활성화 (상호작용 방지)
            if (oreCollider != null)
            {
                oreCollider.enabled = false;
                if (debugInteraction)
                {
                    Debug.Log($"[Ore] 채굴 완료로 콜라이더 비활성화: {gameObject.name}");
                }
            }

            // 아이템 드롭
            SpawnItems();

            // 페이드 아웃 애니메이션
            yield return StartCoroutine(FadeOutMineral());

            // 광물 제거 (아이템들은 자체적으로 수집됨)
            if (GridSystem.Instance != null)
            {
                GridSystem.Instance.OnMineralRemoved(objectType);
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
            if (GridSystem.Instance != null)
            {
                GridSystem.Instance.OnMineralRemoved(objectType);

                GridSystem.Instance.SetCellBlocked(gridPosition, false);

                // 셀 점유 상태도 해제
                GridSystem.Instance.SetCellOccupied(gridPosition, false);

            }
            base.Remove();
        }

        // 게이지 값을 완전히 초기화하는 메서드
        public void ResetGauge()
        {
            if (holdGauge != null)
            {
                holdGauge.value = 0f;
            }
        }

        // 현재 게이지 값을 반환하는 메서드
        public float GetCurrentGaugeValue()
        {
            return holdGauge != null ? holdGauge.value : 0f;
        }
    }
}