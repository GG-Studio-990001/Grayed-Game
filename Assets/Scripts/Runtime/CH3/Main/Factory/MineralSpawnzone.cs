using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening; // DOTween 사용을 위한 using

namespace Runtime.CH3.Main
{
    public class MineralSpawnZone : MonoBehaviour
    {
        [System.Serializable]
        public class MineralSpawnData
        {
            public GridObjectType mineralType;
            public int maxCount;
            [Range(0f, 1f)]
            public float spawnChance = 1f;
        }

        [Header("Spawn Settings")]
        [SerializeField] private Vector2Int zoneStart;
        [SerializeField] private Vector2Int zoneEnd;
        [SerializeField] private List<MineralSpawnData> mineralSettings;
        
        [Header("Spawn Animation")]
        [SerializeField] private float spawnHeight = 10f;        // 생성 시작 높이
        [SerializeField] private float fallDuration = 1f;        // 낙하 시간
        [SerializeField] private float rotationAmount = 360f;    // 회전량
        [SerializeField] private Ease fallEase = Ease.OutBounce; // 낙하 이징
        [SerializeField] private GameObject meteorEffect;        // 메테오 이펙트 프리팹
        
        private Dictionary<GridObjectType, int> currentMineralCounts;
        private GridArea spawnArea;

        private void Start()
        {
            InitializeSpawnZone();
            StartInitialSpawn();
        }

        private void InitializeSpawnZone()
        {
            spawnArea = new GridArea(zoneStart, zoneEnd, "mineral_zone");
            GridAreaSpawner.Instance.RegisterArea(spawnArea);
            
            currentMineralCounts = new Dictionary<GridObjectType, int>();
            foreach (var setting in mineralSettings)
            {
                currentMineralCounts[setting.mineralType] = 0;
            }
        }

        private void StartInitialSpawn()
        {
            foreach (var setting in mineralSettings)
            {
                int remainingToSpawn = setting.maxCount - currentMineralCounts[setting.mineralType];
                for (int i = 0; i < remainingToSpawn; i++)
                {
                    SpawnMineralWithAnimation(setting.mineralType);
                }
            }
        }

        // 광물이 제거될 때 호출되는 메서드
        public void OnMineralRemoved(GridObjectType mineralType)
        {
            if (currentMineralCounts.ContainsKey(mineralType))
            {
                currentMineralCounts[mineralType]--;
                //Debug.Log($"Mineral removed. Current count: {currentMineralCounts[mineralType]}");
        
                var setting = mineralSettings.Find(s => s.mineralType == mineralType);
        
                if (setting != null && Random.value <= setting.spawnChance)
                {
                    float delay = Random.Range(1f, 3f);
                    //Debug.Log($"Scheduling new spawn in {delay} seconds");
                    StartCoroutine(DelayedSpawn(mineralType, delay));
                }
            }
        }

        private IEnumerator DelayedSpawn(GridObjectType mineralType, float delay)
        {
            //Debug.Log($"Starting delayed spawn countdown: {delay}");
            yield return new WaitForSeconds(delay);
            //Debug.Log("Spawning new mineral");
            SpawnMineralWithAnimation(mineralType);
        }

        private void SpawnMineralWithAnimation(GridObjectType mineralType)
        {
            Vector2Int spawnPosition = GetValidSpawnPosition();
            if (spawnPosition == Vector2Int.one * -1) return;

            Vector3 worldPosition = GridManager.Instance.GridToWorldPosition(spawnPosition);
            Vector3 spawnStartPosition = worldPosition + Vector3.up * spawnHeight;

            // 메테오 이펙트 생성
            if (meteorEffect != null)
            {
                GameObject effect = Instantiate(meteorEffect, spawnStartPosition, Quaternion.identity);
                effect.transform.DOMove(worldPosition, fallDuration)
                    .SetEase(fallEase)
                    .OnComplete(() => Destroy(effect));
            }

            // 광물 오브젝트 생성
            var mineral = GridObjectFactory.Instance.CreateObject(mineralType, spawnPosition);
            if (mineral != null)
            {
                var mineralObj = mineral.GameObject;
                mineralObj.transform.position = spawnStartPosition;

                // 낙하 애니메이션
                Sequence spawnSequence = DOTween.Sequence();
                
                spawnSequence.Join(mineralObj.transform.DOMove(worldPosition, fallDuration)
                    .SetEase(fallEase));
                
                spawnSequence.Join(mineralObj.transform.DORotate(
                    new Vector3(0, rotationAmount, 0), 
                    fallDuration, 
                    RotateMode.FastBeyond360));

                spawnSequence.OnComplete(() => {
                    // 카운트 증가 및 콜라이더 활성화
                    currentMineralCounts[mineralType]++;
                    var collider = mineralObj.GetComponent<Collider>();
                    if (collider != null) collider.enabled = true;
                });
            }
        }

        private Vector2Int GetValidSpawnPosition()
        {
            int maxAttempts = 100;
            int attempts = 0;

            while (attempts < maxAttempts)
            {
                Vector2Int position = spawnArea.GetRandomPosition();
                
                if (!GridManager.Instance.IsCellBlocked(position))
                {
                    return position;
                }
                
                attempts++;
            }

            Debug.LogWarning("유효한 생성 위치를 찾을 수 없습니다.");
            return Vector2Int.one * -1;
        }

        // // 디버그용 기즈모
        // private void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.yellow;
        //     Vector3 center = GridManager.Instance.GridToWorldPosition(
        //         (zoneStart + zoneEnd) / 2
        //     );
        //     Vector3 size = new Vector3(
        //         (zoneEnd.x - zoneStart.x + 1) * GridManager.Instance.,
        //         1f,
        //         (zoneEnd.y - zoneStart.y + 1) * GridManager.Instance.CellSize
        //     );
        //     Gizmos.DrawWireCube(center, size);
        // }
    }
}