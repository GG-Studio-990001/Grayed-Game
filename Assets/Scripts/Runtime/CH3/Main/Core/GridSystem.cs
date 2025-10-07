using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

namespace Runtime.CH3.Main
{
    // 오브젝트 타입 열거형
    public enum GridObjectType
    {
        BlockedArea,
        EventArea,
        Structure,
        Teleporter,
        Ore,
        NPC
    }

    // 그리드 셀 클래스
    public class GridCell
    {
        public Vector2Int GridPosition { get; private set; }
        public Vector3 WorldPosition { get; private set; }
        public bool IsOccupied { get; set; }
        public GameObject OccupyingObject { get; set; }

        public GridCell(Vector2Int gridPos, Vector3 worldPos)
        {
            GridPosition = gridPos;
            WorldPosition = worldPos;
            IsOccupied = false;
            OccupyingObject = null;
        }
    }

    // 영역 정의를 위한 구조체
    [System.Serializable]
    public struct GridArea
    {
        public Vector2Int start;
        public Vector2Int end;
        public string areaId;

        public GridArea(Vector2Int start, Vector2Int end, string areaId)
        {
            this.start = start;
            this.end = end;
            this.areaId = areaId;
        }

        public bool Contains(Vector2Int position)
        {
            return position.x >= start.x && position.x <= end.x &&
                   position.y >= start.y && position.y <= end.y;
        }

        public Vector2Int GetRandomPosition()
        {
            int x = Random.Range(start.x, end.x + 1);
            int y = Random.Range(start.y, end.y + 1);
            return new Vector2Int(x, y);
        }
    }

    // 생성 규칙 정의
    [System.Serializable]
    public class SpawnRule
    {
        public GridObjectType objectType;
        public int minCount;
        public int maxCount;
        [Range(0f, 1f)] public float spawnChance = 1f;
        
        [Header("Animation Settings (Optional)")]
        public bool useAnimation = false;
        public float spawnHeight = 10f;
        public float fallDuration = 1f;
        public float rotationAmount = 360f;
        public GameObject meteorEffect;
        
        [Header("Respawn Settings (Optional)")]
        public bool enableRespawn = false;
        public float respawnDelayMin = 1f;
        public float respawnDelayMax = 3f;
    }

    // 통합된 그리드 시스템 매니저
    public class GridSystem : MonoBehaviour
    {
        public static GridSystem Instance;

        [Header("Grid Settings")]
        [Tooltip("정사각형 그리드 크기. 짝수면 자동으로 홀수로 변환, 최소 9")]
        [Range(9, 50)]
        [SerializeField] private int gridSize = 9; // 정사각형 그리드 크기
        [SerializeField] private float cellWidth = 1f;
        [SerializeField] private float heightOffset = 0.3f;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Color gridLineColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        [System.Serializable]
        private class PrefabData
        {
            public GridObjectType type;
            public GameObject prefab;
        }
        [SerializeField] private List<PrefabData> prefabList;

        [System.Serializable]
        public class AreaDefinition
        {
            public string areaId;
            public Vector2Int start;
            public Vector2Int end;
            public List<SpawnRule> spawnRules;
        }
        [SerializeField] private List<AreaDefinition> areaDefinitions;
        [SerializeField] private bool initializeOnStart = true;

        // Mineral System은 Area Management로 통합됨

        // Core Grid System
        private Dictionary<Vector2Int, GridCell> _gridCells = new Dictionary<Vector2Int, GridCell>();
        private Vector3 _forward;
        private Vector3 _right;
        private Vector2Int _gridCenter;
        private int _actualGridSize; // 실제 그리드 크기 (짝수면 홀수로 변환됨)
        private bool[,] blockedCells;
        private Dictionary<GridObjectType, GameObject> prefabDictionary;

