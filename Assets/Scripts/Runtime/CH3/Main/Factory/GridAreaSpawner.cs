using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Runtime.CH3.Main
{
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
        public bool allowOverlap = false;
    }

    // 영역 기반 오브젝트 생성 관리자
    public class GridAreaSpawner : MonoBehaviour
    {
        private static GridAreaSpawner instance;
        public static GridAreaSpawner Instance => instance;

        [SerializeField] private GridManager gridManager;
        [SerializeField] private GridObjectFactory objectFactory;

        private Dictionary<string, GridArea> areas = new Dictionary<string, GridArea>();
        private Dictionary<string, List<IGridObject>> areaObjects = new Dictionary<string, List<IGridObject>>();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                ValidateReferences();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void ValidateReferences()
        {
            if (gridManager == null)
                gridManager = GridManager.Instance;
            if (objectFactory == null)
                objectFactory = GridObjectFactory.Instance;

            if (gridManager == null || objectFactory == null)
            {
                Debug.LogError("필수 컴포넌트가 없습니다. GridManager와 GridObjectFactory가 필요합니다.");
            }
        }

        // 새로운 영역 등록
        public void RegisterArea(GridArea area)
        {
            // 영역이 그리드 범위 내에 있는지 확인
            if (!gridManager.IsWithinGridBounds(area.start) || !gridManager.IsWithinGridBounds(area.end))
            {
                Debug.LogError($"영역이 그리드 범위를 벗어났습니다: {area.areaId}");
                return;
            }

            areas[area.areaId] = area;
            if (!areaObjects.ContainsKey(area.areaId))
            {
                areaObjects[area.areaId] = new List<IGridObject>();
            }
        }

        // 영역 내 랜덤 위치에 오브젝트 생성
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
                    if (gridManager.IsCellBlocked(pos))
                    {
                        occupiedPositions.Add(pos);
                    }
                }
            }

            foreach (var rule in spawnRules)
            {
                int count = Random.Range(rule.minCount, rule.maxCount + 1);

                for (int i = 0; i < count; i++)
                {
                    if (Random.value > rule.spawnChance) continue;

                    Vector2Int position = GetValidSpawnPosition(area, occupiedPositions, rule.allowOverlap);
                    if (position != Vector2Int.one * -1)
                    {
                        var obj = objectFactory.CreateObject(rule.objectType, position);
                        if (obj != null)
                        {
                            areaObjects[areaId].Add(obj);
                            if (!rule.allowOverlap)
                            {
                                occupiedPositions.Add(position);
                            }
                        }
                    }
                }
            }
        }

        private Vector2Int GetValidSpawnPosition(GridArea area, HashSet<Vector2Int> occupiedPositions,
            bool allowOverlap)
        {
            int maxAttempts = 100;
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                Vector2Int position = area.GetRandomPosition();

                if (allowOverlap || !occupiedPositions.Contains(position))
                {
                    if (!gridManager.IsCellBlocked(position))
                    {
                        return position;
                    }
                }

                attempts++;
            }

            Debug.LogWarning($"유효한 생성 위치를 찾을 수 없습니다. (최대 시도 횟수: {maxAttempts})");
            return Vector2Int.one * -1;
        }

        // 영역 내 모든 오브젝트 제거
        public void ClearArea(string areaId)
        {
            if (!areaObjects.ContainsKey(areaId)) return;

            foreach (var obj in areaObjects[areaId].ToList())
            {
                if (obj != null)
                {
                    obj.Remove();
                }
            }

            areaObjects[areaId].Clear();
        }

// 특정 타입의 오브젝트만 제거
        public void ClearAreaByType(string areaId, GridObjectType type)
        {
            if (!areaObjects.ContainsKey(areaId)) return;

            var objectsToRemove = areaObjects[areaId]
                .Where(obj => obj.GameObject.GetComponent<GridObject>()?.GetType().Name == type.ToString())
                .ToList();

            foreach (var obj in objectsToRemove)
            {
                obj.Remove();
                areaObjects[areaId].Remove(obj);
            }
        }

// 영역 내 오브젝트 수 조회
        public int GetObjectCount(string areaId, GridObjectType? type = null)
        {
            if (!areaObjects.ContainsKey(areaId)) return 0;

            if (type.HasValue)
            {
                return areaObjects[areaId].Count(obj => 
                    obj.GameObject.GetComponent<GridObject>()?.GetType().Name == type.Value.ToString());
            }

            return areaObjects[areaId].Count;
        }

        // 영역 존재 여부 확인
        public bool HasArea(string areaId)
        {
            return areas.ContainsKey(areaId);
        }

        // 영역 정보 조회
        public GridArea? GetArea(string areaId)
        {
            return areas.TryGetValue(areaId, out GridArea area) ? area : null;
        }
    }
}