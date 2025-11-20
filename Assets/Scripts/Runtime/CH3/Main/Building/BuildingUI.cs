using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 건축 UI - 그리드 표시 및 설치 UI
    /// </summary>
    public class BuildingUI : MonoBehaviour
    {
        [Header("Grid Visualization")]
        [SerializeField] private LineRenderer gridLinePrefab;
        [SerializeField] private Transform gridParent;
        [SerializeField] private Color gridLineColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        [SerializeField] private bool showGrid = true;
        
        [Header("UI Elements")]
        [SerializeField] private GameObject uiPanel;
        [SerializeField] private TextMeshProUGUI buildingNameText;
        [SerializeField] private TextMeshProUGUI gridPositionText;
        [SerializeField] private Image installIcon;
        [SerializeField] private GameObject installHint;
        [SerializeField] private TextMeshProUGUI currencyText;
        [SerializeField] private Transform currencyListParent;
        
        private GridSystem _gridSystem;
        private CurrencyManager _currencyManager;
        private CH3_LevelData _currentBuildingData;
        private LineRenderer[] _gridLines;
        private Vector2Int _centerGridPosition;
        private int _displayRange;
        
        private void Awake()
        {
            if (gridParent == null)
            {
                gridParent = transform;
            }
            
            if (uiPanel != null)
            {
                uiPanel.SetActive(false);
            }
        }
        
        private void Start()
        {
            _gridSystem = GridSystem.Instance;
            _currencyManager = CurrencyManager.Instance;
        }
        
        /// <summary>
        /// UI 표시
        /// </summary>
        public void Show(CH3_LevelData buildingData, Vector2Int centerGridPosition, int range)
        {
            _currentBuildingData = buildingData;
            _centerGridPosition = centerGridPosition;
            _displayRange = range;
            
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
            
            // 그리드 표시 (플레이어 위치 중심)
            if (showGrid && _gridSystem != null)
            {
                DrawGrid();
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
            
            // 그리드 제거
            ClearGrid();
        }
        
        /// <summary>
        /// 그리드 그리기 (플레이어 위치 중심)
        /// </summary>
        private void DrawGrid()
        {
            if (_gridSystem == null || gridLinePrefab == null) return;
            
            ClearGrid();
            
            // 플레이어 위치를 중심으로 범위 내 그리드만 표시
            int startX = _centerGridPosition.x - _displayRange;
            int endX = _centerGridPosition.x + _displayRange;
            int startY = _centerGridPosition.y - _displayRange;
            int endY = _centerGridPosition.y + _displayRange;
            
            List<LineRenderer> lines = new List<LineRenderer>();
            
            // 세로선 (범위 내)
            for (int x = startX; x <= endX; x++)
            {
                Vector2Int startGrid = new Vector2Int(x, startY);
                Vector2Int endGrid = new Vector2Int(x, endY);
                
                // 그리드 범위 체크
                if (!_gridSystem.IsValidGridPosition(startGrid) || !_gridSystem.IsValidGridPosition(endGrid))
                {
                    continue;
                }
                
                Vector3 startPos = _gridSystem.GridToWorldPosition(startGrid);
                Vector3 endPos = _gridSystem.GridToWorldPosition(endGrid);
                
                LineRenderer line = CreateGridLine(startPos, endPos);
                if (line != null)
                {
                    lines.Add(line);
                }
            }
            
            // 가로선 (범위 내)
            for (int y = startY; y <= endY; y++)
            {
                Vector2Int startGrid = new Vector2Int(startX, y);
                Vector2Int endGrid = new Vector2Int(endX, y);
                
                // 그리드 범위 체크
                if (!_gridSystem.IsValidGridPosition(startGrid) || !_gridSystem.IsValidGridPosition(endGrid))
                {
                    continue;
                }
                
                Vector3 startPos = _gridSystem.GridToWorldPosition(startGrid);
                Vector3 endPos = _gridSystem.GridToWorldPosition(endGrid);
                
                LineRenderer line = CreateGridLine(startPos, endPos);
                if (line != null)
                {
                    lines.Add(line);
                }
            }
            
            _gridLines = lines.ToArray();
        }
        
        /// <summary>
        /// 그리드 라인 생성
        /// </summary>
        private LineRenderer CreateGridLine(Vector3 start, Vector3 end)
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
            
            return line;
        }
        
        /// <summary>
        /// 그리드 제거
        /// </summary>
        private void ClearGrid()
        {
            if (_gridLines != null)
            {
                foreach (var line in _gridLines)
                {
                    if (line != null)
                    {
                        Destroy(line.gameObject);
                    }
                }
                _gridLines = null;
            }
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
    }
}

