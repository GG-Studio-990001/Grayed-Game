using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using Runtime.ETC;
using UnityEngine.SceneManagement; // 씬 리로드용

namespace Runtime.CH3.Dancepace
{
    public class DPManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private DPEffectManager effectManager;
        [SerializeField] private DPKeyBinder keyBinder;
        [SerializeField] private DPUIManager uiManager;

        [Header("Characters")]
        [SerializeField] private PreviewNPC[] previewNPCs;
        [SerializeField] private AnswerNPC[] answerNPCs;
        [SerializeField] private DPRapley playerCharacter;

        [Header("UI")]
        [SerializeField] private GameObject rehearsalPanel;
        [SerializeField] private GameObject moreRehearsalPanel;

        [Header("Sound")]
        [SerializeField] private string greatSFX = "Dancepace/CH3_Great";
        [SerializeField] private string previewSFX = "Dancepace/CH3_Preview";

        private List<WaveData> rehearsalWaves;
        private List<WaveData> mainWaves;
        //private int currentWaveIndex;
        private bool isRehearsalMode;
        private bool userWantsMoreRehearsal = false;
        private DancepaceData _gameData;
        private List<BeatData> currentBeats = new List<BeatData>();
        private HashSet<string> completedPoses = new HashSet<string>();
        //private bool canInput = false;
        private float elapsed = 0f;
        private int totalScore = 0;

        private void Awake()
        {
            InitializeGameData();
            uiManager?.InitializeUI();
        }

        private void InitializeGameData()
        {
            // 임시 하드코딩 데이터
            _gameData = new DancepaceData();
            _gameData.gameConfig = new GameConfig
            {
                limitTime = 60,
                waveForCount = 6,
                waveMainBGM = "Dancepace/CH3_SUB_BGM_01",
                lifeCount = 3,
                waveClearCoin = 15,
                greatCoin = 3,
                goodCoin = 2,
                badCoin = 0,
                greatTimingWindow = 0.2f,
                goodTimingWindow = 0.4f
            };

            rehearsalWaves = new List<WaveData>();
            mainWaves = new List<WaveData>();

            // 리허설 웨이브 1개
            var rehearsalWave = new WaveData
            {
                waveId = "Wave_Rehearsal_0",
                beats = new List<BeatData>
                {
                    new BeatData("Up", 1f, 0.5f),
                    new BeatData("Down", 0.5f, 0.5f),
                    new BeatData("Left", 0.5f, 1f),
                    new BeatData("Right", 1f, 0.5f)
                }
            };
            rehearsalWaves.Add(rehearsalWave);

            // 메인 웨이브 2개
            var mainWave1 = new WaveData
            {
                waveId = "Wave_Normal_1",
                beats = new List<BeatData>
                {
                    new BeatData("Up", 1f, 0.5f),
                    new BeatData("Up", 0.5f, 0.5f),
                    new BeatData("Down", 0.5f, 0.5f),
                    new BeatData("Left", 0.5f, 1f)
                }
            };
            var mainWave2 = new WaveData
            {
                waveId = "Wave_Normal_2",
                beats = new List<BeatData>
                {
                    new BeatData("Down", 1f, 1f),
                    new BeatData("Right", 0.5f, 0.25f),
                    new BeatData("Right", 0.5f, 0.25f),
                    new BeatData("Up", 1f, 0.5f)
                }
            };
            mainWaves.Add(mainWave1);
            mainWaves.Add(mainWave2);

            _gameData.waveDataList = new List<WaveData>();
            _gameData.waveDataList.AddRange(rehearsalWaves);
            _gameData.waveDataList.AddRange(mainWaves);
        }

        private void Start()
        {
            StartCoroutine(GameFlow());
        }