        // Area Management (Mineral System 통합)
        private Dictionary<string, GridArea> areas = new Dictionary<string, GridArea>();
        private Dictionary<string, List<IGridObject>> areaObjects = new Dictionary<string, List<IGridObject>>();
        private Dictionary<string, Dictionary<GridObjectType, int>> areaObjectCounts = new Dictionary<string, Dictionary<GridObjectType, int>>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (initializeOnStart)
            {
                // 씬에 미리 배치된 Structure들이 블록을 설정하도록 대기
                StartCoroutine(InitializeAfterStructures());
            }
        }
        
        private System.Collections.IEnumerator InitializeAfterStructures()
        {
            // 한 프레임 대기하여 모든 Structure의 Start()가 완료되도록 함
            yield return null;
            
            // Structure 블록 설정 완료 후 Area 초기화
            InitializeAreas();
        }

        private void Initialize()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            // 짝수면 홀수로 변환 (사이즈를 하나 내림), 최소 5 보장
            _actualGridSize = (gridSize % 2 == 0) ? gridSize - 1 : gridSize;
            _actualGridSize = Mathf.Max(_actualGridSize, 5); // 최소 5 보장
            
            // 그리드 중심점 계산 (홀수 크기에서 완벽한 중심)
            _gridCenter = new Vector2Int(_actualGridSize / 2, _actualGridSize / 2);
            blockedCells = new bool[_actualGridSize, _actualGridSize];
            
