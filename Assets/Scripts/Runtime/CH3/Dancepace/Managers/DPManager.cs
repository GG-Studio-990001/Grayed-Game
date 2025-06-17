using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using Runtime.ETC;

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
        //[SerializeField] private string goodSFX = "Dancepace/CH3_Good";
        [SerializeField] private string previewSFX = "Dancepace/CH3_Preview";

        private List<WaveData> rehearsalWaves;
        private List<WaveData> mainWaves;
        //private int currentWaveIndex;
        private bool isRehearsalMode = true;
        private bool userWantsMoreRehearsal = false;
        private DancepaceData _gameData;
        private List<BeatData> currentBeats = new List<BeatData>();
        private HashSet<string> completedPoses = new HashSet<string>();
        //private bool canInput = false;
        private float elapsed = 0f;

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
            uiManager?.ShowRehearsalPanel(true);
            uiManager?.SetRehearsalMode(true);
            Managers.Sound.Play(Sound.BGM, _gameData.gameConfig.waveMainBGM);

            do
            {
                uiManager?.ShowKeyGuide(true);
                foreach (var wave in rehearsalWaves)
                {
                    yield return StartCoroutine(PlayRoutine(wave));
                }

                uiManager?.ShowMoreRehearsalPanel(true);
                userWantsMoreRehearsal = false;
                yield return new WaitUntil(() => userWantsMoreRehearsal);
                uiManager?.ShowMoreRehearsalPanel(false);
            }
            while (userWantsMoreRehearsal);

            uiManager?.ShowRehearsalPanel(false);
            uiManager?.SetRehearsalMode(false);
            uiManager?.ShowKeyGuide(false);

            isRehearsalMode = false;
            foreach (var wave in mainWaves)
            {
                yield return StartCoroutine(PlayRoutine(wave));
            }
        }

        private IEnumerator PlayRoutine(WaveData wave)
        {
            if (wave == null || wave.beats == null || wave.beats.Count == 0)
            {
                yield break;
            }

            float limitTime = _gameData.gameConfig.limitTime;
            int waveCount = 0;
            bool isGameOver = false;

            elapsed = 0f;
            uiManager?.UpdateTimeBar(0f, limitTime);

            yield return StartCoroutine(UpdateTimeBarWithWait(1f, limitTime));

            while (!isGameOver && waveCount < _gameData.gameConfig.waveForCount)
            {
                for (int i = 0; i < wave.beats.Count; i++)
                {
                    var beat = wave.beats[i];
                    string nextPoseId = (i + 1 < wave.beats.Count) ? wave.beats[i + 1].poseId : "";
                    uiManager?.UpdateKeyGuide(beat.poseId, nextPoseId); // 각 포즈마다 반드시 호출
                    foreach (var npc in previewNPCs)
                        npc?.PlayPreviewPose(beat.poseId);
                    float waitTime = beat.timing + beat.restTime;
                    yield return StartCoroutine(UpdateTimeBarWithWait(waitTime, limitTime));
                    if (elapsed >= limitTime) { isGameOver = true; break; }
                }

                Managers.Sound.Play(Sound.SFX, previewSFX);
                yield return StartCoroutine(UpdateTimeBarWithWait(0.5f, limitTime));
                if (elapsed >= limitTime) { isGameOver = true; break; }

                for (int i = 0; i < wave.beats.Count; i++)
                {
                    var beat = wave.beats[i];
                    string nextPoseId = (i + 1 < wave.beats.Count) ? wave.beats[i + 1].poseId : "";
                    uiManager?.UpdateKeyGuide(beat.poseId, nextPoseId); // 각 포즈마다 반드시 호출
                    foreach (var npc in answerNPCs)
                        npc?.PlayAnswerPose(beat.poseId);
                    yield return StartCoroutine(WaitForPlayerInputs(new List<BeatData> { beat }));
                    yield return StartCoroutine(UpdateTimeBarWithWait(beat.timing, limitTime));
                    if (beat.restTime > 0)
                        yield return StartCoroutine(UpdateTimeBarWithWait(beat.restTime, limitTime));
                    if (elapsed >= limitTime) { isGameOver = true; break; }
                }
                waveCount++;
            }

            if (isGameOver)
            {
                OnGameSuccess();
            }
            else
            {
                OnGameFail();
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

        private IEnumerator WaitForPlayerInputs(List<BeatData> beats)
        {
            bool allInputsReceived = false;
            float startTime = Time.time;
            float maxWait = beats[0].timing * 2f;

            while (!allInputsReceived && Time.time - startTime < maxWait)
            {
                if (keyBinder != null)
                {
                    foreach (var beat in beats)
                    {
                        if (!completedPoses.Contains(beat.poseId) && keyBinder.IsPoseKeyPressed(beat.poseId))
                        {
                            completedPoses.Add(beat.poseId);
                            float timing = Time.time - startTime;
                            JudgeInput(timing, beat.timing, beat.poseId);
                        }
                    }
                }

                allInputsReceived = completedPoses.Count == beats.Count;
                yield return null;
            }

            if (!allInputsReceived)
            {
                ShowJudgment(JudgmentType.Bad, beats[0].poseId);
            }
        }

        private void JudgeInput(float inputTime, float targetTime, string poseId)
        {
            float diff = Mathf.Abs(inputTime - targetTime);
            if (diff <= _gameData.gameConfig.greatTimingWindow)
            {
                ShowJudgment(JudgmentType.Great, poseId);
                Managers.Sound.Play(Sound.SFX, greatSFX);
            }
            else if (diff <= _gameData.gameConfig.goodTimingWindow)
            {
                ShowJudgment(JudgmentType.Good, poseId);
                Managers.Sound.Play(Sound.SFX, greatSFX);
            }
            else
            {
                ShowJudgment(JudgmentType.Bad, poseId);
            }
        }

        private void ShowJudgment(JudgmentType type, string poseId)
        {
            effectManager.SpawnHeartParticles(type);
        }

        private void OnGameSuccess()
        {
            // 게임 성공 연출
            uiManager?.ShowSuccessPanel(true);
            // TODO: 성공 연출 추가
        }

        private void OnGameFail()
        {
            // 게임 실패 연출
            uiManager?.ShowFailPanel(true);
            // TODO: 실패 연출 추가
        }
    }
}