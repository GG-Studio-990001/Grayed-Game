using Cinemachine;
using Runtime.InGameSystem;
using Runtime.Main.Runtime.ETC;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public Action<bool> OnEnterReward;
        public Action OpenStore;
        
        [SerializeField] private ObstacleSpawnDataSet _dataSet;
        [SerializeField] private CinemachineVirtualCamera _storeCam;
        [SerializeField] private CinemachineVirtualCamera _rewardCam;
        [SerializeField] private SceneSystem _sceneSystem;
        [SerializeField] private Ario _ario;
        [SerializeField] private ArioUIController _ui;

        public string CurrentStage { get; private set; }
        public float GameSpeed { get; private set; }
        public bool IsPlay { get; private set; }
        public bool IsPause { get; private set; }
        public bool IsStore { get; private set; }
        public bool IsReward { get; private set; }
        public bool HasItem { get; private set; }
        public int CoinCnt { get; private set; }
        private ObstacleManager _obstacleManager;
        private DataCheater _dataCheater = new();

        private void Start()
        {
            _obstacleManager = GetComponent<ObstacleManager>();
            StartCoroutine(WaitStart());
            CurrentStage = _dataSet.DataList[0].Stage;
        }

        public void RestartSuperArio()
        {
            if (IsPlay || IsStore)
                return;
            _storeCam.Priority = 10;
            StartGame();
        }

        public void EnterStore()
        {
            _storeCam.Priority = 12;
            _ui.ActiveRestartText(false);
            ChangeHeartUI(1);

            IsStore = true;
            OnEnterStore.Invoke(IsStore);
        }

        public void ExitStore()
        {
            StartCoroutine(WaitExitStore());
        }

        private IEnumerator WaitExitStore()
        {
            yield return new WaitForSeconds(1f);
            IsStore = false;
            RestartSuperArio();
        }

        public void EnterReward()
        {
            _storeCam.Priority = 10;
            _rewardCam.Priority = 12;
            
            IsStore = false;
            IsPlay = false;
            
            OnPlay.Invoke(IsPlay);
            _obstacleManager.DeleteBuilding();
            
            IsReward = true;
            OnEnterReward.Invoke(IsReward);
        }
        
        public void ExitReward()
        {
            StartCoroutine(WaitExitReward());
        }

        private IEnumerator WaitExitReward()
        {
            yield return new WaitForSeconds(1f);
            if(CurrentStage.StartsWith("3"))
                _dataCheater.LoadCheatData("Turn3", _sceneSystem);
            else
            {
                _rewardCam.Priority = 10;
                IsReward = false;
                RestartSuperArio();
            }
            Debug.Log("리워드 종료");
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
            if (_ario.life >= 3)
                return false;
            return true;
        }

        public void PlusLife()
        {
            ChangeHeartUI(_ario.life + 1);
        }

        private void InitData()
        {
            _ui.ChangeCoinText("RAPLEY\n" + CoinCnt);
            _ui.ActiveRestartText(false);
            _ui.ChangeObstacleText(0);
            if (_ario.life <= 1)
                ChangeHeartUI(1);
            IsPause = false; // 시작 시 입력 방지
            //GetItem(); // 테스트 용 아이템 지급
        }

        private void GameOver()
        {
            _ui.ActiveRestartText(true);
            _ui.ChangeObstacleText(0);
            UpdateStage(CurrentStage);
            IsPlay = false;
            OnPlay.Invoke(IsPlay);
        }

        public void ChangeHeartUI(int life)
        {
            _ui.ChangeHeartUI(life);
            if (life == 0)
                GameOver();
            _ario.life = life;
        }

        public void GetCoin()
        {
            CoinCnt++;
            _ui.ChangeCoinText("RAPLEY\n" + CoinCnt);
        }
        
        public void GetItem()
        {
            HasItem = true;
            _ui.GetItemSprite();
        }
        
        public void ChangeItemSprite()
        {
            HasItem = false;
            _ui.UseItemSprite();
        }
        
        public void ChangeObstacleCnt(int count)
        {
            _ui.ChangeObstacleText(count);
        }
        

        public void TouchFlag()
        {
            IsPlay = false;
            IsStore = false;
            IsReward = true;
        }

        private void SpawnBuilding()
        {
            _obstacleManager.SpawnBuilding();
        }

        //TODO: 스테이지가 끝나면 토관 or 빌딩 소환 들어가기 전에 현재 스테이지 저장시키기
        public void CalculateNextStage()
        {
            var parts = CurrentStage.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0], out int world) && int.TryParse(parts[1], out int stage))
            {
                stage++;
                if (stage > 3) // 3 스테이지를 넘어가면 다음 월드로 이동
                {
                    SpawnBuilding();
                    world++;
                    stage = 1;
                    CurrentStage = $"{world}-{stage}";
                    Debug.Log("스테이지 3 끝");
                    return;
                }

                CurrentStage = $"{world}-{stage}";
                GameOver();
            }

            //UpdateStage(CurrentStage); // 기본적으로 변경 없이 반환
        }
        
        private void UpdateStage(string stage)
        {
            var stageData = _dataSet.DataList.Find(data => data.Stage == stage);
            if (stageData == null)
            {
                return;
            }

            CurrentStage = stage; // 현재 스테이지 갱신
            GameSpeed = stageData.Speed + 4; // 게임 속도 갱신
            _ui.ChangeStageText($"WORLD\n{CurrentStage}"); // UI에 스테이지 정보 갱신
            ChangeObstacleCnt(stageData.ObstacleCount);

            // 장애물 매니저에 스테이지 데이터 전달
            _obstacleManager.ChangeStage(CurrentStage, _dataSet);
        }

        public void StoreOpenEvent()
        {
            OpenStore.Invoke();
        }
    }
}