        private IEnumerator GameFlow()
        {
            isRehearsalMode = true;
            uiManager?.SetRehearsalMode(true);

            bool isRehearsalContinue = true;
            while (isRehearsalContinue && isRehearsalMode)
            {
                uiManager?.ShowKeyGuide(true);
                foreach (var wave in rehearsalWaves)
                {
                    yield return StartCoroutine(PlayRoutine(wave));
                }

                uiManager?.ShowMoreRehearsalPanel(true);
                yield return new WaitUntil(() => userWantsMoreRehearsal);
                uiManager?.ShowMoreRehearsalPanel(false);
                isRehearsalContinue = userWantsMoreRehearsal;
            }

            uiManager?.ShowRehearsalPanel(false);
            uiManager?.ShowKeyGuide(false);

            isRehearsalMode = false;
            foreach (var wave in mainWaves)
            {
                yield return StartCoroutine(PlayRoutine(wave));
            }

            // 모든 웨이브가 끝나면 점수 결과만 보여줌
            uiManager?.ShowResultPanel(totalScore);
        }

        private IEnumerator PlayRoutine(WaveData wave)
        {
            Managers.Sound.Play(Sound.BGM, _gameData.gameConfig.waveMainBGM);
            
            if (wave == null || wave.beats == null || wave.beats.Count == 0)
            {
                Debug.LogError("Wave data is null or empty");
                yield break;
            }

            float limitTime = _gameData.gameConfig.limitTime;
            int waveCount = 0;

            elapsed = 0f;
            uiManager?.UpdateTimeBar(0f, limitTime);

            // 타임바를 별도의 코루틴으로 지속적으로 업데이트
            bool timeOver = false;
            float wait = 1.5f;
            IEnumerator timeBarUpdater = TimeBarUpdater(limitTime, () => timeOver = true);
            Coroutine timeBarCoroutine = StartCoroutine(timeBarUpdater);

            yield return StartCoroutine(UpdateTimeBarWithWait(3f, limitTime)); // 시작 전 3초 대기(연출용)

            while (waveCount < _gameData.gameConfig.waveForCount && !timeOver)
            {
                // 1. 사용할 비트 개수를 2~4개로 랜덤하게 결정
                int bitCount = UnityEngine.Random.Range(2, 5);
                var randomBeats = wave.beats.OrderBy(x => UnityEngine.Random.value).Take(bitCount).ToList();
                Managers.Sound.Play(Sound.SFX, previewSFX);
                yield return StartCoroutine(WaitWithTimeCheck(wait, limitTime, () => timeOver));
                // 1. 프리뷰 구간
                for (int i = 0; i < randomBeats.Count && !timeOver; i++)
                {
                    var beat = randomBeats[i];
                    string nextPoseId = (i + 1 < randomBeats.Count) ? randomBeats[i + 1].poseId : "";
                    foreach (var npc in previewNPCs)
                        npc?.PlayPreviewPose(beat.poseId);
                    float waitTime = beat.timing + beat.restTime;
                    yield return StartCoroutine(WaitWithTimeCheck(waitTime, limitTime, () => timeOver));
                }

                // 프리뷰 종료 효과음 재생
                Managers.Sound.Play(Sound.SFX, previewSFX);
                playerCharacter?.ShowSpotlight(true);
                uiManager?.UpdateKeyGuide(randomBeats.Count > 0 ? randomBeats[0].poseId : "");
                yield return StartCoroutine(WaitWithTimeCheck(wait, limitTime, () => timeOver));
                // 2. 입력 구간 (자동 진행, 입력 대기 없이 박자에 맞춰 판정)
                for (int i = 0; i < randomBeats.Count && !timeOver; i++)
                {
                    var beat = randomBeats[i];
                    foreach (var npc in answerNPCs)
                        npc?.PlayAnswerPose(beat.poseId);
                    yield return StartCoroutine(JudgeBeatInputCoroutine(beat, beat.timing, limitTime));
                    if (i + 1 < randomBeats.Count && beat.restTime > 0)
                    {
                        var nextBeat = randomBeats[i + 1];
                        uiManager?.UpdateKeyGuide(nextBeat.poseId);
                        yield return StartCoroutine(WaitWithTimeCheck(beat.restTime, limitTime, () => timeOver));
                    }
                    else if (beat.restTime > 0)
                    {
                        // 마지막 beat의 restTime은 안내 없이 대기
                        yield return StartCoroutine(WaitWithTimeCheck(beat.restTime, limitTime, () => timeOver));
                    }
                }
                playerCharacter?.ShowSpotlight(false);
                uiManager?.UpdateKeyGuide("");
                waveCount++;
            }

            // 타임바 코루틴 종료 전, 제한시간이 남아 있으면 Idle 모션 유지
            if (!timeOver && elapsed < limitTime)
            {
                foreach (var npc in previewNPCs) npc?.SetIdle();
                foreach (var npc in answerNPCs) npc?.SetIdle();
                playerCharacter?.ResetState();
                while (elapsed < limitTime)
                    yield return null;
            }

            // // 타임바 코루틴 강제 종료
            // if (timeBarCoroutine != null)
            //     StopCoroutine(timeBarCoroutine);
        }

