using UnityEngine;
using System.Collections;
using Runtime.Common;
using Runtime.Common.View;
using Runtime.ETC;
using System;

namespace Runtime.CH3.Dancepace
{
    public class RehearsalManager : MonoBehaviour
    {
        public static RehearsalManager Instance { get; private set; }

        [Header("Rehearsal Settings")]
        [SerializeField] private WaveData[] tutorialWaves;
        [SerializeField] private WaveData rehearsalWave;

        public event Action OnRehearsalComplete;
        public event Action OnRehearsalSkip;

        private int currentTutorialIndex = 0;
        private bool _isRehearsalActive = false;
        public bool IsRehearsalActive => _isRehearsalActive;

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
        }

        public void StartRehearsal()
        {
            _isRehearsalActive = true;
            currentTutorialIndex = 0;
            StartCoroutine(RehearsalRoutine());
        }

        private IEnumerator RehearsalRoutine()
        {
            // 기본 포즈 튜토리얼
            while (currentTutorialIndex < tutorialWaves.Length)
            {
                yield return StartCoroutine(PlayTutorialWave(tutorialWaves[currentTutorialIndex]));
                currentTutorialIndex++;
            }

            // 전체 리허설
            yield return StartCoroutine(PlayRehearsalWave());

            // 추가 리허설 여부 확인
            ShowRehearsalPrompt();
        }

        private IEnumerator PlayTutorialWave(WaveData wave)
        {
            // TODO: Implement tutorial wave UI and instructions
            RhythmManager.Instance.StartWave(wave);
            yield return new WaitForSeconds(wave.duration);
        }

        private IEnumerator PlayRehearsalWave()
        {
            // TODO: Implement rehearsal wave UI and instructions
            RhythmManager.Instance.StartWave(rehearsalWave);
            yield return new WaitForSeconds(rehearsalWave.duration);
        }

        private void ShowRehearsalPrompt()
        {
            // TODO: Implement UI prompt for additional rehearsal
            // "더 리허설할래?" 메시지 표시
        }

        public void OnRehearsalPromptResponse(bool wantMoreRehearsal)
        {
            if (wantMoreRehearsal)
            {
                StartRehearsal();
            }
            else
            {
                _isRehearsalActive = false;
                OnRehearsalComplete?.Invoke();
            }
        }

        public void SkipRehearsal()
        {
            if (_isRehearsalActive)
            {
                _isRehearsalActive = false;
                StopAllCoroutines();
                OnRehearsalSkip?.Invoke();
            }
        }
    }
} 