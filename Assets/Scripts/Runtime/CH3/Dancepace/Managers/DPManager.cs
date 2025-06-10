using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;

namespace Runtime.CH3.Dancepace
{
    public class DPManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private DPEffectManager effectManager;
        [SerializeField] private DPKeyBinder keyBinder;

        [Header("Characters")]
        [SerializeField] private PreviewNPC[] previewNPCs; // 미리보기 NPC 4명
        [SerializeField] private AnswerNPC[] answerNPCs;   // 정답 NPC 3명
        [SerializeField] private DPRapley playerCharacter;

        [Header("UI")]
        [SerializeField] private GameObject rehearsalPanel;
        [SerializeField] private GameObject moreRehearsalPanel;

        // --- 데이터 ---
        private List<WaveData> rehearsalWaves;
        private List<WaveData> mainWaves;
        private int currentWaveIndex;
        private bool isRehearsalMode = true;
        private bool userWantsMoreRehearsal = false;
        private DancepaceData _gameData;

        private void Awake()
        {
            _gameData = new DancepaceData();
            rehearsalWaves = new List<WaveData>();
            mainWaves = new List<WaveData>();
            foreach (var wave in _gameData.waveDataList)
            {
                if (wave.waveId.EndsWith("_0")) rehearsalWaves.Add(wave);
                else mainWaves.Add(wave);
            }
        }

        private void Start()
        {
            StartCoroutine(GameFlow());
        }

        private IEnumerator GameFlow()
        {
            isRehearsalMode = true;
            do
            {
                foreach (var wave in rehearsalWaves)
                    yield return StartCoroutine(PlayRoutine(wave));
                moreRehearsalPanel.SetActive(true);
                userWantsMoreRehearsal = false;
                yield return new WaitUntil(() => userWantsMoreRehearsal == true || userWantsMoreRehearsal == false);
                moreRehearsalPanel.SetActive(false);
            }
            while (userWantsMoreRehearsal);

            isRehearsalMode = false;
            foreach (var wave in mainWaves)
                yield return StartCoroutine(PlayRoutine(wave));
        }

        private IEnumerator PlayRoutine(WaveData wave)
        {
            // 1. 미리보기 박자
            foreach (var beat in wave.previewBeats)
            {
                foreach (var npc in previewNPCs)
                    npc.PlayPreviewPose(beat.poseId);
                yield return new WaitForSeconds(beat.timing);
            }

            // 2. 쉬는 박자
            if (wave.restBeats.Count > 0)
                yield return new WaitForSeconds(wave.restBeats[0].timing);

            // 3. 플레이 박자(정답 NPC와 동기화)
            for (int i = 0; i < wave.playBeats.Count; i++)
            {
                foreach (var npc in answerNPCs)
                    npc.PlayAnswerPose(wave.playBeats[i].poseId);
                yield return StartCoroutine(WaitForPlayerInput(wave.playBeats[i]));
            }
        }

        private IEnumerator WaitForPlayerInput(BeatData beat)
        {
            bool inputReceived = false;
            float startTime = Time.time;
            float maxWait = beat.timing * 2f;

            while (!inputReceived && Time.time - startTime < maxWait)
            {
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