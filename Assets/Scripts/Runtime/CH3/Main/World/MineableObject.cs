using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 채굴 가능한 오브젝트의 공통 기능을 제공하는 베이스 클래스
    /// Ore와 Breakable의 중복 코드를 제거하기 위해 생성
    /// </summary>
    public abstract class MineableObject : InteractableGridObject, IHoldInteractable
    {
        [Header("UI")]
        [SerializeField] protected float autoHideDelay = 5f;

        [Header("Mining Settings")]
        [SerializeField] protected int maxMiningCount = 1;
        [SerializeField] protected Sprite[] miningStageSprites;

        [Header("Item Drop Settings")]
        [SerializeField] protected GameObject itemPrefab;
        [SerializeField] protected int minDropCount = 1;
        [SerializeField] protected int maxDropCount = 3;
        [SerializeField] protected float dropRadius = 1f;

        [Header("Interaction Settings")]
        [SerializeField] protected bool enableColliderOnStart = true;
        [SerializeField] protected bool debugInteraction = false;
        [SerializeField] protected bool resetGaugeOnCancel = false;

        // Grid Position Settings는 base 클래스(GridObject)에서 상속됨

        protected int currentMiningCount;
        protected float lastInteractionTime;
        protected Canvas uiCanvas;
        protected Slider holdGauge;
        protected Collider objectCollider;

        // IHoldInteractable 구현
        public float HoldSeconds => 2f;

        public override void Initialize(Vector2Int gridPos)
        {
            // base.Initialize에서 gridPositionMode에 따라 위치 결정 및 이동
            base.Initialize(gridPos);

            InitializeMining();
            InitializeUI();
            InitializeCollider();
        }

        protected virtual void InitializeMining()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            currentMiningCount = maxMiningCount;
            UpdateSprite();
        }

        protected virtual void InitializeUI()
        {
            uiCanvas = GetComponentInChildren<Canvas>(true);
            if (uiCanvas != null)
            {
                uiCanvas.gameObject.SetActive(false);
            }

            holdGauge = GetComponentInChildren<Slider>(true);
            if (holdGauge == null)
            {
                holdGauge = GetComponent<Slider>();
            }
        }

        protected virtual void InitializeCollider()
        {
            // 먼저 자기 자신에서 찾기
            objectCollider = GetComponent<Collider>();
            
            // 없으면 자식에서 찾기 (리소스 분리 구조 지원)
            if (objectCollider == null)
            {
                objectCollider = GetComponentInChildren<Collider>();
            }
            
            // GridObject가 있으면 spriteTransform에서 찾기
            if (objectCollider == null)
            {
                var gridObject = GetComponent<GridObject>();
                if (gridObject != null)
                {
                    var spriteTransform = gridObject.GetSpriteTransform();
                    if (spriteTransform != null && spriteTransform != transform)
                    {
                        objectCollider = spriteTransform.GetComponent<Collider>();
                    }
                }
            }

            if (objectCollider != null)
            {
                if (enableColliderOnStart)
                {
                    objectCollider.enabled = true;
                    if (debugInteraction)
                    {
                        Debug.Log($"[{GetType().Name}] 콜라이더 활성화됨: {gameObject.name} at {objectCollider.transform.position}");
                    }
                }
                else
                {
                    StartCoroutine(EnableColliderAfterDelay(1f));
                }
            }
            else if (debugInteraction)
            {
                Debug.LogWarning($"[{GetType().Name}] 콜라이더가 없습니다: {gameObject.name}");
            }
        }

        private IEnumerator EnableColliderAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (objectCollider != null)
            {
                objectCollider.enabled = true;
                if (debugInteraction)
                {
                    Debug.Log($"[{GetType().Name}] 지연 후 콜라이더 활성화됨: {gameObject.name}");
                }
            }
        }

        public override void OnInteract(GameObject interactor) { /* 연타 채집 비활성화 */ }

        public void OnHoldStart(GameObject interactor)
        {
            lastInteractionTime = Time.time;
            if (uiCanvas != null)
            {
                uiCanvas.gameObject.SetActive(true);
                if (debugInteraction)
                {
                    Debug.Log($"[{GetType().Name}] UI 활성화됨: {gameObject.name}");
                }
            }
            if (holdGauge != null)
            {
                holdGauge.gameObject.SetActive(true);
                if (debugInteraction)
                {
                    Debug.Log($"[{GetType().Name}] 게이지 활성화됨: {gameObject.name}, 현재 값: {holdGauge.value}");
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
            if (resetGaugeOnCancel)
            {
                if (holdGauge != null)
                {
                    holdGauge.value = 0f;
                }
            }
        }

        public void OnHoldComplete(GameObject interactor)
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

            HideUI();
        }

        protected virtual void HideUI()
        {
            if (uiCanvas != null)
            {
                uiCanvas.gameObject.SetActive(false);
            }
            if (holdGauge != null)
            {
                if (currentMiningCount <= 0)
                {
                    holdGauge.value = 0f;
                }
                holdGauge.gameObject.SetActive(false);
            }

            if (debugInteraction)
            {
                Debug.Log($"[{GetType().Name}] UI 숨김: {gameObject.name}");
            }
        }

        private void Update()
        {
            if (uiCanvas != null && uiCanvas.gameObject.activeSelf)
            {
                if (Time.time - lastInteractionTime > autoHideDelay)
                {
                    uiCanvas.gameObject.SetActive(false);
                    if (holdGauge != null)
                    {
                        if (resetGaugeOnCancel)
                        {
                            holdGauge.value = 0f;
                        }
                        holdGauge.gameObject.SetActive(false);
                    }

                    if (debugInteraction)
                    {
                        Debug.Log($"[{GetType().Name}] {autoHideDelay}초 후 자동 숨김: {gameObject.name}");
                    }
                }
            }
        }

        protected void UpdateSprite()
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

        protected void PlayMiningEffect()
        {
            transform.DOShakePosition(0.3f, 0.1f, 10, 90, false, true);

            var particleSystem = GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
        }

        protected IEnumerator MiningCompleteSequence(GameObject interactor)
        {
            canInteract = false;

            if (objectCollider != null)
            {
                objectCollider.enabled = false;
                if (debugInteraction)
                {
                    Debug.Log($"[{GetType().Name}] 채굴 완료로 콜라이더 비활성화: {gameObject.name}");
                }
            }

            SpawnItems();
            yield return StartCoroutine(FadeOutMineral());
            OnMiningComplete();
            Remove();
        }

        protected abstract void OnMiningComplete();

        protected void SpawnItems()
        {
            if (itemPrefab == null) return;

            int dropCount = Random.Range(minDropCount, maxDropCount + 1);
            float angleStep = 360f / dropCount;
            Vector3 basePosition = transform.position;

            for (int i = 0; i < dropCount; i++)
            {
                float angle = i * angleStep + Random.Range(-15f, 15f);
                Vector3 dropDirection = Quaternion.Euler(0, 0, angle) * Vector3.right;
                float distance = Random.Range(0.5f, dropRadius);

                GameObject item = Instantiate(itemPrefab, basePosition, Quaternion.identity);
                OreItem oreItem = item.GetComponent<OreItem>();

                if (oreItem != null)
                {
                    oreItem.Drop(basePosition, dropDirection, distance);
                }
            }
        }

        protected IEnumerator FadeOutMineral()
        {
            Sequence fadeSequence = DOTween.Sequence();
            fadeSequence.Join(spriteRenderer.DOFade(0f, 0.5f));
            fadeSequence.Join(transform.DOScale(Vector3.zero, 0.5f));
            yield return fadeSequence.WaitForCompletion();
        }

        public void ResetGauge()
        {
            if (holdGauge != null)
            {
                holdGauge.value = 0f;
            }
        }

        public float GetCurrentGaugeValue()
        {
            return holdGauge != null ? holdGauge.value : 0f;
        }
    }
}

