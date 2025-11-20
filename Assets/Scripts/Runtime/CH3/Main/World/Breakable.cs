using UnityEngine;

namespace Runtime.CH3.Main
{
    /// <summary>
    /// 부서질 수 있는 오브젝트 - 채굴 가능한 구조물
    /// </summary>
    public class Breakable : MineableObject
    {
        protected override void Start()
        {
            base.Start();
            Initialize(GridPosition);
        }

        public override void Initialize(Vector2Int gridPos)
        {
            base.Initialize(gridPos);

            // Breakable은 그리드를 차단함 (점유는 base.Initialize에서 처리됨)
            if (GridSystem.Instance != null)
            {
                BlockTiles(true);
            }
        }

        protected override void OnMiningComplete()
        {
            if (GridSystem.Instance != null)
            {
                GridSystem.Instance.OnMineralRemoved(ObjectType);
            }
        }

        public override void Remove()
        {
            if (GridSystem.Instance != null)
            {
                BlockTiles(false);
            }
            base.Remove();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.1f, 0.4f);
        }
    }
}