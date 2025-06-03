using UnityEngine;
using System.Collections;
using Runtime.Common;
using Runtime.Common.View;
using Runtime.ETC;
using System.Collections.Generic;
using System;

namespace Runtime.CH3.Dancepace
{
    public class DancepaceManager : MonoBehaviour
    {
        public static DancepaceManager Instance { get; private set; }

        [Header("=Managers=")]
        [SerializeField] private RhythmManager _rhythmManager;
        [SerializeField] private RehearsalManager _rehearsalManager;
        [SerializeField] private DancepaceKeyBinder _keyBinder;

        [Header("=Audio=")]
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioClip _mainBGM;
        [SerializeField] private AudioClip _rehearsalBGM;

        private const string GAME_CONFIG_PATH = "DancepaceData";
        private const string WAVE_DATA_PATH = "DancepaceWaveData";

        private GameConfig _gameConfig;
        private List<WaveData> _waveDataList;
        private int _currentWaveIndex = 0;
        private float _gameTimer = 0f;
        private bool _isGameRunning = false;

        public event Action OnGameStart;
        public event Action OnGameEnd;
        public event Action OnWaveStart;
        public event Action OnWaveEnd;

        private int totalCoins = 0;
        private int remainingLives;
        private bool isFirstPlay = true;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            InitializeManagers();
            LoadGameData();
        }

        private void LoadGameData()
        {
            // 게임 설정 로드
            var configData = CSVReader.ReadAndConvert<GameConfig>(GAME_CONFIG_PATH);
            if (configData != null && configData.Length > 0)
            {
                _gameConfig = configData[0];
            }
            else
            {
                Debug.LogError($"Failed to load game config from {GAME_CONFIG_PATH}");
                return;
            }

            // 웨이브 데이터 로드
            var waveData = CSVReader.ReadAndConvert<WaveData>(WAVE_DATA_PATH);
            if (waveData != null)
            {
                _waveDataList = new List<WaveData>(waveData);
            }
            else
            {
                Debug.LogError($"Failed to load wave data from {WAVE_DATA_PATH}");
                _waveDataList = new List<WaveData>();
            }
        }

        private void InitializeManagers()
        {
            _rhythmManager = GetComponent<RhythmManager>();
            _rehearsalManager = GetComponent<RehearsalManager>();
            _keyBinder = transform.GetChild(0).GetComponent<DancepaceKeyBinder>();
            
            // 리듬 매니저 이벤트 구독
            RhythmManager.Instance.OnJudgment += HandleJudgment;
            
            // 리허설 매니저 이벤트 구독
            RehearsalManager.Instance.OnRehearsalComplete += OnRehearsalComplete;
            RehearsalManager.Instance.OnRehearsalSkip += OnRehearsalComplete;
        }

        private void Start()
        {
            if (_gameConfig == null)
            {
                Debug.LogError("Failed to load game config");
                return;
            }

            _gameTimer = _gameConfig.limitTime;
            StartCoroutine(GameRoutine());
        }

        private IEnumerator GameRoutine()
        {
            // 리허설 시작
            _rehearsalManager.StartRehearsal();
            yield return new WaitUntil(() => !_rehearsalManager.IsRehearsalActive);

            // 메인 게임 시작
            _isGameRunning = true;
            PlayBGM(_mainBGM);

            while (_isGameRunning && _gameTimer > 0)
            {
                _gameTimer -= Time.deltaTime;
                
                // 웨이브 진행
                if (_currentWaveIndex < _gameConfig.waveForCount)
                {
                    var waveData = _waveDataList.Find(wave => wave.waveId == $"Wave_Normal_{_currentWaveIndex + 1}");
                    if (waveData != null)
                    {
                        _rhythmManager.StartWave(waveData);
                        yield return new WaitUntil(() => !_rhythmManager.IsWaveActive);
                        _currentWaveIndex++;
                    }
                }
                else
                {
                    _isGameRunning = false;
                }

                yield return null;
            }

            // 게임 종료
            StopBGM();
            _isGameRunning = false;
        }

        private void OnRehearsalComplete()
        {
            StartCoroutine(GameRoutine());
        }

        private void PlayBGM(AudioClip bgm)
        {
            if (_bgmSource != null && bgm != null)
            {
                _bgmSource.clip = bgm;
                _bgmSource.Play();
            }
        }

        private void StopBGM()
        {
            if (_bgmSource != null)
            {
                _bgmSource.Stop();
            }
        }

        public void PlaySFX(AudioClip sfx)
        {
            if (_sfxSource != null && sfx != null)
            {
                _sfxSource.PlayOneShot(sfx);
            }
        }

        private void EndGame(bool isSuccess)
        {
            _isGameRunning = false;
            isFirstPlay = false;

            if (isSuccess)
            {
                AddCoins(_gameConfig.waveClearCoin);
            }

            OnGameEnd?.Invoke();
        }

        private void HandleJudgment(JudgmentType judgment)
        {
            switch (judgment)
            {
                case JudgmentType.Great:
                    AddCoins(_gameConfig.greatCoin);
                    break;
                case JudgmentType.Good:
                    AddCoins(_gameConfig.goodCoin);
                    break;
                case JudgmentType.Bad:
                    remainingLives--;
                    break;
            }
        }

        public void AddCoins(int amount)
        {
            totalCoins += amount;
            // TODO: UI 업데이트 구현
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                // 이벤트 구독 해제
                if (RhythmManager.Instance != null)
                {
                    RhythmManager.Instance.OnJudgment -= HandleJudgment;
                }
                
                if (RehearsalManager.Instance != null)
                {
                    RehearsalManager.Instance.OnRehearsalComplete -= OnRehearsalComplete;
                    RehearsalManager.Instance.OnRehearsalSkip -= OnRehearsalComplete;
                }
            }
        }
    }
} 