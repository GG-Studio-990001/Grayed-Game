using UnityEngine;
using System.Collections;
using System;
using Runtime.ETC;
using System.Collections.Generic;

namespace Runtime.CH3.Dancepace
{
    public class RhythmManager : MonoBehaviour
    {
        public static RhythmManager Instance { get; private set; }

        [Header("Rhythm Settings")]
        [SerializeField] private float bpm = 120f;
        [SerializeField] private float beatInterval = 0.5f;
        [SerializeField] private float previewDuration = 2f;
        [SerializeField] private GameConfig gameConfig;

        [Header("=Components=")]
        [SerializeField] private SpeakerAnimation _speakerAnimation;
        [SerializeField] private AudioSource _beatSource;
        [SerializeField] private AudioClip _beatSound;

        [Header("=Beat Settings=")]
        [SerializeField] private float _previewBeatInterval = 0.5f;
        [SerializeField] private float _judgmentWindow = 0.1f;

        [Header("=Pose Data=")]
        [SerializeField] private List<PoseData> _poseDataList;

        public event Action<BeatData> OnPreviewBeat;
        public event Action<BeatData> OnPlayBeat;
        public event Action<BeatData> OnRestBeat;
        public event Action<JudgmentType> OnJudgment;

        private WaveData currentWave;
        private int currentPreviewIndex;
        private int currentRestIndex;
        private int currentPlayIndex;
        private float waveTimer;
        private bool _isWaveActive = false;
        public bool IsWaveActive => _isWaveActive;

        private bool _isPlaying = false;
        private float _currentTime = 0f;
        private float _nextBeatTime = 0f;
        private Coroutine _beatCoroutine;

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

            InitializePoseData();
        }

        private void InitializePoseData()
        {
            if (_poseDataList == null)
            {
                _poseDataList = new List<PoseData>
                {
                    new PoseData { poseId = "up", poseName = "Up", inputKey = KeyCode.UpArrow },
                    new PoseData { poseId = "down", poseName = "Down", inputKey = KeyCode.DownArrow },
                    new PoseData { poseId = "left", poseName = "Left", inputKey = KeyCode.LeftArrow },
                    new PoseData { poseId = "right", poseName = "Right", inputKey = KeyCode.RightArrow }
                };
            }
        }

        private void Start()
        {
            if (_speakerAnimation == null)
            {
                _speakerAnimation = FindObjectOfType<SpeakerAnimation>();
            }
        }

        public void StartWave(WaveData wave)
        {
            if (_isWaveActive)
            {
                Debug.LogWarning("Wave is already active");
                return;
            }

            currentWave = wave;
            currentPreviewIndex = 0;
            currentRestIndex = 0;
            currentPlayIndex = 0;
            waveTimer = 0f;
            _isWaveActive = true;

            StartCoroutine(WaveRoutine());
        }

        private IEnumerator WaveRoutine()
        {
            // 미리보기 박자 출력
            yield return StartCoroutine(PlayPreviewBeats());

            // +@ 박자 출력
            yield return StartCoroutine(PlayRestBeats());

            // 플레이 박자 시작
            yield return StartCoroutine(PlayGameBeats());

            // 웨이브 종료
            _isWaveActive = false;
        }

        private IEnumerator PlayPreviewBeats()
        {
            while (currentPreviewIndex < currentWave.previewBeats.Length)
            {
                OnPreviewBeat?.Invoke(currentWave.previewBeats[currentPreviewIndex]);
                currentPreviewIndex++;
                yield return new WaitForSeconds(beatInterval);
            }
        }

        private IEnumerator PlayRestBeats()
        {
            while (currentRestIndex < currentWave.restBeats.Length)
            {
                OnRestBeat?.Invoke(currentWave.restBeats[currentRestIndex]);
                currentRestIndex++;
                yield return new WaitForSeconds(beatInterval);
            }
        }

        private IEnumerator PlayGameBeats()
        {
            while (currentPlayIndex < currentWave.playBeats.Length && _isWaveActive)
            {
                OnPlayBeat?.Invoke(currentWave.playBeats[currentPlayIndex]);
                currentPlayIndex++;
                yield return new WaitForSeconds(beatInterval);
            }
        }

        public JudgmentType JudgeInput(KeyCode inputKey, float timing)
        {
            if (!_isWaveActive || currentPlayIndex == 0) return JudgmentType.Bad;

            BeatData currentBeat = currentWave.playBeats[currentPlayIndex - 1];
            PoseData poseData = GetPoseDataById(currentBeat.poseId);

            if (poseData == null || poseData.inputKey != inputKey)
            {
                return JudgmentType.Bad;
            }

            float timingDiff = Mathf.Abs(timing - currentBeat.timing);
            if (timingDiff <= gameConfig.greatTimingWindow)
            {
                return JudgmentType.Great;
            }
            else if (timingDiff <= gameConfig.goodTimingWindow)
            {
                return JudgmentType.Good;
            }

            return JudgmentType.Bad;
        }

        public PoseData GetPoseDataById(string poseId)
        {
            return _poseDataList.Find(pose => pose.poseId == poseId);
        }

        public void StopWave()
        {
            if (_isWaveActive)
            {
                _isWaveActive = false;
                StopAllCoroutines();
            }
        }

        public void StartRhythm()
        {
            if (_isPlaying) return;
            
            _isPlaying = true;
            _currentTime = 0f;
            _nextBeatTime = 0f;
            
            if (_beatCoroutine != null)
            {
                StopCoroutine(_beatCoroutine);
            }
            _beatCoroutine = StartCoroutine(BeatRoutine());
        }

        public void StopRhythm()
        {
            _isPlaying = false;
            if (_beatCoroutine != null)
            {
                StopCoroutine(_beatCoroutine);
                _beatCoroutine = null;
            }
            _speakerAnimation?.StopBeatAnimation();
        }

        private IEnumerator BeatRoutine()
        {
            while (_isPlaying)
            {
                _currentTime += Time.deltaTime;

                if (_currentTime >= _nextBeatTime)
                {
                    // 비트 발생
                    PlayBeat();
                    _nextBeatTime = _currentTime + beatInterval;
                }

                yield return null;
            }
        }

        private void PlayBeat()
        {
            // 비트 사운드 재생
            if (_beatSource != null && _beatSound != null)
            {
                _beatSource.PlayOneShot(_beatSound);
            }

            // 스피커 애니메이션 실행
            _speakerAnimation?.StartBeatAnimation();
        }

        public void StartPreview()
        {
            if (_beatCoroutine != null)
            {
                StopCoroutine(_beatCoroutine);
            }
            _beatCoroutine = StartCoroutine(PreviewRoutine());
        }

        private IEnumerator PreviewRoutine()
        {
            while (_isPlaying)
            {
                _currentTime += Time.deltaTime;

                if (_currentTime >= _nextBeatTime)
                {
                    // 프리뷰 비트 발생
                    PlayBeat();
                    _nextBeatTime = _currentTime + _previewBeatInterval;
                }

                yield return null;
            }
        }

        public bool IsInJudgmentWindow(float inputTime)
        {
            float timeDiff = Mathf.Abs(inputTime - _currentTime);
            return timeDiff <= _judgmentWindow;
        }

        private void OnDisable()
        {
            StopRhythm();
        }
    }
} 