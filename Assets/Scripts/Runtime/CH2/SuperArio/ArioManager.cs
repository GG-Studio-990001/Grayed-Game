using Cinemachine;
using Runtime.CH1.Pacmom;
using Runtime.ETC;
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

        public static ArioManager Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            GameSpeed = 1;
            IsPause = true;
            CurrentStage = "1-1";
        }

        #endregion

        public Action<bool> OnPlay;
        public Action<bool> OnEnterStore;
        public Action OnEnterReward;
        public Action OpenStore;

        [SerializeField] private ObstacleSpawnDataSet _dataSet;
        [SerializeField] private CinemachineVirtualCamera _storeCam;
        [SerializeField] private CinemachineVirtualCamera _rewardCam;
        [SerializeField] private SceneSystem _sceneSystem;
        [SerializeField] private Ario _ario;
        [SerializeField] private Mario _mario;
        [SerializeField] private ArioUIController _ui;
        [SerializeField] private string stageName;

        [Header("디버그")]
        [SerializeField] private bool skipOpening = false;
        public bool SkipOpening => skipOpening;

        public string CurrentStage { get; private set; }
        public float GameSpeed { get; private set; }
        public bool IsPlay { get; private set; }
        public bool IsPause { get; private set; }
        public bool IsStore { get; private set; }
        public bool IsReward { get; private set; }
        public bool HasItem { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsOpening { get; private set; }
        public int CoinCnt { get; private set; }
        private ObstacleManager _obstacleManager;
        private CameraProduction _production;
        private SceneTransform _sceneTransform;
        private DataCheater _dataCheater;

        private void Start()
        {
            _obstacleManager = GetComponent<ObstacleManager>();
            _production = GetComponent<CameraProduction>();
            CurrentStage = Managers.Data.CH2.ArioStage;
            CoinCnt = Managers.Data.Common.Coin;

            InitData();

            if (skipOpening)
            {
                WaitStart();
                return;
            }
            _production.SetAspectRatio(AspectRatio.Ratio_8_7, true);
        }

        public void RestartSuperArio()
        {
            if (IsPlay || IsStore || IsOpening)
                return;
            _storeCam.Priority = 10;
            _ui.ActiveRestartText(false);
            StartGame(true);
        }

        public void EnterStoreAnimation()
        {
            _ui.ActiveRestartText(false);
            _ario.EnterStoreAnimation();
        }

        public void EnterStore()
        {
            _production.SetAspectRatio(AspectRatio.Ratio_8_7, true);
            _storeCam.Priority = 12;
            _ui.ActiveRestartText(false);
            ChangeHeartUI(1);
            _obstacleManager.DestroyPipe();

            IsPlay = false;
            IsStore = true;
            OnEnterStore.Invoke(IsStore);
            Managers.Sound.Play(Sound.BGM, "SuperArio/CH2_SUB_BGM_02_30s");
        }

        public void ExitStore()
        {
            StartCoroutine(WaitExitStore());
        }

        private IEnumerator WaitExitStore()
        {
            yield return new WaitForSeconds(1f);
            _production.SetAspectRatio(AspectRatio.Ratio_21_9, true);
            IsStore = false;
            _storeCam.Priority = 10;
            StartGame(false);
        }

        public void EnterReward()
        {
            StartCoroutine(WaitEnterReward(_production.FadeInOut()));
        }

        private IEnumerator WaitEnterReward(float delay = 0)
        {
            yield return new WaitForSeconds(delay - 1f);
            Managers.Sound.Play(Sound.BGM, "SuperArio/CH2_SUB_BGM_03");
            _production.SetAspectRatio(AspectRatio.Ratio_8_7, true);
            _obstacleManager.DestroyBuilding();
            _storeCam.Priority = 10;
            _rewardCam.Priority = 12;

            IsStore = false;
            IsPlay = false;

            OnPlay.Invoke(IsPlay);

            IsReward = true;
            OnEnterReward.Invoke();
        }

        public void ExitReward()
        {
            StartCoroutine(WaitExitReward());
        }

        public void StopGame()
        {
            IsPlay = false;
            OnPlay.Invoke(IsPlay);
        }

        private IEnumerator WaitExitReward()
        {
            yield return new WaitForSeconds(1f);
            _ui.gameObject.SetActive(false);
            Managers.Data.CH2.ArioStage = CurrentStage;

            var parts = CurrentStage[0];
            switch (parts)
            {
                case '2':
                    Managers.Data.CH2.Turn = 3;
                    break;
                case '3':
                    Managers.Data.CH2.Turn = 5;
                    break;
                case '4':
                    Managers.Data.CH2.Turn = 7;
                    break;
            }
            Debug.Log($"{Managers.Data.CH2.Turn}턴 시작");
            _sceneTransform = FindObjectOfType<SceneTransform>();
            _sceneTransform.EscapeFromScene("CH2");

        }

        public void WaitStart()
        {
            _production.SetAspectRatio(AspectRatio.Ratio_21_9);
            IsOpening = false;
            StartGame();
        }

        public void PauseKey()
        {
            IsPause = !IsPause;
        }

        private void StartGame(bool initLife = true)
        {
            Managers.Sound.Play(Sound.BGM, "SuperArio/CH2_SUB_BGM_01");

            // 모든 스테이지 시작 시 저장된 코인 데이터 불러오기
            CoinCnt = Managers.Data.Common.Coin;

            UpdateStage(CurrentStage);

            IsGameOver = false;
            IsPlay = true;
            if (initLife)
                ChangeHeartUI(1); // 게임 시작 시에만 체력 초기화
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
            if (_ario.life == 0)
                ChangeHeartUI(1);
            IsOpening = true;
            IsPause = false; // 시작 시 입력 방지
        }

        private void GameOver()
        {
            IsGameOver = true;
            _ui.ActiveRestartText(true);
            _ui.ChangeObstacleText(0);
            UpdateStage(CurrentStage);
            IsPlay = false;
            OnPlay.Invoke(IsPlay);

            // 게임 종료 시 코인 데이터 저장
            Managers.Data.Common.Coin = CoinCnt;
        }

        private void NextStage()
        {
            UpdateStage(CurrentStage);
            _obstacleManager.SpawnPipe();
        }

        public void ChangeHeartUI(int life)
        {
            _ui.ChangeHeartUI(life);
            if (life == 0)
                GameOver();
            _ario.life = life;
        }

        public void GetCoin(int count = 0)
        {
            if (count == 0)
                CoinCnt++;
            else
                CoinCnt += count;

            _ui.ChangeCoinText("RAPLEY\n" + CoinCnt);
        }

        public bool UseCoin(int count)
        {
            if (CoinCnt < count)
                return false;
            CoinCnt -= count;
            _ui.ChangeCoinText("RAPLEY\n" + CoinCnt);
            return true;
        }

        public void GetItem()
        {
            HasItem = true;
            _ui.GetItemSprite();
        }

        public void UseItem()
        {
            Managers.Sound.Play(Sound.BGM, "SuperArio/CH2_SUB_BGM_04");
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
            _production.SetAspectRatio(AspectRatio.Ratio_8_7);
            _ui.gameObject.SetActive(false);
            _mario.PauseAnimation();
        }

        public void CalculateNextStage()
        {
            var parts = CurrentStage.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0], out int world) && int.TryParse(parts[1], out int stage))
            {
                stage++;
                if (stage > 3) // 3 스테이지를 넘어가면 다음 월드로 이동
                {
                    _obstacleManager.SpawnBuilding();
                    world++;
                    stage = 1;
                    CurrentStage = $"{world}-{stage}";
                    Debug.Log("스테이지 3 끝");
                    return;
                }

                CurrentStage = $"{world}-{stage}";
                NextStage();
            }
        }

        private void UpdateStage(string stage)
        {
            var stageData = _dataSet.DataList.Find(data => data.Stage == stage);
            if (stageData == null)
            {
                return;
            }

            CurrentStage = stage; // 현재 스테이지 갱신
            GameSpeed = stageData.Speed + 5; // 게임 속도 갱신
            _ui.ChangeStageText($"STAGE\n{CurrentStage}"); // UI에 스테이지 정보 갱신
            ChangeObstacleCnt(stageData.ObstacleCount);

            // 장애물 매니저에 스테이지 데이터 전달
            _obstacleManager.ChangeStage(CurrentStage, _dataSet);
        }

        public void StoreOpenEvent()
        {
            OpenStore.Invoke();
        }

        public void AddCheatCoins()
        {
            if (!IsPlay) return;
            if (CoinCnt >= 500) return;

            for (int i = 0; i < 500; i++)
            {
                GetCoin();
            }
            
            Managers.Sound.Play(Sound.SFX, "SuperArio/CH2_SUB_SFX_31");
        }
    }
}