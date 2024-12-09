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
            GameSpeed = 1;
            IsPause = true;
            CurrentStage = "1-1";
        }
        #endregion

        public Action<bool> OnPlay;
        public Action<bool> OnEnterStore;

        [SerializeField] private ArioUIController ui;
        [SerializeField] private ObstacleManager obstacleManager;
        [SerializeField] private ObstacleSpawnDataSet dataSet;
        [SerializeField] private CinemachineVirtualCamera storeCamera;
        [SerializeField] private Ario _ario;
        
        public float GameSpeed { get; private set; }
        public bool IsPlay {  get; private set; }
        public bool IsPause { get; private set; }
        public bool HasItem { get; private set; }
        public string CurrentStage { get; private set; }
        
        private int _coinCnt;
        private bool _isStore;
        
        private void Start()
        {
            StartCoroutine(WaitStart());
        }

        public void RestartSuperArio()
        {
            if (IsPlay || _isStore)
                return;
            
            storeCamera.Priority = 10;
            StartGame();
        }

        public void EnterStore()
        {
            // 카메라 변경
            storeCamera.Priority = 12;
            ui.ActiveRestartText(false);
            ChangeHeartUI(1);

            _isStore = true;
            OnEnterStore.Invoke(_isStore);
        }

        private IEnumerator WaitStart()
        {
            yield return new WaitForSeconds(0.5f);
            StartGame();
        }

        public void PauseKey()
        {
            IsPause = !IsPause;
        }

        private void StartGame()
        {
            InitData();
            UpdateStage(CurrentStage);
            IsPlay = true;
            OnPlay.Invoke(IsPlay);
        }

        public bool LifeCheck()
        {
            if (_ario._life >= 3)
                return false;
            return true;
        }

        public void PlusLife()
        {
            ChangeHeartUI(_ario._life+1);
        }

        public bool ItemCheck()
        {
            if (HasItem)
                return false;
            return true;
        }
        
        private void InitData()
        {
            ui.ChangeCoinText("RAPLEY\n" + _coinCnt);
            ui.ActiveRestartText(false);
            ui.ChangeObstacleText(0);
            if(_ario._life <= 1)
                ChangeHeartUI(1);
            IsPause = false; // 시작 시 입력 방지
            HasItem = true; // 테스트 용 아이템 지급
        }

        private void GameOver()
        {
            ui.ChangeObstacleText(0);
            UpdateStage(CurrentStage);
            ui.ActiveRestartText(true);
            IsPlay = false;
            OnPlay.Invoke(IsPlay);
        }

        public void ChangeHeartUI(int life)
        {
            ui.ChangeHeartUI(life);
            if (life == 0)
                GameOver();
            _ario._life = life;
        }

        public void GetCoin()
        {
            _coinCnt++;
            ui.ChangeCoinText("RAPLEY\n" + _coinCnt);
        }

        public void ChangeItemSprite()
        {
            HasItem = false;
            ui.UseItemSprite();
        }

        public void GetItem()
        {
            HasItem = true;
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
            
            CurrentStage = stage; // 현재 스테이지 갱신
            GameSpeed = stageData.Speed + 4; // 게임 속도 갱신
            ui.ChangeStageText($"WORLD\n{CurrentStage}"); // UI에 스테이지 정보 갱신

            // 장애물 매니저에 스테이지 데이터 전달
            obstacleManager.ChangeStage(CurrentStage, dataSet);
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
                IsPlay = true;
                OnPlay.Invoke(IsPlay);
            }
        }
        
        private void EnterRewardRoom()
        {
            // 보상 방으로 이동하는 로직
            IsPlay = false;
            OnPlay.Invoke(IsPlay);
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
