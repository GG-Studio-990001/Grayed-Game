using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;

namespace Runtime.CH3.Dancepace
{
    public class DancepaceManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private EffectManager effectManager;
        [SerializeField] private PoseIndicator poseIndicator;
        [SerializeField] private DancepaceKeyBinder keyBinder;

        [Header("Characters")]
        [SerializeField] private DPRapley[] previewNPCs; // 미리보기 NPC 4명
        [SerializeField] private DPRapley playerCharacter; // 플레이어

        [Header("UI")]
        [SerializeField] private GameObject rehearsalPanel;
        [SerializeField] private GameObject moreRehearsalPanel;

        // --- 데이터 ---
        private List<WaveData> rehearsalWaves; // id가 _0인 웨이브
        private List<WaveData> mainWaves;      // id가 _0이 아닌 웨이브
        private int currentWaveIndex;
        private bool isRehearsalMode = true;
        private bool userWantsMoreRehearsal = false;
        private DancepaceData _gameData;

        private void Awake()
        {
            // 데이터 로드 및 분리
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
            // 1. 리허설 강제 1회
            isRehearsalMode = true;
            do
            {
                foreach (var wave in rehearsalWaves)
                    yield return StartCoroutine(PlayRoutine(wave));
                // "더 연습할래?" 패널 활성화, 유저 선택 대기
                moreRehearsalPanel.SetActive(true);
                userWantsMoreRehearsal = false;
                // UI에서 버튼 클릭 시 userWantsMoreRehearsal 값을 true/false로 변경하도록 연결 필요
                yield return new WaitUntil(() => userWantsMoreRehearsal == true || userWantsMoreRehearsal == false);
                moreRehearsalPanel.SetActive(false);
            }
            while (userWantsMoreRehearsal);

            // 2. 본게임
            isRehearsalMode = false;
            foreach (var wave in mainWaves)
                yield return StartCoroutine(PlayRoutine(wave));
        }

        private IEnumerator PlayRoutine(WaveData wave)
        {
            // 1. 미리보기
            foreach (var beat in wave.previewBeats)
            {
                foreach (var npc in previewNPCs)
                    npc.PlayPose(beat.poseId);
                // 포즈 UI 표시
                poseIndicator.ShowPose(beat.poseId, null, beat.poseId);
                yield return new WaitForSeconds(beat.timing);
            }

            // 2. 쉬는 박자
            if (wave.restBeats.Count > 0)
                yield return new WaitForSeconds(wave.restBeats[0].timing);

            // 3. 따라하기
            for (int i = 0; i < wave.playBeats.Count; i++)
            {
                poseIndicator.ShowPose(wave.playBeats[i].poseId, null, wave.playBeats[i].poseId);
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
                // 바인딩된 키로 입력 체크
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
            // 판정 텍스트는 UI에 표시하지 않음, 하트 파티클만 등급별로 생성
            effectManager.SpawnHeartParticles(type);
        }

        private Vector2 PoseToVector2(string poseId)
        {
            switch (poseId)
            {
                case "Up": return Vector2.up;
                case "Down": return Vector2.down;
                case "Left": return Vector2.left;
                case "Right": return Vector2.right;
                default: return Vector2.zero;
            }
        }
    }
} 