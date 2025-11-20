using UnityEngine;
using TMPro;

namespace Runtime.CH3.Main.Exploration
{
    // 프로젝트의 정렬/충돌 규칙에 맞춘 가림 처리
    // - 뒤/앞 판정: 이 오브젝트의 SpriteRenderer.sortingOrder > 플레이어의 sortingOrder 이고, 일정 반경 내에 있을 때
    // - 투명도 변경: SpriteRenderer 알파가 아닌 셰이더 그래프의 머티리얼 프로퍼티를 변경하여 처리
    [DisallowMultipleComponent]
    public class OcclusionFader : MonoBehaviour
    {
        [Header("Renderers to control")]
        [SerializeField] private SpriteRenderer spriteRenderer; // 기준 정렬용
        [SerializeField] private Renderer[] additionalRenderers; // 다중 렌더러 지원

        [Header("Shader Graph Property")] 
        [SerializeField] private string fadeProperty = "_QueueControl"; // 셰이더에서 투명 제어할 프로퍼티 이름
        [SerializeField] private float visibleValue = 0f; // 기본값(불투명)
        [SerializeField] private float occludedValue = 1f; // 가려졌을 때 값(투명/반투명)
        [SerializeField] private float lerpSpeed = 10f;

        [Header("Occlusion Rule")] 
        [SerializeField] private Transform player; // 자동 탐색: Player 태그
        [SerializeField] private float detectRadius = 1.2f; // 가까울 때만 처리(슬롯처럼 얇은 오브젝트 과도 페이드 방지)
        [SerializeField] private LayerMask playerLayer = ~0;
        [SerializeField] private bool showLabel; // 라벨 사용 여부
        [SerializeField] private TextMeshPro labelPrefab;
        [SerializeField] private string labelText = "";
        [SerializeField] private Vector3 labelOffset = new Vector3(0, 1.2f, 0);

        private int _fadePropId;
        private float _currentValue;
        private TextMeshPro _labelInstance;
        private SpriteRenderer _playerSr;

        private void Reset()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            additionalRenderers = GetComponentsInChildren<Renderer>();
        }

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (player == null)
            {
                var go = GameObject.FindGameObjectWithTag("Player");
                if (go != null) player = go.transform;
            }
            _playerSr = player != null ? player.GetComponent<SpriteRenderer>() : null;

            _fadePropId = Shader.PropertyToID(fadeProperty);
            _currentValue = visibleValue;

            if (showLabel && !string.IsNullOrEmpty(labelText) && labelPrefab != null)
            {
                _labelInstance = Instantiate(labelPrefab, transform);
                _labelInstance.text = labelText;
                _labelInstance.gameObject.SetActive(false);
            }
        }

        private void LateUpdate()
        {
            if (spriteRenderer == null || player == null || _playerSr == null) return;

            bool closeEnough = (player.position - transform.position).sqrMagnitude <= (detectRadius * detectRadius);
            bool occluded = false;

            if (closeEnough)
            {
                // 정렬 순서 비교 기반: 오브젝트가 플레이어 앞에 있으면 가림
                occluded = spriteRenderer.sortingOrder > _playerSr.sortingOrder;
            }

            float target = occluded ? occludedValue : visibleValue;
            _currentValue = Mathf.MoveTowards(_currentValue, target, lerpSpeed * Time.deltaTime);

            ApplyToMaterials(_currentValue);

            if (_labelInstance != null)
            {
                _labelInstance.transform.position = transform.position + labelOffset;
                _labelInstance.gameObject.SetActive(occluded);
            }
        }

        private void ApplyToMaterials(float value)
        {
            if (spriteRenderer != null && spriteRenderer.sharedMaterial != null)
            {
                spriteRenderer.material.SetFloat(_fadePropId, value);
            }
            if (additionalRenderers != null)
            {
                for (int i = 0; i < additionalRenderers.Length; i++)
                {
                    var r = additionalRenderers[i];
                    if (r == null) continue;
                    foreach (var mat in r.materials)
                    {
                        if (mat != null) mat.SetFloat(_fadePropId, value);
                    }
                }
            }
        }
    }
}


