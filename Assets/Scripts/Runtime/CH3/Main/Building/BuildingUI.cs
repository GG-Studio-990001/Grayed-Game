using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 건축 UI - 설치 UI
    /// </summary>
    public class BuildingUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject uiPanel;
        [SerializeField] private TextMeshProUGUI buildingNameText;
        [SerializeField] private TextMeshProUGUI gridPositionText;
        [SerializeField] private Image installIcon;
        [SerializeField] private GameObject installHint;
        [SerializeField] private TextMeshProUGUI currencyText;
        
        [Header("Grid Visualization")]
        [SerializeField] private LineRenderer gridLinePrefab;
        [SerializeField] private Transform gridParent;
        [SerializeField] private Color gridLineColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); // 회색 반투명
        [SerializeField] private bool showGrid = true;
        [SerializeField] private int gridViewRange = 30; // 플레이어 주변 표시 범위
        
        private CH3_LevelData _currentBuildingData;
        private GridSystem _gridSystem;
        
        // 오브젝트 풀링: 세로선 (x 좌표를 키로)
        private Dictionary<int, LineRenderer> _verticalLines = new Dictionary<int, LineRenderer>();
        // 오브젝트 풀링: 가로선 (y 좌표를 키로)
        private Dictionary<int, LineRenderer> _horizontalLines = new Dictionary<int, LineRenderer>();
        
        private Vector2Int _lastPlayerGridPosition = new Vector2Int(int.MinValue, int.MinValue);
        
        // SortingOrder 계산용 (BuildingPreview와 동일한 로직)
        private int _gridSortingScale = 1;
        
        // 그리드 라인 Material (메모리 누수 방지)
        private Material _gridLineMaterial;
        
        private void Awake()
        {
            if (uiPanel != null)
            {
                uiPanel.SetActive(false);
            }
            
            if (gridParent == null)
            {
                gridParent = transform;
            }
        }
        
        private void Start()
        {
            _gridSystem = GridSystem.Instance;
            
            // 그리드 라인 Material 생성
            if (_gridLineMaterial == null)
            {
                _gridLineMaterial = new Material(Shader.Find("Sprites/Default"));
            }
        }
        
        /// <summary>
        /// UI 표시
        /// </summary>
        public void Show(CH3_LevelData buildingData, Vector2Int centerGridPosition, int range)
        {
            _currentBuildingData = buildingData;
            
            // SortingOrder 계산용 설정 저장
            if (buildingData != null && buildingData.applyInitialGridSorting)
            {
                _gridSortingScale = buildingData.gridSortingScale;
            }
            else
            {
                _gridSortingScale = 1;
            }
            
            if (uiPanel != null)
            {
                uiPanel.SetActive(true);
            }
            
            if (buildingNameText != null && buildingData != null)
            {
                buildingNameText.text = buildingData.id;
            }
            
            if (installHint != null)
            {
                installHint.SetActive(true);
            }
            
            // 재화 정보 표시
            UpdateCurrencyDisplay(buildingData);
            
            // 그리드 시각화 (플레이어 위치 기준)
            if (showGrid && _gridSystem != null)
            {
                UpdateGridVisibility(centerGridPosition);
            }
        }
        
        /// <summary>
        /// UI 숨김
        /// </summary>
        public void Hide()
        {
            _currentBuildingData = null;
            
            if (uiPanel != null)
            {
                uiPanel.SetActive(false);
            }
            
            if (installHint != null)
            {
                installHint.SetActive(false);
            }
            
            // 그리드 비활성화 (삭제하지 않음)
            DisableAllGridLines();
        }
        
        /// <summary>
        /// 재화 정보 표시 업데이트 (재화 불필요하므로 비활성화)
        /// </summary>
        private void UpdateCurrencyDisplay(CH3_LevelData buildingData)
        {
            // 재화 표시 제거 (재화 불필요)
            if (currencyText != null)
            {
                currencyText.text = "";
            }
        }
        
        /// <summary>
        /// 프리뷰 업데이트 (재화 정보도 함께 업데이트)
        /// </summary>
        public void UpdatePreview(Vector2Int gridPosition, bool canPlace)
        {
            if (gridPositionText != null)
            {
                gridPositionText.text = $"({gridPosition.x}, {gridPosition.y})";
            }
            
            if (installIcon != null)
            {
                installIcon.color = canPlace ? Color.white : Color.red;
            }
            
            // 재화 정보도 업데이트
            if (_currentBuildingData != null)
            {
                UpdateCurrencyDisplay(_currentBuildingData);
            }
        }
        
        /// <summary>
        /// 플레이어 위치 업데이트 (BuildingSystem에서 호출)
        /// </summary>
        public void UpdatePlayerPosition(Vector2Int playerGridPosition)
        {
            if (showGrid && _gridSystem != null && playerGridPosition != _lastPlayerGridPosition)
            {
                _lastPlayerGridPosition = playerGridPosition;
                UpdateGridVisibility(playerGridPosition);
            }
        }
        
        /// <summary>
        /// 그리드 가시성 업데이트 (플레이어 위치 기준으로 범위 내만 활성화)
        /// </summary>
        private void UpdateGridVisibility(Vector2Int playerGridPosition)
        {
            if (_gridSystem == null || gridLinePrefab == null) return;
            
            int actualGridSize = _gridSystem.ActualGridSize;
            int halfSize = actualGridSize / 2;
            
            // 플레이어 위치 기준 범위 계산
            int startX = playerGridPosition.x - gridViewRange;
            int endX = playerGridPosition.x + gridViewRange;
            int startY = playerGridPosition.y - gridViewRange;
            int endY = playerGridPosition.y + gridViewRange;
            
            // 그리드 전체 범위와 교집합
            int gridStartX = -halfSize;
            int gridEndX = halfSize;
            int gridStartY = -halfSize;
            int gridEndY = halfSize;
            
            startX = Mathf.Max(startX, gridStartX);
            endX = Mathf.Min(endX, gridEndX);
            startY = Mathf.Max(startY, gridStartY);
            endY = Mathf.Min(endY, gridEndY);
            
            // 세로선 업데이트 (x 좌표 기준)
            // 그리드 라인은 셀 경계에 그려져야 함 (셀 중심에서 ±cellWidth/2)
            float cellWidth = _gridSystem.CellWidth;
            float halfCellWidth = cellWidth / 2f;
            
            for (int x = gridStartX; x <= gridEndX + 1; x++)
            {
                // x 좌표의 왼쪽 경계 라인 (x 셀의 왼쪽 경계)
                bool shouldBeActive = (x - 1) >= startX && (x - 1) <= endX;
                
                if (!_verticalLines.ContainsKey(x))
                {
                    // 처음 생성
                    if (shouldBeActive)
                    {
                        // 셀 경계 위치 계산: (x-0.5) * cellWidth 위치
                        Vector3 cellCenterLeft = _gridSystem.GridToWorldPosition(new Vector2Int(x - 1, 0));
                        Vector3 cellCenterRight = _gridSystem.GridToWorldPosition(new Vector2Int(x, 0));
                        Vector3 boundaryPos = (cellCenterLeft + cellCenterRight) / 2f;
                        
                        // 위아래 끝점 계산
                        Vector3 startPos = boundaryPos + _gridSystem.transform.forward * (gridStartY * cellWidth - halfCellWidth);
                        Vector3 endPos = boundaryPos + _gridSystem.transform.forward * (gridEndY * cellWidth + halfCellWidth);
                        
                        // 세로선은 y 좌표 범위를 가로지르므로, 가장 뒤에 그려져야 하는 셀(가장 큰 y 값)의 baseOrder 사용
                        // baseOrder = -y * scale이므로, y가 클수록 baseOrder가 작아짐 (더 뒤에 그려짐)
                        // 따라서 가장 큰 y 값의 baseOrder를 사용하면 모든 셀보다 뒤에 그려짐
                        int maxY = Mathf.Max(gridStartY, gridEndY);
                        int baseOrder = -maxY * _gridSortingScale;
                        
                        LineRenderer line = CreateGridLine(startPos, endPos, baseOrder);
                        if (line != null)
                        {
                            _verticalLines[x] = line;
                        }
                    }
                }
                else
                {
                    // 이미 생성된 라인 활성화/비활성화
                    if (_verticalLines[x] != null)
                    {
                        _verticalLines[x].gameObject.SetActive(shouldBeActive);
                    }
                }
            }
            
            // 가로선 업데이트 (y 좌표 기준)
            for (int y = gridStartY; y <= gridEndY + 1; y++)
            {
                // y 좌표의 아래 경계 라인 (y 셀의 아래 경계)
                bool shouldBeActive = (y - 1) >= startY && (y - 1) <= endY;
                
                if (!_horizontalLines.ContainsKey(y))
                {
                    // 처음 생성
                    if (shouldBeActive)
                    {
                        // 셀 경계 위치 계산: (y-0.5) * cellWidth 위치
                        Vector3 cellCenterBottom = _gridSystem.GridToWorldPosition(new Vector2Int(0, y - 1));
                        Vector3 cellCenterTop = _gridSystem.GridToWorldPosition(new Vector2Int(0, y));
                        Vector3 boundaryPos = (cellCenterBottom + cellCenterTop) / 2f;
                        
                        // 좌우 끝점 계산
                        Vector3 startPos = boundaryPos + _gridSystem.transform.right * (gridStartX * cellWidth - halfCellWidth);
                        Vector3 endPos = boundaryPos + _gridSystem.transform.right * (gridEndX * cellWidth + halfCellWidth);
                        
                        // 가로선은 y-1 셀의 경계이므로, y-1의 baseOrder 사용
                        int gridY = y - 1;
                        int baseOrder = -gridY * _gridSortingScale;
                        
                        LineRenderer line = CreateGridLine(startPos, endPos, baseOrder);
                        if (line != null)
                        {
                            _horizontalLines[y] = line;
                        }
                    }
                }
                else
                {
                    // 이미 생성된 라인 활성화/비활성화
                    if (_horizontalLines[y] != null)
                    {
                        _horizontalLines[y].gameObject.SetActive(shouldBeActive);
                    }
                }
            }
        }
        
        /// <summary>
        /// 그리드 라인 생성
        /// </summary>
        private LineRenderer CreateGridLine(Vector3 start, Vector3 end, int baseOrder)
        {
            if (gridLinePrefab == null) return null;
            
            LineRenderer line = Instantiate(gridLinePrefab, gridParent);
            line.positionCount = 2;
            line.SetPosition(0, start);
            line.SetPosition(1, end);
            line.startColor = gridLineColor;
            line.endColor = gridLineColor;
            line.startWidth = 0.05f;
            line.endWidth = 0.05f;
            
            // Material 설정 (핑크색 오류 방지)
            if (_gridLineMaterial != null)
            {
                line.material = _gridLineMaterial;
            }
            
            // SortingOrder: GridLine < Border < Sprite
            // Border는 baseOrder - 1, Sprite는 baseOrder이므로
            // GridLine은 baseOrder - 2
            line.sortingOrder = baseOrder - 2;
            
            return line;
        }
        
        /// <summary>
        /// 모든 그리드 라인 비활성화 (삭제하지 않음)
        /// </summary>
        private void DisableAllGridLines()
        {
            foreach (var line in _verticalLines.Values)
            {
                if (line != null)
                {
                    line.gameObject.SetActive(false);
                }
            }
            
            foreach (var line in _horizontalLines.Values)
            {
                if (line != null)
                {
                    line.gameObject.SetActive(false);
                }
            }
            
            _lastPlayerGridPosition = new Vector2Int(int.MinValue, int.MinValue);
        }
        
        /// <summary>
        /// 그리드 완전 제거 (필요시에만 사용)
        /// </summary>
        private void ClearGrid()
        {
            // 모든 라인 제거
            foreach (var line in _verticalLines.Values)
            {
                if (line != null)
                {
                    Destroy(line.gameObject);
                }
            }
            _verticalLines.Clear();
            
            foreach (var line in _horizontalLines.Values)
            {
                if (line != null)
                {
                    Destroy(line.gameObject);
                }
            }
            _horizontalLines.Clear();
            
            _lastPlayerGridPosition = new Vector2Int(int.MinValue, int.MinValue);
        }
        
        private void OnDestroy()
        {
            // Material 메모리 누수 방지
            if (_gridLineMaterial != null)
            {
                Destroy(_gridLineMaterial);
            }
        }
    }
}

