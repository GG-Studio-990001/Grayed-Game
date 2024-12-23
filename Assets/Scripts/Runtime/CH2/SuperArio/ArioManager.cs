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
        public Action<bool> OnExitStore;
        public Action OpenStore;


        [SerializeField] private ObstacleSpawnDataSet _dataSet;
        [SerializeField] private CinemachineVirtualCamera _storeCam;
        [SerializeField] private Ario _ario;
        [SerializeField] private ArioUIController _ui;

        public float GameSpeed { get; private set; }
        public bool IsPlay { get; private set; }
        public bool IsPause { get; private set; }
        public bool HasItem { get; private set; }
        public string CurrentStage { get; private set; }

        public int CoinCnt { get; private set; }
        public bool IsStore { get; private set; }
        private ObstacleManager _obstacleManager;

        private void Start()
        {
            _obstacleManager = GetComponent<ObstacleManager>();
            StartCoroutine(WaitStart());
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
            IsStore = false;
            RestartSuperArio();
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
            //HasItem = true; // 테스트 용 아이템 지급
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

            // 장애물 매니저에 스테이지 데이터 전달
            _obstacleManager.ChangeStage(CurrentStage, _dataSet);
        }

        public void NextStage(string nextStage)
        {
            // 특정 스테이지 조건 체크
            if (nextStage.EndsWith("-3"))
            {
                SpawnBuilding();
            }
            else
            {
                UpdateStage(nextStage); // 일반 스테이지 업데이트
            }
        }

        public void TouchFlag()
        {
            IsPlay = false;
        }

        private void SpawnBuilding()
        {
            Debug.Log("예아");
            
            // 1. 장애물 대신 깃발, 건물이 다가오게 하기
            _obstacleManager.CreateBuilding();
            // 2. 깃발 닿으면 배경, 바닥 움직이는 것 멈춤 -> isPlay false, 연출 시작
            //
            // 3. 깃발 닿은 위치 계산 내려오고 건물로 자동 이동
            //
            // 5. 다음 스테이지
            //IsPlay = false;
            
            //OnPlay.Invoke(IsPlay);
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

        public void StoreOpenEvent()
        {
            OpenStore.Invoke();
        }
    }
}