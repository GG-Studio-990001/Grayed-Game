using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Runtime.ETC;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Runtime.CH3.Dancepace
{
    public class GameFlowManager : MonoBehaviour
    {
        [Header("WaveSettings")]
        [Range(0, 2)]
        [Tooltip("0: Rehearsal Wave, 1: Main Wave 1, 2: Main Wave 2")]
        [SerializeField] private int _curWave;

        [Header("Managers")]
        [SerializeField] private EffectController effectManager;
        [SerializeField] private DPKeyBinder keyBinder;
        [SerializeField] private UIManager uiManager;

        [Header("Characters")]
        [SerializeField] private PreviewNPC[] previewNPCs;
        [SerializeField] private AnswerNPC[] answerNPCs;
        [SerializeField] private DPRapley playerCharacter;

        [Header("Data")]
        [SerializeField] private GameConfigSO _gameData;
        [SerializeField] private WaveDataSO _waveData;
        private GameResultData _gameResult;

        private List<WaveData> rehearsalWaves;
        private List<WaveData> mainWaves;
        private bool isRehearsalMode;
        private bool? userWantsMoreRehearsal = null;
        private float elapsed = 0f;

        private void Awake()
        {
            InitializeGameData();
            uiManager?.InitializeUI();
        }

        private void InitializeGameData()
        {
            rehearsalWaves = new List<WaveData>();
            mainWaves = new List<WaveData>();
            _gameResult = new GameResultData(0, 0, 0, 0);

            rehearsalWaves.Add(_waveData.waveDatas[_curWave]);
            mainWaves.Add(_waveData.waveDatas[1]);
            mainWaves.Add(_waveData.waveDatas[2]);
        }

        private IEnumerator GameFlow()
        {
            isRehearsalMode = true;
            uiManager?.SetRehearsalMode(true);

            bool isRehearsalContinue = true;
            while (isRehearsalContinue && isRehearsalMode)
            {
                effectManager.ShowAudience(0);
                userWantsMoreRehearsal = null;
                uiManager?.ShowKeyGuide(true);
                uiManager?.ShowTimeBar(false);
                foreach (var wave in rehearsalWaves)
                {
                    yield return StartCoroutine(PlayRoutine(wave, null));
                }

                uiManager?.ShowMoreRehearsalPanel(true);
                yield return new WaitUntil(() => userWantsMoreRehearsal != null);
                uiManager?.ShowMoreRehearsalPanel(false);
                isRehearsalContinue = userWantsMoreRehearsal == true;
            }

            effectManager.ShowAudience(1);
            uiManager?.ShowTimeBar(true);
            uiManager?.ShowKeyGuide(true);

            isRehearsalMode = false;
            foreach (var wave in mainWaves)
            {
                bool timeOver = false;
                yield return StartCoroutine(PlayRoutine(wave, (to) => timeOver = to));
                if (timeOver) break;
            }

            // 모든 웨이브가 끝나면 점수 결과만 보여줌
            uiManager?.ShowResultPanel(_gameResult.TotalScore, _gameResult.PerfectCnt, 
            _gameResult.GreatCnt, _gameResult.BadCnt);
        }

        private IEnumerator PlayRoutine(WaveData wave, Action<bool> onTimeOver)
        {
            if (wave == null || wave.beats == null || wave.beats.Count == 0)
            {
                Debug.LogError("Wave data is null or empty");
                yield break;
            }

            float limitTime = _gameData.limitTime;
            int waveCount = 0;
            bool timeOver = false;
            float wait = 3f;
            elapsed = 0f;

            Managers.Sound.Play(Sound.BGM, "Dancepace/CH3_SUB_BGM_01");
            effectManager.StartBeatAnimation();

            // 리허설 모드일 때: 타임바 완전 무시
            if (isRehearsalMode)
            {
                // 타임바 숨김
                uiManager?.ShowTimeBar(false);
                foreach (var waveData in rehearsalWaves)
                {
                    var beats = waveData.beats;
                    yield return StartCoroutine(PlayPreviewPhase(beats, 3f, float.PositiveInfinity, () => false));
                    yield return StartCoroutine(PlayInputPhase(beats, 3f, float.PositiveInfinity, () => false));
                }
                effectManager.StopBeatAnimation();
                yield break;
            }

            uiManager?.UpdateTimeBar(0f, limitTime);
            IEnumerator timeBarUpdater = TimeBarUpdater(limitTime, () => timeOver = true);
            Coroutine timeBarCoroutine = StartCoroutine(timeBarUpdater);

            Managers.Sound.Play(Sound.SFX, "Dancepace/CH3_SUB_SFX_99");
            uiManager?.ShowMcText();
            yield return StartCoroutine(UpdateTimeBarWithWait(4f, limitTime));

            while (waveCount < _gameData.waveForCount && !timeOver)
            {
                if (timeOver) break;
                int bitCount = UnityEngine.Random.Range(2, 5);
                var randomBeats = wave.beats.OrderBy(x => UnityEngine.Random.value).Take(bitCount).ToList();
                yield return PlayPreviewPhase(randomBeats, wait, limitTime, () => timeOver);
                if (timeOver) break;
                yield return PlayInputPhase(randomBeats, wait, limitTime, () => timeOver);
                waveCount++;
            }

            if (!timeOver && elapsed < limitTime)
                yield return HandleWaveIdle(limitTime);

            effectManager.StopBeatAnimation();
            onTimeOver?.Invoke(timeOver);
            yield return null;
        }

        private IEnumerator PlayPreviewPhase(List<BeatData> beats, float wait, float limitTime, Func<bool> isTimeOver)
        {
            Managers.Sound.Play(Sound.SFX, "Dancepace/CH3_Preview");
            foreach (var npc in previewNPCs)
                npc?.PlayPreviewPose(EPoseType.None);
            foreach (var npc in answerNPCs)
                npc?.PlayAnswerPose(EPoseType.None);
            yield return StartCoroutine(WaitWithTimeCheck(wait, limitTime, isTimeOver));

            for (int i = 0; i < beats.Count && !isTimeOver(); i++)
            {
                Managers.Sound.Play(Sound.SFX, "Dancepace/CH3_Good");
                var beat = beats[i];
                foreach (var npc in previewNPCs)
                    npc?.PlayPreviewPose(beat.poseData);
                float waitTime = beat.timing + beat.restTime;
                yield return StartCoroutine(WaitWithTimeCheck(waitTime, limitTime, isTimeOver));
            }

            Managers.Sound.Play(Sound.SFX, "Dancepace/CH3_Preview");
            playerCharacter?.StartSpotlightSequence(wait);
            //uiManager?.ShowTextBalloon(beats.Count > 0 ? beats[0].poseData : EPoseType.None);
            yield return StartCoroutine(WaitWithTimeCheck(wait, limitTime, isTimeOver));
        }

        private IEnumerator PlayInputPhase(List<BeatData> beats, float wait, float limitTime, Func<bool> isTimeOver)
        {
            // 리허설 모드일 때 한 비트마다 안내, 입력 전까지 멈춤, 맞게 누르면 피드백
            if (isRehearsalMode)
            {
                foreach (var beat in beats)
                {
                    uiManager?.ShowTextBalloon(beat.poseData);
                    // 해당 키를 누를 때까지 대기 (시간 멈춤)
                    while (!keyBinder.IsPoseKeyPressed(beat.EnumToString()))
                        yield return null;
                    // 피드백
                    uiManager?.ShowCustomTextBalloon("잘했어!", 0.7f, false);
                    yield return new WaitForSeconds(0.7f);
                }
                playerCharacter?.ResetState();
                playerCharacter?.HideSpotlight();
                yield break;
            }

            for (int i = 0; i < beats.Count && !isTimeOver(); i++)
            {
                Managers.Sound.Play(Sound.SFX, "Dancepace/CH3_Good");
                var beat = beats[i];
                foreach (var npc in answerNPCs)
                    npc?.PlayAnswerPose(beat.poseData);
                yield return StartCoroutine(JudgeBeatInputCoroutine(beat, beat.timing, limitTime));
                if (i + 1 < beats.Count && beat.restTime > 0)
                {
                    var nextBeat = beats[i + 1];
                    //uiManager?.ShowTextBalloon(nextBeat.poseData);
                    yield return StartCoroutine(WaitWithTimeCheck(beat.restTime, limitTime, isTimeOver));
                }
                else if (beat.restTime > 0)
                {
                    yield return StartCoroutine(WaitWithTimeCheck(beat.restTime, limitTime, isTimeOver));
                }
            }
            playerCharacter?.ResetState();
            playerCharacter?.HideSpotlight();
            uiManager?.HideTextBalloon();
        }

        private IEnumerator HandleWaveIdle(float limitTime)
        {
            foreach (var npc in previewNPCs) npc?.SetIdle();
            foreach (var npc in answerNPCs) npc?.SetIdle();
            playerCharacter?.ResetState();
            while (elapsed < limitTime)
                yield return null;
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

        // 시간 흐름을 체크하며 대기하는 코루틴
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

        private void JudgeInput(float inputTime, float beatTime, string poseId)
        {
            float ratio = inputTime / beatTime;
            float perfectMin = 0.5f - _gameData.perfectTimingWindow;
            float perfectMax = 0.5f + _gameData.perfectTimingWindow;
            float greatMin = 0.5f - _gameData.greatTimingWindow;
            float greatMax = 0.5f + _gameData.greatTimingWindow;

            if (ratio >= perfectMin && ratio <= perfectMax)
            {
                ShowJudgment(EJudgmentType.Perfect, poseId);
                //uiManager?.ShowTextBalloon(EJudgmentType.Perfect);
                _gameResult.AddPerfect();
                _gameResult.AddScore(_gameData.greatCoin);
            }
            else if (ratio >= greatMin && ratio <= greatMax)
            {
                //uiManager?.ShowTextBalloon(EJudgmentType.Great);
                ShowJudgment(EJudgmentType.Great, poseId);
                _gameResult.AddGreat();
                _gameResult.AddScore(_gameData.goodCoin);
            }
            else
            {
                //uiManager?.ShowTextBalloon(EJudgmentType.Bad);
                ShowJudgment(EJudgmentType.Bad, poseId);
                _gameResult.AddBad();
                _gameResult.AddScore(_gameData.badCoin);
            }
        }

        private void ShowJudgment(EJudgmentType type, string poseId)
        {
            effectManager.SpawnHeartParticles(type);
        }

        // 입력 판정(자동 진행) 코루틴 분리
        private IEnumerator JudgeBeatInputCoroutine(BeatData beat, float beatTime, float limitTime)
        {
            bool inputReceived = false;
            float timer = 0f;

            // 비트 시작 전에 이미 키가 눌려 있으면 Bad 판정
            if (keyBinder.IsPoseKeyHeld(beat.EnumToString()))
            {
                JudgeInput(float.MaxValue, beat.timing, beat.EnumToString());
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
                if (!inputReceived && keyBinder.IsPoseKeyPressed(beat.EnumToString()))
                {
                    float inputTime = timer;
                    JudgeInput(inputTime, beatTime, beat.EnumToString());
                    inputReceived = true;
                }
                if (!inputReceived && keyBinder.IsAnyOtherPoseKeyPressed(beat.EnumToString()))
                {
                    JudgeInput(float.MaxValue, beatTime, beat.EnumToString());
                    inputReceived = true;
                }
                timer += Time.deltaTime;
                elapsed += Time.deltaTime;
                yield return null;
            }
            if (!inputReceived)
                JudgeInput(float.MaxValue, beatTime, beat.EnumToString());
        }

        public void StartMainWaves()
        {
            userWantsMoreRehearsal = false;
        }

        public void RestartRehersal()
        {
            userWantsMoreRehearsal = true;
        }

        public void StartGame()
        {
            StartCoroutine(GameFlow());
        }
        public void EndGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            userWantsMoreRehearsal = false;
        }

        public void ReloadScene()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
            userWantsMoreRehearsal = true;
        }
    }
}