        // 타임바를 실시간으로 업데이트하는 코루틴
        private IEnumerator TimeBarUpdater(float limitTime, Action onTimeOver)
        {
            while (elapsed < limitTime)
            {
                uiManager?.UpdateTimeBar(elapsed, limitTime);
                yield return null;
            }
            uiManager?.UpdateTimeBar(limitTime, limitTime);
            onTimeOver?.Invoke();
        }

        // 시간 흐름을 체크하며 대기하는 유틸리티 코루틴
        private IEnumerator WaitWithTimeCheck(float waitTime, float limitTime, Func<bool> isTimeOver)
        {
            float timer = 0f;
            while (timer < waitTime && elapsed < limitTime && !isTimeOver())
            {
                float delta = Time.deltaTime;
                timer += delta;
                elapsed += delta;
                yield return null;
            }
        }

        private IEnumerator UpdateTimeBarWithWait(float waitTime, float limitTime)
        {
            float timer = 0f;
            while (timer < waitTime && elapsed < limitTime)
            {
                float delta = Time.deltaTime;
                timer += delta;
                elapsed += delta;
                uiManager?.UpdateTimeBar(elapsed, limitTime);
                yield return null;
            }
        }

        private void JudgeInput(float inputTime, float targetTime, string poseId)
        {
            float diff = Mathf.Abs(inputTime - targetTime);
            if (diff <= _gameData.gameConfig.greatTimingWindow)
            {
                ShowJudgment(JudgmentType.Great, poseId);
                Managers.Sound.Play(Sound.SFX, greatSFX);
                totalScore += _gameData.gameConfig.greatCoin;
            }
            else if (diff <= _gameData.gameConfig.goodTimingWindow)
            {
                ShowJudgment(JudgmentType.Good, poseId);
                Managers.Sound.Play(Sound.SFX, greatSFX);
                totalScore += _gameData.gameConfig.goodCoin;
            }
            else
            {
                ShowJudgment(JudgmentType.Bad, poseId);
                totalScore += _gameData.gameConfig.badCoin;
            }
        }

        private void ShowJudgment(JudgmentType type, string poseId)
        {
            effectManager.SpawnHeartParticles(type);
        }

        // 입력 판정(자동 진행) 코루틴 분리
        private IEnumerator JudgeBeatInputCoroutine(BeatData beat, float beatTime, float limitTime)
        {
            bool inputReceived = false;
            float timer = 0f;

            // 비트 시작 전에 이미 키가 눌려 있으면 Bad 판정
            if (keyBinder.IsPoseKeyHeld(beat.poseId))
            {
                JudgeInput(float.MaxValue, 0f, beat.poseId);
                inputReceived = true;
                // 비트 시간만큼 그냥 대기
                while (timer < beatTime && elapsed < limitTime)
                {
                    timer += Time.deltaTime;
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                yield break;
            }

            // 정상 판정 루프
            while (timer < beatTime && elapsed < limitTime)
            {
                if (!inputReceived && keyBinder.IsPoseKeyPressed(beat.poseId))
                {
                    float inputTime = timer;
                    JudgeInput(inputTime, 0f, beat.poseId);
                    inputReceived = true;
                }
                if (!inputReceived && keyBinder.IsAnyOtherPoseKeyPressed(beat.poseId))
                {
                    JudgeInput(float.MaxValue, 0f, beat.poseId);
                    inputReceived = true;
                }
                timer += Time.deltaTime;
                elapsed += Time.deltaTime;
                yield return null;
            }
            if (!inputReceived)
                JudgeInput(float.MaxValue, 0f, beat.poseId);
        }

        public void EndGame()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        public void ReloadScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
    }
}