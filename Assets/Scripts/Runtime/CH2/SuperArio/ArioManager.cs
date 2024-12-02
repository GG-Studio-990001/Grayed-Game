using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class ArioManager : MonoBehaviour
    {
        #region Instance

        public static ArioManager instance;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
        }
        #endregion

        public Action<bool> OnPlay;
        public Action<bool> OnEnterStore;

        [SerializeField] private ArioUIController ui; // UI 컨트롤러
        [SerializeField] private ObstacleManager obstacleManager; // 장애물 매니저
        [SerializeField] private ObstacleSpawnDataSet dataSet; // ScriptableObject 데이터셋
        [SerializeField] private CinemachineVirtualCamera stageCamera;
        [SerializeField] private CinemachineVirtualCamera storeCamera;
        
        
        public float gameSpeed = 1; // 현재 게임 속도
        public bool isPlay;
        public bool isPause = true;

        private int _coinCnt;
        public string _currentStage = "1-1"; // 초기 스테이지 설정
        public bool GetItem { get; private set; }


        private void Start()
        {
            StartCoroutine(WaitStart());
        }

        public void RestartSuperArio()
        {
            storeCamera.Priority = 10;
            if (!isPlay)
                StartGame();
        }

        public void EnterStore()
        {
            // 카메라 변경
            storeCamera.Priority = 12;
            OnEnterStore.Invoke(true);
            // 입장 연출
        }

        private IEnumerator WaitStart()
        {
            yield return new WaitForSeconds(0.5f);
            StartGame();
        }

        private void StartGame()
        {
            ui.ChangeCoinText("RAPLEY\n" + _coinCnt);
            ui.ActiveRestartText(false);
            ui.ChangeObstacleText(0);
            ChangeHeartUI(1);
            isPlay = true;
            isPause = false;
            GetItem = true;
            OnPlay.Invoke(isPlay);
            UpdateStage(_currentStage);
        }

        private void GameOver()
        {
            ui.ChangeObstacleText(0);
            UpdateStage(_currentStage);
            ui.ActiveRestartText(true);
            isPlay = false;
            OnPlay.Invoke(isPlay);
        }

        public void ChangeHeartUI(int life)
        {
            ui.ChangeHeartUI(life);
            if (life == 0)
                GameOver();
        }

        public void GetCoin()
        {
            _coinCnt++;
            ui.ChangeCoinText("RAPLEY\n" + _coinCnt);
        }

        public void ChangeItemSprite()
        {
            GetItem = false;
            ui.ChangeItemSprite();
        }

        public void GetItemSprite()
        {
            GetItem = true;
            ui.GetItemSprite();
        }

        public void ChangeObstacleCnt(int count)
        {
            ui.ChangeObstacleText(count);
        }

        // 스테이지 업데이트
        public void UpdateStage(string stage)
        {
            var stageData = dataSet.DataList.Find(data => data.Stage == stage);
            if (stageData == null)
            {
                return;
            }

            _currentStage = stage; // 현재 스테이지 갱신
            gameSpeed = stageData.Speed + 4; // 게임 속도 갱신
            ui.ChangeStageText($"WORLD\n{_currentStage}"); // UI에 스테이지 정보 갱신

            // 장애물 매니저에 스테이지 데이터 전달
            obstacleManager.ChangeStage(_currentStage, dataSet);
        }

        // 스테이지 변경 후 게임 재시작
        public void NextStage(string nextStage)
        {
            // 특정 스테이지 조건 체크
            if (nextStage.EndsWith("-3"))
            {
                EnterRewardRoom(); // 보상 방으로 이동
            }
            else
            {
                UpdateStage(nextStage); // 일반 스테이지 업데이트
                isPlay = true;
                OnPlay.Invoke(isPlay);
            }
        }
        
        private void EnterRewardRoom()
        {
            // 보상 방으로 이동하는 로직
            isPlay = false;
            OnPlay.Invoke(isPlay); // 게임 상태 정지
        }
        
        public string CalculateNextStage(string currentStage)
        {
            var parts = currentStage.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0], out int world) && int.TryParse(parts[1], out int stage))
            {
                stage++;
                if (stage > 3) // 3 스테이지를 넘어가면 다음 월드로 이동
                {
                    world++;
                    stage = 1;
                }
                return $"{world}-{stage}";
            }
            
            return currentStage; // 기본적으로 변경 없이 반환
        }
    }
}
