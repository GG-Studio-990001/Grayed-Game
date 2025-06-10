using UnityEngine;
using System.Collections;
using Runtime.ETC;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

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

        private DancepaceData _gameData;
        private bool _isGameOver = false;

        public event Action OnGameStart;
        public event Action OnGameEnd;
        public event Action OnWaveStart;
        public event Action OnWaveEnd;
        public event Action OnRehearsalStart;
        public event Action OnRehearsalEnd;

        private async void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                await InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private async Task InitializeGame()
        {
            try
            {
                await LoadGameData();
                InitializeManagers();
                StartCoroutine(GameRoutine());
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in InitializeGame: {e.Message}");
            }
        }

        private async Task LoadGameData()
        {
            try
            {
                // 구글 스프레드시트에서 게임 설정 데이터 로드
                var configData = await CSVReader.ReadFromGoogleSheet("GameConfig!A1:Z1000", true);
                if (configData != null && configData.Count > 0)
                {
                    _gameData = new DancepaceData();
                    var firstRow = configData[0];
                    
                    // 실제 스프레드시트의 열 이름으로 데이터 로드
                    if (firstRow.TryGetValue("limitTime", out object limitTime))
                        _gameData.gameConfig.limitTime = Convert.ToSingle(limitTime);
                    if (firstRow.TryGetValue("waveForCount", out object waveForCount))
                        _gameData.gameConfig.waveForCount = Convert.ToInt32(waveForCount);
                    if (firstRow.TryGetValue("waveMainBGM", out object waveMainBGM))
                        _gameData.gameConfig.waveMainBGM = waveMainBGM.ToString();
                    if (firstRow.TryGetValue("lifeCount", out object lifeCount))
                        _gameData.gameConfig.lifeCount = Convert.ToInt32(lifeCount);
                    if (firstRow.TryGetValue("waveClearCoin", out object waveClearCoin))
                        _gameData.gameConfig.waveClearCoin = Convert.ToInt32(waveClearCoin);
                    if (firstRow.TryGetValue("greatCoin", out object greatCoin))
                        _gameData.gameConfig.greatCoin = Convert.ToInt32(greatCoin);
                    if (firstRow.TryGetValue("goodCoin", out object goodCoin))
                        _gameData.gameConfig.goodCoin = Convert.ToInt32(goodCoin);
                    if (firstRow.TryGetValue("badCoin", out object badCoin))
                        _gameData.gameConfig.badCoin = Convert.ToInt32(badCoin);

                    Debug.Log("Game config loaded successfully");

                    // 웨이브 데이터 로드
                    var waveData = await CSVReader.ReadFromGoogleSheet("WaveData!A1:Z1000", false);
                    if (waveData != null && waveData.Count > 0)
                    {
                        foreach (var row in waveData)
                        {
                            if (!row.ContainsKey("Data") || string.IsNullOrEmpty(row["Data"].ToString()))
                                continue;

                            var wave = new WaveData
                            {
                                waveId = row["Data"].ToString(),
                                duration = 30f, // 기본 30초
                                isRehearsal = row["Data"].ToString().EndsWith("_0"),
                                previewBeats = new List<BeatData>(),
                                playBeats = new List<BeatData>(),
                                restBeats = new List<BeatData>()
                            };

                            // 비트 데이터 파싱
                            for (int i = 0; i < 4; i++) // 최대 4개의 비트
                            {
                                string poseKey = i == 0 ? "poseData" : $"poseData{i + 1}";
                                string beatKey = i == 0 ? "beatData" : $"beatData{i + 1}";
                                string restKey = i == 0 ? "restBeatData" : $"restBeatData{i + 1}";

                                if (row.ContainsKey(poseKey) && row.ContainsKey(beatKey))
                                {
                                    var beat = new BeatData
                                    {
                                        poseId = row[poseKey].ToString(),
                                        timing = Convert.ToSingle(row[beatKey]),
                                        isPreview = true,
                                        isPlay = true,
                                        isRestBeat = false
                                    };
                                    wave.previewBeats.Add(beat);
                                    wave.playBeats.Add(beat);

                                    if (row.ContainsKey(restKey))
                                    {
                                        var restBeat = new BeatData
                                        {
                                            poseId = "rest",
                                            timing = Convert.ToSingle(row[restKey]),
                                            isPreview = false,
                                            isPlay = false,
                                            isRestBeat = true
                                        };
                                        wave.restBeats.Add(restBeat);
                                    }
                                }
                            }

                            _gameData.waveDataList.Add(wave);
                        }
                        Debug.Log($"Loaded {_gameData.waveDataList.Count} waves");
                    }
                    else
                    {
                        Debug.LogError("Failed to load wave data: No data found");
                    }
                }
                else
                {
                    Debug.LogError("Failed to load game config: No data found");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game data: {e.Message}");
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

        private IEnumerator GameRoutine()
        {
            if (_gameData == null || _gameData.waveDataList == null || _gameData.waveDataList.Count == 0)
            {
                Debug.LogError("Game data is not properly initialized");
                yield break;
            }

            _gameData.ResetCurrentData();
            _isGameOver = false;

            OnGameStart?.Invoke();

            // 첫 플레이시 리허설 실행
            if (_gameData.IsRehearsalMode)
            {
                yield return StartCoroutine(RehearsalRoutine());
            }

            // 메인 게임 시작
            while (!_isGameOver && !_gameData.IsWaveComplete())
            {
                var currentWave = _gameData.waveDataList[_gameData.CurrentWave];
                if (currentWave == null)
                {
                    Debug.LogError($"Wave data at index {_gameData.CurrentWave} is null");
                    yield break;
                }

                OnWaveStart?.Invoke();
                PlayBGM(_mainBGM);

                yield return StartCoroutine(PlayWaveRoutine(currentWave));

                OnWaveEnd?.Invoke();
                _gameData.CurrentWave++;
            }

            EndGame(!_gameData.IsGameOver());
        }

        private IEnumerator RehearsalRoutine()
        {
            OnRehearsalStart?.Invoke();
            PlayBGM(_rehearsalBGM);

            // 리허설용 웨이브 찾기
            var rehearsalWave = _gameData.waveDataList.Find(w => w.isRehearsal);
            if (rehearsalWave != null)
            {
                yield return StartCoroutine(PlayWaveRoutine(rehearsalWave));
            }

            OnRehearsalEnd?.Invoke();
            _gameData.IsRehearsalMode = false;
        }

        private IEnumerator PlayWaveRoutine(WaveData wave)
        {
            if (wave == null || wave.playBeats == null)
            {
                Debug.LogError("Invalid wave data for play");
                yield break;
            }

            foreach (var beat in wave.playBeats)
            {
                if (beat == null) continue;
                // 플레이 비트 처리
                yield return new WaitForSeconds(beat.timing);
            }
        }

        private void OnRehearsalComplete()
        {
            _gameData.IsRehearsalMode = false;
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
            _isGameOver = true;
            StopBGM();

            if (isSuccess)
            {
                _gameData.AddCoins(_gameData.gameConfig.waveClearCoin);
                _gameData.CompleteWave(_gameData.CurrentWave);
            }

            _gameData.UpdateHighScore();
            _gameData.UpdateBestAccuracy();
            _gameData.UpdateMaxCombo();

            OnGameEnd?.Invoke();
        }

        private void HandleJudgment(JudgmentType judgment)
        {
            switch (judgment)
            {
                case JudgmentType.Great:
                    _gameData.AddCoins(_gameData.gameConfig.greatCoin);
                    _gameData.AddCombo();
                    break;
                case JudgmentType.Good:
                    _gameData.AddCoins(_gameData.gameConfig.goodCoin);
                    _gameData.AddCombo();
                    break;
                case JudgmentType.Bad:
                    _gameData.LoseLife();
                    _gameData.ResetCombo();
                    break;
            }
        }

        public DancepaceData GetGameData()
        {
            return _gameData;
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