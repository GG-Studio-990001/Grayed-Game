using UnityEngine;
using System.Collections;
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
        [SerializeField] protected bool resetGaugeOnCancel = false;

        // Grid Position Settings는 base 클래스(GridObject)에서 상속됨

        protected int currentMiningCount;
        protected float lastInteractionTime;
        protected Canvas uiCanvas;
        protected Slider holdGauge;
        protected Collider objectCollider;

        // IHoldInteractable 구현
        public float HoldSeconds => 2f;

        /// <summary>
        /// CH3_LevelData로부터 데이터를 초기화합니다.
        /// </summary>
        public override void InitializeFromData(CH3_LevelData data)
        {
            base.InitializeFromData(data);
            if (data != null)
            {
                maxMiningCount = data.maxMiningCount;
                miningStageSprites = data.miningStageSprites;
                itemPrefab = data.itemPrefab;
                minDropCount = data.minDropCount;
                maxDropCount = data.maxDropCount;
                dropRadius = data.dropRadius;
                enableColliderOnStart = data.enableColliderOnStart;
            }
        }

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
                }
                else
                {
                    StartCoroutine(EnableColliderAfterDelay(1f));
                }
            }
        }

        private IEnumerator EnableColliderAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (objectCollider != null)
            {
                objectCollider.enabled = true;
            }
        }

        public override void OnInteract(GameObject interactor) { /* 연타 채집 비활성화 */ }

        public void OnHoldStart(GameObject interactor)
        {
            lastInteractionTime = Time.time;
            
            // UI 참조가 없거나 유효하지 않으면 다시 찾기
            if (uiCanvas == null)
            {
                InitializeUI();
            }
            
            if (uiCanvas != null)
            {
                uiCanvas.gameObject.SetActive(true);
            }
            
            // holdGauge가 null이면 다시 찾기
            if (holdGauge == null)
            {
                holdGauge = GetComponentInChildren<Slider>(true);
                if (holdGauge == null)
                {
                    holdGauge = GetComponent<Slider>();
                }
            }
            
            if (holdGauge != null)
            {
                holdGauge.gameObject.SetActive(true);
            }

            // 플레이어 채집 애니메이션 시작
            if (interactor != null)
            {
                var player = interactor.GetComponent<PlayerController>();
                if (player != null)
                {
                    // 이 MineableObject가 속한 게임오브젝트 이름을 기준으로
                    // 어떤 채집 애니메이션을 쓸지 결정한다.
                    player.StartGatherAnimation(gameObject.name);
                }
            }
        }

        public void OnHoldProgress(GameObject interactor, float normalized01)
        {
            lastInteractionTime = Time.time;
            
            // holdGauge가 null이면 다시 찾기
            if (holdGauge == null)
            {
                holdGauge = GetComponentInChildren<Slider>(true);
                if (holdGauge == null)
                {
                    holdGauge = GetComponent<Slider>();
                }
            }
            
            if (holdGauge != null)
            {
                // 게이지바가 비활성화되어 있으면 활성화
                if (!holdGauge.gameObject.activeSelf)
                {
                    holdGauge.gameObject.SetActive(true);
                }
                
                // UI Canvas도 활성화
                if (uiCanvas != null && !uiCanvas.gameObject.activeSelf)
                {
                    uiCanvas.gameObject.SetActive(true);
                }
                
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

            // 채집이 중간에 취소되면 애니메이션도 정지
            if (interactor != null)
            {
                var player = interactor.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.StopGatherAnimation();
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

             // 채집이 완료되면 애니메이션 정지
            if (interactor != null)
            {
                var player = interactor.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.StopGatherAnimation();
                }
            }
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

