using UnityEngine;
using System.Collections.Generic;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 레벨 설치 데이터
    /// 레벨 내 설치되는 구조물, 레벨 내 배치되는 아트 리소스에 대한 데이터
    /// 그리드 타일 에디터 툴에서 사용
    /// </summary>
    [CreateAssetMenu(fileName = "NewLevelData", menuName = "CH3/Level Data", order = 1)]
    public class CH3_LevelData : ScriptableObject
    {
        [Tooltip("데이터 구분을 위한 고유 ID")]
        public string id;
        
        [Tooltip("개발/기획자 확인용 메모 (개발용 X)")]
        [TextArea(2, 4)]
        public string dev;
        
        [Tooltip("해당 건물의 설치 사이즈 X (타일 단위)")]
        public int sizeX = 1;
        
        [Tooltip("해당 건물의 설치 사이즈 Y (타일 단위)")]
        public int sizeY = 1;
        
        [Tooltip("해당 건물의 스프라이트 (필드 내 설치)")]
        public Sprite sprite;
        
        [Tooltip("해당 건물의 UI 프리팹 경로 (UI로 보일)")]
        public GameObject uiPrefab;
        
        [Tooltip("그리드 오브젝트 타입 (Structure 선택 시 isBreakable로 Breakable/Structure 자동 판단)")]
        public GridObjectType objectType = GridObjectType.Structure;
        
        [Tooltip("건설 가능한지?")]
        public bool isBuilding = false;
        
        [ConditionalHide("isBuilding")]
        [Tooltip("최대 건설 가능 개수")]
        public int maxBuild = 1;
        
        [ConditionalHide("isBuilding")]
        [Tooltip("건설 시 필요한 재화, 개수 (배열로 있어, 중복 적용 가능)")]
        public List<CurrencyData> buildCurrency = new List<CurrencyData>();
        
        [Tooltip("생산할 아이템과 개수 (배열로 있어, 중복 적용 가능) - Is Building이 true일 때만 사용")]
        public List<ItemProductionData> productionItems = new List<ItemProductionData>();
        
        [Tooltip("생산 간격 (초) - Is Building이 true일 때만 사용")]
        public float productionInterval = 5f;
        
        [Tooltip("최대 생산 개수 (건물에 저장 가능한 최대 생산물 개수) - Is Building이 true일 때만 사용")]
        public int maxProduction = 10;
        
        [Tooltip("타일을 차단하는지 (Structure 전용)")]
        public bool isBlocking = true;
        
        [Tooltip("부술 수 있는지 (MineableObject 전용)")]
        public bool isBreakable = false;
        
        [ConditionalHide("isBreakable")]
        [Tooltip("부술 경우 드랍되는 리소스의 총 양")]
        public int maxDrop = 0;
        
        [ConditionalHide("isBreakable")]
        [Tooltip("드랍되는 재화, 개수, 확률 (배열로 있어, 중복 적용 가능)")]
        public List<CurrencyData> dropCurrency = new List<CurrencyData>();
        
        [ConditionalHide("isBreakable")]
        [Tooltip("채굴 단계 수 (MineableObject 전용)")]
        public int maxMiningCount = 1;
        
        [ConditionalHide("isBreakable")]
        [Tooltip("채굴 단계별 스프라이트 (MineableObject 전용)")]
        public Sprite[] miningStageSprites;
        
        [ConditionalHide("isBreakable")]
        [Tooltip("드랍 아이템 프리팹 (MineableObject 전용)")]
        public GameObject itemPrefab;
        
        [ConditionalHide("isBreakable")]
        [Tooltip("최소 드랍 개수 (MineableObject 전용)")]
        public int minDropCount = 1;
        
        [ConditionalHide("isBreakable")]
        [Tooltip("최대 드랍 개수 (MineableObject 전용)")]
        public int maxDropCount = 3;
        
        [ConditionalHide("isBreakable")]
        [Tooltip("드랍 반경 (MineableObject 전용)")]
        public float dropRadius = 1f;
        
        [Tooltip("리스폰이 가능한지")]
        public bool isRespawn = false;
        
        [Tooltip("리스폰 최소 딜레이 (초)")]
        public float respawnDelayMin = 1f;
        
        [Tooltip("리스폰 최대 딜레이 (초)")]
        public float respawnDelayMax = 3f;
        
        [Tooltip("그리드 위치 초기화 방식")]
        public GridObject.GridPositionInitializationMode gridPositionMode = GridObject.GridPositionInitializationMode.UseNearestGridPosition;
        
        [Tooltip("인스펙터에서 설정할 그리드 위치 (UseInspectorPosition일 때만 사용)")]
        public Vector2Int gridPosition = Vector2Int.zero;
        
        [Tooltip("커스텀 Y 위치 사용 여부")]
        public bool useCustomY = false;
        
        [Tooltip("커스텀 Y 위치 값")]
        public float customY = 0.53f;
        
        [Tooltip("그리드 기반 초기 정렬 순서 적용")]
        public bool applyInitialGridSorting = true;
        
        [Tooltip("그리드 정렬 스케일")]
        public int gridSortingScale = 1;
        
        [Tooltip("상호작용 범위 (InteractableGridObject 전용)")]
        public float interactionRange = 2f;
        
        [Tooltip("시작 시 Collider 활성화 (MineableObject 전용)")]
        public bool enableColliderOnStart = true;
        
        /// <summary>
        /// 타일 크기를 Vector2Int로 반환
        /// </summary>
        public Vector2Int TileSize => new Vector2Int(sizeX, sizeY);
        
        /// <summary>
        /// 데이터 유효성 검사
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning($"CH3_LevelData: id가 비어있습니다!");
                return false;
            }
            
            if (sprite == null)
            {
                Debug.LogWarning($"CH3_LevelData '{id}': sprite가 설정되지 않았습니다!");
                return false;
            }
            
            if (sizeX < 1 || sizeY < 1)
            {
                Debug.LogWarning($"CH3_LevelData '{id}': sizeX, sizeY는 1 이상이어야 합니다!");
                return false;
            }
            
            return true;
        }
    }
}

