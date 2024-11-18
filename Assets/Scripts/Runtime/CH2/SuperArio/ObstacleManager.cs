using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class ObstacleManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] obstacles; // 장애물 프리팹 배열
        private List<GameObject> _obstaclePool = new List<GameObject>();
        private int _poolObjCnt = 3;
        
        private int _obstacleTypeCnt = 3;
        private float _spawnDelay = 1.5f; // 스폰 간격 (기본값)
        private int _remainingSpawnCount = 20; // 남은 스폰 카운트

        private void SetStageData(string stage, ObstacleSpawnDataSet dataSet)
        {
            // 데이터셋에서 스테이지 데이터 가져오기
            var stageData = dataSet.DataList.Find(data => data.Stage == stage);
            if (stageData == null)
            {
                Debug.LogWarning($"Stage data for {stage} not found.");
                return;
            }

            _obstacleTypeCnt = stageData.ObstacleTypes; // 장애물 종류 수
            _spawnDelay = 3.0f / stageData.Speed; // 스폰 간격 계산 (속도 반비례)
            _remainingSpawnCount = stageData.ObstacleCount; // 남은 스폰 카운트 초기화
        }

        private void PlayGame()
        {
            // 기존 장애물 제거
            if (_obstaclePool.Count > 0)
            {
                _obstaclePool.ForEach(Destroy);
                _obstaclePool.Clear();
            }

            // 장애물 풀 초기화
            for (int i = 0; i < _obstacleTypeCnt; i++)
            {
                for (int j = 0; j < _poolObjCnt; j++)
                {
                    _obstaclePool.Add(CreateObj(obstacles[i], transform));
                }
            }

            for (int index = 0; index < _obstaclePool.Count; index++)
            {
                GameObject item = _obstaclePool[index];
                if (item.activeSelf)
                    item.SetActive(false);
            }

            StartCoroutine(CreateObstacle());
        }

        private IEnumerator CreateObstacle()
        {
            yield return new WaitForSeconds(0.5f); // 시작 대기 시간

            while (ArioManager.instance.isPlay && _remainingSpawnCount > 0)
            {
                ArioManager.instance.ChangeObstacleCnt(_remainingSpawnCount);
                // 비활성화된 장애물 중에서 하나를 활성화
                int obstacleIndex = DeactiveObstacle();
                if (obstacleIndex != -1)
                {
                    _obstaclePool[obstacleIndex].SetActive(true);
                    _remainingSpawnCount--; // 스폰 카운트 감소
                }

                // 남은 스폰 카운트가 0이면 코루틴 종료
                if (_remainingSpawnCount <= 0)
                {
                    yield return new WaitForSeconds(1.0f);

                    string nextStage = ArioManager.instance.CalculateNextStage(ArioManager.instance._currentStage);
                    ArioManager.instance.NextStage(nextStage); // 다음 스테이지 이동
                    yield break;
                }

                // 다음 장애물을 생성하기 전에 대기 (스폰 딜레이 적용)
                yield return new WaitForSeconds(_spawnDelay);
            }
        }

        private int DeactiveObstacle()
        {
            List<int> num = new List<int>();

            for (int i = 0; i < _obstaclePool.Count; i++)
            {
                if (!_obstaclePool[i].activeSelf) // 비활성화된 장애물만 추가
                    num.Add(i);
            }

            if (num.Count > 0)
            {
                return num[Random.Range(0, num.Count)];
            }

            return -1;
        }

        private GameObject CreateObj(GameObject obj, Transform parent)
        {
            GameObject copy = Instantiate(obj);
            copy.transform.SetParent(parent);
            copy.SetActive(false);
            return copy;
        }

        public void ChangeStage(string newStage, ObstacleSpawnDataSet dataSet)
        {
            SetStageData(newStage, dataSet);
            StopAllCoroutines();
            PlayGame();
        }
    }
}