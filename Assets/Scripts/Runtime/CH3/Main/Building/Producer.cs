using UnityEngine;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 생산소 오브젝트 - isBuilding이 true인 건물에 사용
    /// 상호작용 시 UI를 띄우는 기능 포함
    /// isBlocking이 false일 때 통과 가능하도록 IGridPassable 구현
    /// </summary>
    public class Producer : InteractableGridObject, IGridPassable
    {
        [SerializeField] protected bool isBlocking = true;
        private CH3_LevelData _buildingData;
        private BuildingUI _buildingUI;
        
        /// <summary>
        /// isBlocking이 false이면 통과 가능
        /// </summary>
        public bool IsPassable => !isBlocking;
        
        public override void InitializeFromData(CH3_LevelData data)
        {
            base.InitializeFromData(data);
            _buildingData = data;
            if (data != null)
            {
                isBlocking = data.isBlocking;
            }
        }
        
        public override void Initialize(Vector2Int gridPos)
        {
            base.Initialize(gridPos);
            if (gridManager != null)
            {
                BlockTiles(isBlocking);
            }
        }
        
        protected override void Start()
        {
            base.Start();
            
            if (gridManager == null)
            {
                gridManager = GridSystem.Instance;
            }
            
            if (gridManager != null && GridPosition != Vector2Int.zero)
            {
                if (occupiedTiles == null || occupiedTiles.Count == 0)
                {
                    OccupyTiles(GridPosition);
                }
                BlockTiles(isBlocking);
            }
            
            // BuildingUI 찾아서 초기화
            InitializeBuildingUI();
        }
        
        /// <summary>
        /// BuildingUI를 찾아서 초기화
        /// </summary>
        private void InitializeBuildingUI()
        {
            if (_buildingUI == null)
            {
                _buildingUI = FindObjectOfType<BuildingUI>();
                if (_buildingUI == null)
                {
                    Debug.LogWarning($"Producer '{gameObject.name}': BuildingUI를 찾을 수 없습니다!");
                }
            }
        }
        
        public override void OnInteract(GameObject interactor)
        {
            // BuildingUI가 없으면 다시 찾기 시도
            if (_buildingUI == null)
            {
                InitializeBuildingUI();
            }
            
            // 건축물 관리창 표시
            if (_buildingUI != null)
            {
                _buildingUI.ShowManagementWindow(this);
            }
            else
            {
                Debug.LogWarning($"Producer '{gameObject.name}': BuildingUI를 찾을 수 없어 관리창을 표시할 수 없습니다!");
            }
        }
        
        public override void Remove()
        {
            if (gridManager != null)
            {
                BlockTiles(false);
            }
            base.Remove();
        }
    }
}

