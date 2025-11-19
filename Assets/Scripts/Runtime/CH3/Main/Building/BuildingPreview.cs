using UnityEngine;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 건축 프리뷰 - 설치될 건물의 미리보기
    /// </summary>
    public class BuildingPreview : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Transform spriteTransform;
        [SerializeField] private LineRenderer borderLineRenderer; // 건물 크기 테두리
        
        private CH3_LevelData _buildingData;
        private Material _previewMaterial;
        private Color _validColor = new Color(1f, 1f, 1f, 0.6f);
        private Color _invalidColor = new Color(1f, 0f, 0f, 0.6f);
        private Color _validBorderColor = new Color(0f, 1f, 0f, 1f); // 초록색 테두리
        private Color _invalidBorderColor = new Color(0.5f, 0.5f, 0.5f, 1f); // 회색 테두리
        private float _cellWidth = 1f;
        
        public void Initialize(CH3_LevelData data, float alpha, float cellWidth = 1f)
        {
            _buildingData = data;
            _cellWidth = cellWidth;
            
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
            
            if (spriteRenderer == null)
            {
                Debug.LogError("BuildingPreview: SpriteRenderer를 찾을 수 없습니다!");
                return;
            }
            
            // 스프라이트 설정
            if (data.sprite != null)
            {
                spriteRenderer.sprite = data.sprite;
            }
            
            // 머티리얼 복사 및 알파 설정
            _previewMaterial = new Material(spriteRenderer.material);
            _previewMaterial.SetFloat("_Mode", 2); // Fade mode
            _previewMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _previewMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            _previewMaterial.SetInt("_ZWrite", 0);
            _previewMaterial.DisableKeyword("_ALPHATEST_ON");
            _previewMaterial.EnableKeyword("_ALPHABLEND_ON");
            _previewMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            _previewMaterial.renderQueue = 3000;
            
            spriteRenderer.material = _previewMaterial;
            
            // 초기 색상 설정
            _validColor.a = alpha;
            _invalidColor.a = alpha;
            spriteRenderer.color = _validColor;
            
            // Collider 비활성화 (프리뷰는 충돌하지 않음)
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
            
            // 테두리 초기화
            InitializeBorder();
        }
        
        /// <summary>
        /// 테두리 초기화
        /// </summary>
        private void InitializeBorder()
        {
            if (_buildingData == null) return;
            
            // LineRenderer가 없으면 생성
            if (borderLineRenderer == null)
            {
                GameObject borderObj = new GameObject("Border");
                borderObj.transform.SetParent(transform);
                borderObj.transform.localPosition = Vector3.zero;
                borderLineRenderer = borderObj.AddComponent<LineRenderer>();
            }
            
            if (borderLineRenderer == null) return;
            
            // LineRenderer 설정
            borderLineRenderer.useWorldSpace = false;
            borderLineRenderer.loop = true;
            borderLineRenderer.startWidth = 0.1f;
            borderLineRenderer.endWidth = 0.1f;
            borderLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            borderLineRenderer.sortingOrder = 100; // 다른 오브젝트 위에 표시
            
            // 건물 크기에 맞게 테두리 그리기
            Vector2Int tileSize = _buildingData.TileSize;
            
            // 테두리 꼭짓점 계산 (중심 기준)
            float halfWidth = (tileSize.x * _cellWidth) / 2f;
            float halfHeight = (tileSize.y * _cellWidth) / 2f;
            
            borderLineRenderer.positionCount = 4;
            borderLineRenderer.SetPosition(0, new Vector3(-halfWidth, 0.01f, -halfHeight));
            borderLineRenderer.SetPosition(1, new Vector3(halfWidth, 0.01f, -halfHeight));
            borderLineRenderer.SetPosition(2, new Vector3(halfWidth, 0.01f, halfHeight));
            borderLineRenderer.SetPosition(3, new Vector3(-halfWidth, 0.01f, halfHeight));
        }
        
        /// <summary>
        /// 프리뷰 위치 및 유효성 업데이트
        /// </summary>
        public void SetPosition(Vector3 position, bool isValid)
        {
            transform.position = position;
            
            if (spriteRenderer != null)
            {
                spriteRenderer.color = isValid ? _validColor : _invalidColor;
            }
            
            // 테두리 색상 업데이트
            if (borderLineRenderer != null)
            {
                borderLineRenderer.startColor = isValid ? _validBorderColor : _invalidBorderColor;
                borderLineRenderer.endColor = isValid ? _validBorderColor : _invalidBorderColor;
            }
        }
        
        private void OnDestroy()
        {
            if (_previewMaterial != null)
            {
                Destroy(_previewMaterial);
            }
        }
    }
}

