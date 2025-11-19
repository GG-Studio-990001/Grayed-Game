using UnityEngine;
using System.Collections.Generic;
using Runtime.Input;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 건축 시스템 - 건축 모드 관리 및 설치 로직
    /// </summary>
    public class BuildingSystem : MonoBehaviour
    {
        public static BuildingSystem Instance { get; private set; }
        
        [Header("References")]
        [SerializeField] private GridSystem gridSystem;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private GameObject basePrefab; // 2.5D_Object 프리팹
        [SerializeField] private BuildingPreview previewPrefab;
        [SerializeField] private BuildingUI buildingUI;
        [SerializeField] private Inventory inventory;
        [SerializeField] private CurrencyManager currencyManager;
        [SerializeField] private PlayerController playerController;
        
        [Header("Building Range Settings")]
        [Tooltip("플레이어 주변 건축 가능 범위 (타일 단위)")]
        [SerializeField] private int buildingRange = 5; // 플레이어 주변 5타일 범위
        
        [Header("Settings")]
        [SerializeField] private LayerMask groundLayerMask = 1; // Default layer
        [SerializeField] private float previewAlpha = 0.6f;
        
        private bool _isBuildingMode = false;
        private Item _currentBuildingItem;
        private CH3_LevelData _currentBuildingData;
        private BuildingPreview _currentPreview;
        private Vector2Int _currentGridPosition;
        private bool _canPlace = false;
        private int _currentItemSlotIndex = -1; // 현재 사용 중인 아이템 슬롯 인덱스
        private Vector2Int _playerGridPosition; // 건축 모드 시작 시 플레이어 위치
        private Vector2Int _manualGridOffset = Vector2Int.zero; // 키보드로 조정한 오프셋
        
        // 건설된 건물 추적 (maxBuild 제한용)
        private Dictionary<string, int> _builtCounts = new Dictionary<string, int>();
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            // 자동 참조 찾기
            if (gridSystem == null)
            {
                gridSystem = GridSystem.Instance;
            }
            
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            
            if (inventory == null)
            {
                inventory = FindObjectOfType<Inventory>();
            }
            
            if (currencyManager == null)
            {
                currencyManager = CurrencyManager.Instance;
            }
            
            if (playerController == null)
            {
                playerController = FindObjectOfType<PlayerController>();
            }
            
            // BuildingObjectFactory에 기본 프리팹 설정
            if (basePrefab != null)
            {
                BuildingObjectFactory.SetBasePrefab(basePrefab);
            }
        }
        
        private void Update()
        {
            if (!_isBuildingMode) return;
            
            UpdatePreview();
            HandleInput();
        }
        
        /// <summary>
        /// 건축 모드 시작
        /// </summary>
        public void StartBuildingMode(Item buildingItem, int slotIndex = -1)
        {
            if (buildingItem == null || !buildingItem.IsBuildingItem)
            {
                Debug.LogWarning("건축 아이템이 아닙니다!");
                return;
            }
            
            if (playerController == null || gridSystem == null)
            {
                Debug.LogWarning("플레이어 또는 그리드 시스템을 찾을 수 없습니다!");
                return;
            }
            
            // 플레이어 현재 위치 저장
            Vector3 playerWorldPos = playerController.transform.position;
            _playerGridPosition = gridSystem.WorldToGridPosition(playerWorldPos);
            _manualGridOffset = Vector2Int.zero; // 오프셋 초기화
            
            _currentBuildingItem = buildingItem;
            _currentBuildingData = buildingItem.buildingData;
            _currentItemSlotIndex = slotIndex;
            _isBuildingMode = true;
            
            // 플레이어 이동 비활성화
            if (Managers.Data != null && Managers.Data.InGameKeyBinder != null)
            {
                Managers.Data.InGameKeyBinder.PlayerInputDisable();
            }
            
            // 프리뷰 생성
            if (previewPrefab != null && _currentPreview == null)
            {
                _currentPreview = Instantiate(previewPrefab);
                float cellWidth = gridSystem != null ? gridSystem.CellWidth : 1f;
                _currentPreview.Initialize(_currentBuildingData, previewAlpha, cellWidth);
            }
            
            // UI 표시 (플레이어 위치 전달)
            if (buildingUI != null)
            {
                buildingUI.Show(_currentBuildingData, _playerGridPosition, buildingRange);
            }
        }
        
        /// <summary>
        /// 건축 모드 종료
        /// </summary>
        public void EndBuildingMode()
        {
            _isBuildingMode = false;
            _currentBuildingItem = null;
            _currentBuildingData = null;
            _manualGridOffset = Vector2Int.zero; // 오프셋 초기화
            
            // 플레이어 이동 활성화
            if (Managers.Data != null && Managers.Data.InGameKeyBinder != null)
            {
                Managers.Data.InGameKeyBinder.PlayerInputEnable();
            }
            
            // 프리뷰 제거
            if (_currentPreview != null)
            {
                Destroy(_currentPreview.gameObject);
                _currentPreview = null;
            }
            
            // UI 숨김
            if (buildingUI != null)
            {
                buildingUI.Hide();
            }
        }
        
        /// <summary>
        /// 프리뷰 업데이트
        /// </summary>
        private void UpdatePreview()
        {
            if (_currentBuildingData == null || _currentPreview == null) return;
            
            Vector2Int targetGridPos;
            
            // 키보드 입력으로 수동 조정 중인지 확인
            bool hasKeyboardInput = HandleKeyboardInput();
            
            if (hasKeyboardInput)
            {
                // 키보드 입력이 있으면 수동 오프셋 사용
                targetGridPos = _playerGridPosition + _manualGridOffset;
            }
            else
            {
                // 마우스 위치에서 레이캐스트
                Ray ray = mainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
                RaycastHit hit;
                
                Vector3 worldPosition = Vector3.zero;
                bool hitGround = false;
                
                if (Physics.Raycast(ray, out hit, 100f, groundLayerMask))
                {
                    worldPosition = hit.point;
                    hitGround = true;
                }
                else
                {
                    // 레이캐스트 실패 시 평면과의 교차점 계산
                    Plane groundPlane = new Plane(Vector3.up, gridSystem.transform.position);
                    if (groundPlane.Raycast(ray, out float distance))
                    {
                        worldPosition = ray.GetPoint(distance);
                        hitGround = true;
                    }
                }
                
                if (!hitGround)
                {
                    // 마우스 레이캐스트 실패 시 수동 오프셋 사용
                    targetGridPos = _playerGridPosition + _manualGridOffset;
                }
                else
                {
                    // 월드 위치를 그리드 위치로 변환
                    Vector2Int mouseGridPos = gridSystem.WorldToGridPosition(worldPosition);
                    
                    // 플레이어 위치 기준으로 상대 위치 계산
                    Vector2Int relativePos = mouseGridPos - _playerGridPosition;
                    
                    // 범위 내로 제한
                    int clampedX = Mathf.Clamp(relativePos.x, -buildingRange, buildingRange);
                    int clampedY = Mathf.Clamp(relativePos.y, -buildingRange, buildingRange);
                    
                    targetGridPos = _playerGridPosition + new Vector2Int(clampedX, clampedY);
                    _manualGridOffset = new Vector2Int(clampedX, clampedY);
                }
            }
            
            // 그리드 범위 체크
            if (!gridSystem.IsValidGridPosition(targetGridPos))
            {
                _canPlace = false;
                Vector3 invalidPos = gridSystem.GridToWorldPosition(targetGridPos);
                if (_currentPreview != null)
                {
                    _currentPreview.SetPosition(invalidPos, false);
                }
                _currentGridPosition = targetGridPos;
                
                // UI 업데이트
                if (buildingUI != null)
                {
                    buildingUI.UpdatePreview(targetGridPos, false);
                }
                return;
            }
            
            // 설치 가능 여부 확인
            _canPlace = CanPlaceBuilding(targetGridPos);
            
            // 그리드 위치에 맞게 월드 위치 조정
            Vector3 snappedWorldPos = gridSystem.GridToWorldPosition(targetGridPos);
            
            _currentGridPosition = targetGridPos;
            
            // 프리뷰 위치 업데이트
            if (_currentPreview != null)
            {
                _currentPreview.SetPosition(snappedWorldPos, _canPlace);
            }
            
            // UI 업데이트
            if (buildingUI != null)
            {
                buildingUI.UpdatePreview(targetGridPos, _canPlace);
            }
        }
        
        /// <summary>
        /// 키보드 입력 처리 (방향키/WASD로 프리뷰 위치 조정)
        /// </summary>
        private bool HandleKeyboardInput()
        {
            bool hasInput = false;
            Vector2Int inputDirection = Vector2Int.zero;
            
            // 방향키 또는 WASD 입력 확인
            if (UnityEngine.Input.GetKeyDown(KeyCode.UpArrow) || UnityEngine.Input.GetKeyDown(KeyCode.W))
            {
                inputDirection.y += 1;
                hasInput = true;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.DownArrow) || UnityEngine.Input.GetKeyDown(KeyCode.S))
            {
                inputDirection.y -= 1;
                hasInput = true;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftArrow) || UnityEngine.Input.GetKeyDown(KeyCode.A))
            {
                inputDirection.x -= 1;
                hasInput = true;
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.RightArrow) || UnityEngine.Input.GetKeyDown(KeyCode.D))
            {
                inputDirection.x += 1;
                hasInput = true;
            }
            
            if (hasInput)
            {
                // 오프셋 업데이트
                _manualGridOffset += inputDirection;
                
                // 범위 제한
                _manualGridOffset.x = Mathf.Clamp(_manualGridOffset.x, -buildingRange, buildingRange);
                _manualGridOffset.y = Mathf.Clamp(_manualGridOffset.y, -buildingRange, buildingRange);
            }
            
            return hasInput;
        }
        
        /// <summary>
        /// 입력 처리
        /// </summary>
        private void HandleInput()
        {
            // 좌클릭 - 설치
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                if (_canPlace)
                {
                    PlaceBuilding();
                }
            }
            
            // ESC 또는 우클릭 - 취소
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) || UnityEngine.Input.GetMouseButtonDown(1))
            {
                EndBuildingMode();
            }
        }
        
        /// <summary>
        /// 건물 설치
        /// </summary>
        private void PlaceBuilding()
        {
            if (_currentBuildingData == null) return;
            
            // maxBuild 제한 확인
            if (!CheckMaxBuildLimit())
            {
                Debug.LogWarning($"최대 건설 개수({_currentBuildingData.maxBuild})에 도달했습니다!");
                return;
            }
            
            // 건물 생성
            GameObject building = BuildingObjectFactory.CreateObjectFromData(
                _currentBuildingData, 
                _currentGridPosition, 
                gridSystem
            );
            
            if (building == null)
            {
                Debug.LogError("건물 생성 실패!");
                return;
            }
            
            // 건설 카운트 증가
            if (!_builtCounts.ContainsKey(_currentBuildingData.id))
            {
                _builtCounts[_currentBuildingData.id] = 0;
            }
            _builtCounts[_currentBuildingData.id]++;
            
            // 인벤토리에서 아이템 소모
            if (inventory != null && _currentItemSlotIndex >= 0)
            {
                bool consumed = inventory.TryConsumeAt(_currentItemSlotIndex, 1);
                if (!consumed)
                {
                    Debug.LogWarning("아이템 소모 실패!");
                    Destroy(building);
                    return;
                }
                
                // 아이템이 모두 소모되었으면 건축 모드 종료
                var slot = inventory.GetSlot(_currentItemSlotIndex);
                if (slot == null || slot.item == null || slot.count <= 0)
                {
                    EndBuildingMode();
                    return;
                }
            }
            
            Debug.Log($"건물 설치 완료: {_currentBuildingData.id} at {_currentGridPosition}");
        }
        
        /// <summary>
        /// 건물 설치 가능 여부 확인
        /// </summary>
        private bool CanPlaceBuilding(Vector2Int gridPosition)
        {
            if (_currentBuildingData == null) return false;
            
            Vector2Int tileSize = _currentBuildingData.TileSize;
            
            // 건물이 차지하는 모든 타일 확인
            for (int x = 0; x < tileSize.x; x++)
            {
                for (int y = 0; y < tileSize.y; y++)
                {
                    Vector2Int checkPos = gridPosition + new Vector2Int(x, y);
                    
                    // 그리드 범위 체크
                    if (!gridSystem.IsValidGridPosition(checkPos))
                    {
                        return false;
                    }
                    
                    // 플레이어 주변 범위 체크 (건물의 각 타일이 범위 내에 있어야 함)
                    int distanceX = Mathf.Abs(checkPos.x - _playerGridPosition.x);
                    int distanceY = Mathf.Abs(checkPos.y - _playerGridPosition.y);
                    int maxDistance = Mathf.Max(distanceX, distanceY);
                    
                    if (maxDistance > buildingRange)
                    {
                        return false;
                    }
                    
                    // 점유 여부 체크 (이미 설치된 오브젝트가 있으면 설치 불가)
                    if (gridSystem.IsCellOccupied(checkPos))
                    {
                        return false;
                    }
                    
                    // 블록 여부 체크
                    if (gridSystem.IsCellBlocked(checkPos))
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 최대 건설 개수 제한 확인
        /// </summary>
        private bool CheckMaxBuildLimit()
        {
            if (_currentBuildingData == null) return true;
            
            if (!_builtCounts.ContainsKey(_currentBuildingData.id))
            {
                return true;
            }
            
            return _builtCounts[_currentBuildingData.id] < _currentBuildingData.maxBuild;
        }
        
        public bool IsBuildingMode => _isBuildingMode;
    }
}

