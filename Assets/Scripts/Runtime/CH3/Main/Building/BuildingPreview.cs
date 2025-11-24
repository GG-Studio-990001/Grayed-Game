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
        
        private CH3_LevelData _buildingData;
        private Material _previewMaterial;
        private Color _validColor = new Color(1f, 1f, 1f, 0.6f);
        private Color _invalidColor = new Color(1f, 0f, 0f, 0.6f);
        private float _cellWidth = 1f;
        private SortingOrderObject _sortingOrderObject;
        private int _colorPropertyId;
        
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
            
            // 머티리얼 복사
            _previewMaterial = new Material(spriteRenderer.material);
            spriteRenderer.material = _previewMaterial;
            
            // 셰이더 프로퍼티 ID 캐싱 (OcclusionFader 방식)
            _colorPropertyId = Shader.PropertyToID("_Color");
            
            // 초기 색상 설정 (반투명)
            _validColor.a = alpha;
            _invalidColor.a = alpha;
            
            // Material의 _Color 프로퍼티를 직접 설정하여 반투명 적용
            _previewMaterial.SetColor(_colorPropertyId, _validColor);
            
            // Collider 비활성화 (프리뷰는 충돌하지 않음)
            Collider[] colliders = GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }
            
            // SortingOrderObject 찾기
            if (spriteTransform != null)
            {
                _sortingOrderObject = spriteTransform.GetComponent<SortingOrderObject>();
            }
            if (_sortingOrderObject == null)
            {
                _sortingOrderObject = GetComponentInChildren<SortingOrderObject>();
            }
        }
        
        /// <summary>
        /// 프리뷰 위치 및 유효성 업데이트
        /// </summary>
        public void SetPosition(Vector3 position, bool isValid, Vector2Int gridPosition)
        {
            transform.position = position;
            
            if (spriteRenderer != null && _previewMaterial != null)
            {
                // 반투명 상태 유지 - Material의 _Color 프로퍼티를 직접 설정 (OcclusionFader 방식)
                Color currentColor = isValid ? _validColor : _invalidColor;
                _previewMaterial.SetColor(_colorPropertyId, currentColor);
            }
            
            // SortingOrder 업데이트 (GridObject와 동일한 로직)
            if (_buildingData != null && _buildingData.applyInitialGridSorting)
            {
                int baseOrder = -gridPosition.y * _buildingData.gridSortingScale;
                
                if (_sortingOrderObject != null)
                {
                    _sortingOrderObject.SetBaseOrder(baseOrder);
                }
                else if (spriteRenderer != null)
                {
                    // SortingOrderObject가 없으면 직접 설정
                    spriteRenderer.sortingOrder = baseOrder;
                }
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

