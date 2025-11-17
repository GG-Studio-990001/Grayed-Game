using UnityEngine;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// Structure는 isBlocking이 false일 때 통과 가능하도록 IGridPassable 구현
    /// </summary>
    public class Structure : GridObject, IGridPassable
    {
        [SerializeField] protected bool isBlocking = true;
        
        /// <summary>
        /// isBlocking이 false이면 통과 가능
        /// </summary>
        public bool IsPassable => !isBlocking;

        public override void Initialize(Vector2Int gridPos)
        {
            // base.Initialize에서 gridPositionMode에 따라 위치 결정
            base.Initialize(gridPos);

            // isBlocking 상태에 따라 차단 설정/해제
            if (gridManager != null)
            {
                BlockTiles(isBlocking);
            }
        }

        // 에디터에서 직접 배치된 경우를 위해 Start 시에도 차단 정보를 보강한다
        protected override void Start()
        {
            base.Start();
            
            // Initialize가 호출되지 않은 경우(씬에 직접 배치된 오브젝트)를 대비
            if (gridManager == null)
            {
                gridManager = GridSystem.Instance;
            }
            
            if (gridManager != null && GridPosition != Vector2Int.zero)
            {
                // 타일이 점유되지 않았다면 점유
                if (occupiedTiles == null || occupiedTiles.Count == 0)
                {
                    OccupyTiles(GridPosition);
                }
                
                // isBlocking 상태에 따라 차단 설정/해제
                BlockTiles(isBlocking);
            }
        }
        
        /// <summary>
        /// 인스펙터에서 isBlocking 값이 변경될 때 호출됨
        /// </summary>
        private void OnValidate()
        {
            // 에디터에서만 동작 (플레이 모드가 아니고, 씬에 배치된 오브젝트)
            if (!Application.isPlaying) return;
            
            if (gridManager != null && occupiedTiles != null && occupiedTiles.Count > 0)
            {
                // isBlocking 상태에 따라 차단 설정/해제
                BlockTiles(isBlocking);
            }
        }

        public override void Remove()
        {
            // 오브젝트 제거 시 항상 차단 해제 (isBlocking 상태와 관계없이)
            if (gridManager != null)
            {
                BlockTiles(false);
            }
            base.Remove();
        }
    }
}