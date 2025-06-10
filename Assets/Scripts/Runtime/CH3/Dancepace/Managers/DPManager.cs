using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;

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

        private List<WaveData> rehearsalWaves;
        private List<WaveData> mainWaves;
        private int currentWaveIndex;
        private bool isRehearsalMode = true;
        private bool userWantsMoreRehearsal = false;
        private DancepaceData _gameData;

        private void Awake()
        {
            Debug.Log("[DPManager] Awake 진입");
            InitializeGameData();
            if (uiManager == null) Debug.LogError("[DPManager] uiManager가 연결되어 있지 않습니다!");
            if (previewNPCs == null || previewNPCs.Length == 0) Debug.LogError("[DPManager] previewNPCs가 비어있습니다!");
            if (answerNPCs == null || answerNPCs.Length == 0) Debug.LogError("[DPManager] answerNPCs가 비어있습니다!");
            if (playerCharacter == null) Debug.LogError("[DPManager] playerCharacter가 연결되어 있지 않습니다!");
            uiManager?.InitializeUI();
        }

        private async void InitializeGameData()
        {
            // 스프레드시트 로드 부분 주석 처리
            // _gameData = new DancepaceData();
            // rehearsalWaves = new List<WaveData>();
            // mainWaves = new List<WaveData>();
            // try { ... } catch { ... }

            // 임시 하드코딩 데이터
            _gameData = new DancepaceData();
            _gameData.gameConfig = new GameConfig
            {
                limitTime = 60,
                waveForCount = 6,
                waveMainBGM = "-",
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

            Debug.Log("[DPManager] 임시 데이터로 게임 데이터 초기화 완료");
        }

        private void Start()
        {
            Debug.Log("[DPManager] Start 진입");
            StartCoroutine(GameFlow());
        }

        private IEnumerator GameFlow()
        {
            Debug.Log("[DPManager] GameFlow 진입");
            // 리허설 모드
            isRehearsalMode = true;
            uiManager?.ShowRehearsalPanel(true);
            uiManager?.SetRehearsalMode(true);
            Managers.Sound.Play(ETC.Sound.BGM, "SuperArio/CH2_SUB_BGM_01");
            
            do
            {
                Debug.Log("[DPManager] 리허설 웨이브 반복 시작");
                foreach (var wave in rehearsalWaves)
                {
                    Debug.Log($"[DPManager] 리허설 웨이브: {wave.waveId}");
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

            // 본게임 모드
            isRehearsalMode = false;
            Debug.Log("[DPManager] 메인 웨이브 반복 시작");
            foreach (var wave in mainWaves)
            {
                Debug.Log($"[DPManager] 메인 웨이브: {wave.waveId}");
                yield return StartCoroutine(PlayRoutine(wave));
            }
        }

        private IEnumerator PlayRoutine(WaveData wave)
        {
            Debug.Log($"[DPManager] PlayRoutine 진입: {wave?.waveId}");
            if (wave == null || wave.beats == null || wave.beats.Count == 0)
            {
                Debug.LogError("[DPManager] wave 또는 beats가 null/empty");
                yield break;
            }
            float waveDuration = wave.duration > 0 ? wave.duration : _gameData.gameConfig.limitTime;
            float elapsed = 0f;
            int beatIndex = 0;
            while (elapsed < waveDuration)
            {
                var beat = wave.beats[beatIndex];
                // 1. PreviewNPCs가 포즈 시연
                if (previewNPCs != null)
                {
                    foreach (var npc in previewNPCs)
                    {
                        if (npc != null)
                            npc.PlayPreviewPose(beat.poseId);
                    }
                }
                uiManager?.UpdateKeyGuide(beat.poseId ?? "", "");
                yield return new WaitForSeconds(beat.timing);
                elapsed += beat.timing;

                // 2. 쉬는 타임 (최소 0.5초 보장)
                float rest = Mathf.Max(beat.restTime, 0.5f);
                yield return new WaitForSeconds(rest);
                elapsed += rest;

                // 3. AnswerNPCs가 포즈 시연 + 플레이어 입력 대기
                if (answerNPCs != null)
                {
                    foreach (var npc in answerNPCs)
                    {
                        if (npc != null)
                            npc.PlayAnswerPose(beat.poseId);
                    }
                }
                uiManager?.UpdateKeyGuide(beat.poseId ?? "", "");
                yield return StartCoroutine(WaitForPlayerInput(beat));
                // (입력 대기 시간은 elapsed에 포함하지 않음)

                beatIndex = (beatIndex + 1) % wave.beats.Count;
            }
        }

        private IEnumerator WaitForPlayerInput(BeatData beat)
        {
            Debug.Log($"[DPManager] WaitForPlayerInput 진입: poseId={beat?.poseId}, timing={beat?.timing}");
            bool inputReceived = false;
            float startTime = Time.time;
            float maxWait = beat.timing * 2f;

            while (!inputReceived && Time.time - startTime < maxWait)
            {
                float currentTime = Time.time - startTime;
                uiManager?.UpdateTimeBar(currentTime, maxWait);

                if (keyBinder != null && keyBinder.IsPoseKeyPressed(beat.poseId))
                {
                    inputReceived = true;
                    float timing = Time.time - startTime;
                    JudgeInput(timing, beat.timing, beat.poseId);
                }
                yield return null;
            }

            if (!inputReceived)
            {
                Debug.LogWarning("[DPManager] 입력 미수행: Bad 처리");
                ShowJudgment(JudgmentType.Bad, beat.poseId);
            }
        }

        private void JudgeInput(float inputTime, float targetTime, string poseId)
        {
            float diff = Mathf.Abs(inputTime - targetTime);
            if (diff <= _gameData.gameConfig.greatTimingWindow)
            {
                ShowJudgment(JudgmentType.Great, poseId);
                effectManager.SpawnCoinEffect(playerCharacter.transform.position);
            }
            else if (diff <= _gameData.gameConfig.goodTimingWindow)
            {
                ShowJudgment(JudgmentType.Good, poseId);
                effectManager.SpawnCoinEffect(playerCharacter.transform.position);
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
    }
} 