            InitializeCameraAlignment();
            InitializeGrid();
            InitializePrefabDictionary();
        }

        #region Core Grid System
        private void InitializeCameraAlignment()
        {
            _forward = Vector3.ProjectOnPlane(mainCamera.transform.forward, Vector3.up).normalized;
            _right = Vector3.ProjectOnPlane(mainCamera.transform.right, Vector3.up).normalized;
        }

        private void InitializeGrid()
        {
            // 홀수 크기만 지원 (짝수는 자동으로 홀수로 변환됨)
            int halfSize = _actualGridSize / 2;
            int startX = -halfSize;       // -4 (9x9 그리드)
            int startY = -halfSize;       // -4 (9x9 그리드)
            int endX = halfSize;          // 4 (포함)
            int endY = halfSize;          // 4 (포함)

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Vector2Int gridPosition = new Vector2Int(x, y);
                    Vector3 worldPosition = GridToWorldPosition(gridPosition);
                    _gridCells[gridPosition] = new GridCell(gridPosition, worldPosition);
                }
            }
        }

        private void InitializePrefabDictionary()
        {
            prefabDictionary = new Dictionary<GridObjectType, GameObject>();
            foreach (var data in prefabList)
            {
                prefabDictionary[data.type] = data.prefab;
            }
        }

        public Vector3 GridToWorldPosition(Vector2Int gridPos)
        {
            Vector3 basePos = new Vector3(
                gridPos.x * cellWidth,
                heightOffset,
                gridPos.y * cellWidth
            );

            return transform.position +
                   _right * basePos.x +
                   Vector3.up * basePos.y +
                   _forward * basePos.z;
        }

        public Vector2Int WorldToGridPosition(Vector3 worldPos)
        {
            Vector3 relativePos = worldPos - transform.position;

            float x = Vector3.Dot(relativePos, _right) / cellWidth;
            float z = Vector3.Dot(relativePos, _forward) / cellWidth;

            return new Vector2Int(
                Mathf.RoundToInt(x),
                Mathf.RoundToInt(z)
            );
        }

        public bool IsValidGridPosition(Vector2Int gridPos)
        {
            // 홀수 크기만 지원 (짝수는 자동으로 홀수로 변환됨)
            int halfSize = _actualGridSize / 2;
            return gridPos.x >= -halfSize && gridPos.x <= halfSize &&
                   gridPos.y >= -halfSize && gridPos.y <= halfSize;
        }

        public bool IsWithinGridBounds(Vector2Int position)
        {
            // 홀수 크기만 지원 (짝수는 자동으로 홀수로 변환됨)
            int halfSize = _actualGridSize / 2;
            return position.x >= -halfSize && position.x <= halfSize &&
                   position.y >= -halfSize && position.y <= halfSize;
        }

        public GridCell GetCell(Vector2Int gridPos)
        {
            return _gridCells.TryGetValue(gridPos, out GridCell cell) ? cell : null;
        }

        public bool IsCellBlocked(Vector2Int position)
        {
            if (!IsWithinGridBounds(position)) 
            {
                Debug.LogWarning($"IsCellBlocked: 위치 {position}가 그리드 범위를 벗어남");
                return true;
            }
            Vector2Int arrayIndex = ToArrayIndex(position);
            bool blocked = blockedCells[arrayIndex.x, arrayIndex.y];
            return blocked;
        }

        public bool IsCellOccupied(Vector2Int position)
        {
            if (!IsWithinGridBounds(position)) return true;
            
            // 그리드 셀에 오브젝트가 있는지 확인
            if (_gridCells.TryGetValue(position, out GridCell cell))
            {
                return cell.IsOccupied && cell.OccupyingObject != null;
            }
            
            return false;
        }

        public void SetCellBlocked(Vector2Int position, bool blocked)
        {
            if (!IsWithinGridBounds(position)) 
            {
                Debug.LogWarning($"SetCellBlocked: 위치 {position}가 그리드 범위를 벗어남");
                return;
            }
            Vector2Int arrayIndex = ToArrayIndex(position);
            blockedCells[arrayIndex.x, arrayIndex.y] = blocked;
        }

        public void SetCellOccupied(Vector2Int position, bool occupied, GameObject occupyingObject = null)
        {
            if (!IsWithinGridBounds(position)) return;
            
            if (_gridCells.TryGetValue(position, out GridCell cell))
            {
                cell.IsOccupied = occupied;
                cell.OccupyingObject = occupied ? occupyingObject : null;
            }
        }

        public Vector2Int ToArrayIndex(Vector2Int position)
        {
            // 그리드 좌표를 배열 인덱스로 변환
            // 홀수 크기만 지원 (짝수는 자동으로 홀수로 변환됨)
            int halfSize = _actualGridSize / 2;
            return new Vector2Int(position.x + halfSize, position.y + halfSize);
        }

        public Vector2Int GridCenter => _gridCenter;

        // 중심 좌표 (0,0) 얻기 - y값을 유지하도록 수정
        public Vector3 GetCenterPosition(float customHeight)
        {
            Vector3 centerPos = GridToWorldPosition(Vector2Int.zero);
            centerPos.y = customHeight; // 원하는 높이 값 사용
            return centerPos;
        }

        // 오버로드 - 현재 오브젝트의 높이를 유지하고 싶을 때
        public Vector3 GetCenterPosition(Transform targetTransform)
        {
            Vector3 centerPos = GridToWorldPosition(Vector2Int.zero);
            centerPos.y = targetTransform.position.y; // 현재 오브젝트의 높이 유지
            return centerPos;
        }

        public void RecalculateGridAlignment()
        {
            InitializeCameraAlignment();
            InitializeGrid();
        }
        #endregion

        #region Object Factory
        public IGridObject CreateObject(GridObjectType type, Vector2Int gridPosition)
        {
            if (!prefabDictionary.TryGetValue(type, out GameObject prefab))
            {
                Debug.LogError($"Prefab not found for type: {type}");
                return null;
            }

            GameObject instance = Instantiate(prefab);
            IGridObject gridObject = instance.GetComponent<IGridObject>();
            
            if (gridObject == null)
            {
                Debug.LogError($"Created object does not implement IGridObject: {type}");
                Destroy(instance);
                return null;
            }

            gridObject.Initialize(gridPosition);
            
            // 그리드 셀을 점유 상태로 설정 (Structure는 제외 - 자체적으로 처리)
            if (!(gridObject is Structure))
            {
                SetCellOccupied(gridPosition, true, instance);
            }
            
            return gridObject;
        }
        #endregion

        #region Area Management (통합된 스폰 시스템)
        public void InitializeAreas()
        {
            foreach (var areaDef in areaDefinitions)
            {
                GridArea area = new GridArea(areaDef.start, areaDef.end, areaDef.areaId);
                
                if (HasArea(areaDef.areaId))
                {
                    ClearArea(areaDef.areaId);
                }

                RegisterArea(area);
                PopulateArea(areaDef.areaId, areaDef.spawnRules);
            }
        }

        public void RegisterArea(GridArea area)
        {
            if (!IsWithinGridBounds(area.start) || !IsWithinGridBounds(area.end))
            {
                Debug.LogError($"영역이 그리드 범위를 벗어났습니다: {area.areaId}");
                return;
            }

            areas[area.areaId] = area;
            if (!areaObjects.ContainsKey(area.areaId))
            {
                areaObjects[area.areaId] = new List<IGridObject>();
            }
            if (!areaObjectCounts.ContainsKey(area.areaId))
            {
                areaObjectCounts[area.areaId] = new Dictionary<GridObjectType, int>();
            }
        }

        public void PopulateArea(string areaId, List<SpawnRule> spawnRules)
        {
            if (!areas.ContainsKey(areaId))
            {
                Debug.LogError($"등록되지 않은 영역입니다: {areaId}");
                return;
            }

            GridArea area = areas[areaId];
            HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();

            // 기존 블록된 셀들 추가
            for (int x = area.start.x; x <= area.end.x; x++)
            {
                for (int y = area.start.y; y <= area.end.y; y++)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (IsCellBlocked(pos))
                    {
                        occupiedPositions.Add(pos);
                    }
                }
            }

            foreach (var rule in spawnRules)
            {
                // 현재 수량 확인
                if (!areaObjectCounts[areaId].ContainsKey(rule.objectType))
                    areaObjectCounts[areaId][rule.objectType] = 0;
                
                int currentCount = areaObjectCounts[areaId][rule.objectType];
                int targetCount = Random.Range(rule.minCount, rule.maxCount + 1);
                int remainingToSpawn = targetCount - currentCount;

                for (int i = 0; i < remainingToSpawn; i++)
                {
                    if (Random.value > rule.spawnChance) continue;

                    Vector2Int position = GetValidSpawnPosition(area, occupiedPositions);
                    if (position != Vector2Int.one * -1)
                    {
                        if (rule.useAnimation)
                        {
                            SpawnObjectWithAnimation(rule, position, areaId);
                        }
                        else
                        {
                            var obj = CreateObject(rule.objectType, position);
                            if (obj != null)
                            {
                                areaObjects[areaId].Add(obj);
                                areaObjectCounts[areaId][rule.objectType]++;
                                occupiedPositions.Add(position);
                            }
                        }
                    }
                }
            }
        }

        private Vector2Int GetValidSpawnPosition(GridArea area, HashSet<Vector2Int> occupiedPositions)
        {
            int maxAttempts = 100;
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                Vector2Int position = area.GetRandomPosition();

                // 1. 블록된 셀인지 확인
                if (IsCellBlocked(position))
                {
                    attempts++;
                    continue;
                }

                // 2. 이미 점유된 위치인지 확인 (항상 오브젝트가 있는 곳은 피함)
                // 현재 스폰 중인 위치들 확인
                if (occupiedPositions.Contains(position))
                {
                    attempts++;
                    continue;
                }

                // 실제 그리드에 있는 오브젝트들 확인
                if (IsCellOccupied(position))
                {
                    attempts++;
                    continue;
                }

                return position;
            }

            Debug.LogWarning($"유효한 생성 위치를 찾을 수 없습니다. (최대 시도 횟수: {maxAttempts})");
            return Vector2Int.one * -1;
        }

        public void ClearArea(string areaId)
        {
            if (!areaObjects.ContainsKey(areaId)) return;

            foreach (var obj in areaObjects[areaId].ToList())
            {
                if (obj != null)
                {
                    // 그리드 셀을 비워줌
                    SetCellOccupied(obj.GridPosition, false);
                    obj.Remove();
                }
            }

            areaObjects[areaId].Clear();
            if (areaObjectCounts.ContainsKey(areaId))
            {
                areaObjectCounts[areaId].Clear();
            }
        }

        public void ClearAreaByType(string areaId, GridObjectType type)
        {
            if (!areaObjects.ContainsKey(areaId)) return;

            var objectsToRemove = areaObjects[areaId]
                .Where(obj => obj.GameObject.GetComponent<GridObject>()?.GetType().Name == type.ToString())
                .ToList();

            foreach (var obj in objectsToRemove)
            {
                // 그리드 셀을 비워줌
                SetCellOccupied(obj.GridPosition, false);
                obj.Remove();
                areaObjects[areaId].Remove(obj);
            }

            // 카운트 업데이트
            if (areaObjectCounts.ContainsKey(areaId) && areaObjectCounts[areaId].ContainsKey(type))
            {
                areaObjectCounts[areaId][type] = 0;
            }
        }

        public int GetObjectCount(string areaId, GridObjectType? type = null)
        {
            if (!areaObjects.ContainsKey(areaId)) return 0;

            if (type.HasValue)
            {
                // areaObjectCounts에서 직접 가져오기 (더 효율적)
                if (areaObjectCounts.ContainsKey(areaId) && areaObjectCounts[areaId].ContainsKey(type.Value))
                {
                    return areaObjectCounts[areaId][type.Value];
                }
                return 0;
            }

            return areaObjects[areaId].Count;
        }

        public bool HasArea(string areaId)
        {
            return areas.ContainsKey(areaId);
        }

        public GridArea? GetArea(string areaId)
        {
            return areas.TryGetValue(areaId, out GridArea area) ? area : null;
        }

        public void ResetArea(string areaId)
        {
            var areaDef = areaDefinitions.Find(a => a.areaId == areaId);
            if (areaDef != null)
            {
                ClearArea(areaId);
                PopulateArea(areaId, areaDef.spawnRules);
            }
        }

        public void ResetAllAreas()
        {
            foreach (var areaDef in areaDefinitions)
            {
                ResetArea(areaDef.areaId);
            }
        }
        #endregion

        // 애니메이션과 함께 오브젝트 스폰
        private void SpawnObjectWithAnimation(SpawnRule rule, Vector2Int position, string areaId)
        {
            // 오브젝트를 먼저 생성하여 GridObject.Initialize가 customY 등을 적용하도록 함
            var obj = CreateObject(rule.objectType, position);
            if (obj == null) return;

            var objGameObject = obj.GameObject;

            // Initialize()가 반영한 최종 목표 월드 좌표(특히 useCustomY)를 기준으로 애니메이션 타깃 설정
            Vector3 targetWorldPosition = objGameObject.transform.position;
            Vector3 spawnStartPosition = targetWorldPosition + Vector3.up * rule.spawnHeight;

            // 메테오 이펙트 생성
            if (rule.meteorEffect != null)
            {
                GameObject effect = Instantiate(rule.meteorEffect, spawnStartPosition, Quaternion.identity);
                effect.transform.DOMove(targetWorldPosition, rule.fallDuration)
                    .SetEase(Ease.OutBounce)
                    .OnComplete(() => Destroy(effect));
            }

            // 시작 위치로 올려놓고 낙하 애니메이션
            objGameObject.transform.position = spawnStartPosition;

            Sequence spawnSequence = DOTween.Sequence();
            spawnSequence.Join(objGameObject.transform.DOMove(targetWorldPosition, rule.fallDuration)
                .SetEase(Ease.OutBounce));
            spawnSequence.Join(objGameObject.transform.DORotate(
                new Vector3(0, rule.rotationAmount, 0),
                rule.fallDuration,
                RotateMode.FastBeyond360));

            spawnSequence.OnComplete(() => {
                areaObjects[areaId].Add(obj);
                areaObjectCounts[areaId][rule.objectType]++;
                var collider = objGameObject.GetComponent<Collider>();
                if (collider != null) collider.enabled = true;
            });
        }

        // 오브젝트 제거 시 리스폰 처리
        public void OnObjectRemoved(string areaId, GridObjectType objectType)
        {
            if (!areaObjectCounts.ContainsKey(areaId) || !areaObjectCounts[areaId].ContainsKey(objectType))
                return;

            areaObjectCounts[areaId][objectType]--;

            // 해당 영역의 스폰 규칙에서 리스폰 설정 확인
            var areaDef = areaDefinitions.Find(ad => ad.areaId == areaId);
            if (areaDef != null)
            {
                var rule = areaDef.spawnRules.Find(r => r.objectType == objectType && r.enableRespawn);
                if (rule != null && Random.value <= rule.spawnChance)
                {
                    float delay = Random.Range(rule.respawnDelayMin, rule.respawnDelayMax);
                    StartCoroutine(DelayedObjectSpawn(rule, areaId, delay));
                }
            }
        }

        private System.Collections.IEnumerator DelayedObjectSpawn(SpawnRule rule, string areaId, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (areas.ContainsKey(areaId))
            {
                // 현재 영역의 모든 점유된 위치들을 수집
                HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();
                if (areaObjects.ContainsKey(areaId))
                {
                    foreach (var obj in areaObjects[areaId])
                    {
                        if (obj != null)
                        {
                            occupiedPositions.Add(obj.GridPosition);
                        }
                    }
                }
                
                Vector2Int position = GetValidSpawnPosition(areas[areaId], occupiedPositions);
                if (position != Vector2Int.one * -1)
                {
                    if (rule.useAnimation)
                    {
                        SpawnObjectWithAnimation(rule, position, areaId);
                    }
                    else
                    {
                        var obj = CreateObject(rule.objectType, position);
                        if (obj != null)
                        {
                            areaObjects[areaId].Add(obj);
                            areaObjectCounts[areaId][rule.objectType]++;
                        }
                    }
                }
            }
        }

        // 기존 OnMineralRemoved 호환성을 위한 래퍼
        public void OnMineralRemoved(GridObjectType mineralType)
        {
            // 모든 영역에서 해당 타입의 오브젝트가 제거되었는지 확인
            foreach (var areaId in areaObjectCounts.Keys)
            {
                OnObjectRemoved(areaId, mineralType);
            }
        }

        #region Debug Visualization
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            // 그리드 라인 그리기
            Gizmos.color = gridLineColor;
            int halfSize = _actualGridSize / 2;
            int startX = -halfSize;
            int startY = -halfSize;
            int endX = halfSize;
            int endY = halfSize;

            for (int x = startX; x <= endX; x++)
            {
                Vector3 startPos = GridToWorldPosition(new Vector2Int(x, startY));
                Vector3 endPos = GridToWorldPosition(new Vector2Int(x, endY));
                Gizmos.DrawLine(startPos, endPos);
            }

            for (int y = startY; y <= endY; y++)
            {
                Vector3 startPos = GridToWorldPosition(new Vector2Int(startX, y));
                Vector3 endPos = GridToWorldPosition(new Vector2Int(endX, y));
                Gizmos.DrawLine(startPos, endPos);
            }

            // 각 셀의 중심점과 좌표 표시
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Vector2Int gridPosition = new Vector2Int(x, y);
                    Vector3 worldPosition = GridToWorldPosition(gridPosition);

                    // 차단된 셀 표시
                    if (IsCellBlocked(gridPosition))
                    {
                        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
                        Gizmos.DrawCube(worldPosition, new Vector3(0.9f, 0.05f, 0.9f));
                        Gizmos.color = Color.black;
                        Gizmos.DrawWireCube(worldPosition, new Vector3(0.9f, 0.05f, 0.9f));
                    }
                    // (0,0) 좌표는 다른 색상으로 표시
                    else if (gridPosition == Vector2Int.zero)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireSphere(worldPosition, 0.15f);
                    }
                    else
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawWireSphere(worldPosition, 0.1f);
                    }

// #if UNITY_EDITOR
//                     UnityEditor.Handles.Label(worldPosition,
//                         $"({gridPosition.x}, {gridPosition.y})");
// #endif
                }
            }

            // 그리드 방향 표시
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, _forward * 2f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, _right * 2f);
        }

        private void OnDrawGizmosSelected()
        {
            OnDrawGizmos();
        }
        #endregion
    }
}
