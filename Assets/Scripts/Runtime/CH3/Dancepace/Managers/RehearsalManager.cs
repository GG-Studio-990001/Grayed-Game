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
        [SerializeField] private float poseDisplayTime = 2f;
        [SerializeField] private float transitionTime = 1f;

        [Header("UI References")]
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private GameObject poseGuidePanel;
        [SerializeField] private GameObject controlPanel;

        [Header("Audio")]
        [SerializeField] private AudioClip poseSound;
        [SerializeField] private AudioClip successSound;

        public event Action OnRehearsalComplete;
        public event Action OnRehearsalSkip;

        private int currentPoseIndex = 0;
        private bool isRehearsing = false;

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
            isRehearsing = true;
            currentPoseIndex = 0;
            ShowTutorialPanel();
        }

        private void ShowTutorialPanel()
        {
            tutorialPanel.SetActive(true);
            poseGuidePanel.SetActive(false);
            controlPanel.SetActive(false);
        }

        public void StartPoseGuide()
        {
            tutorialPanel.SetActive(false);
            poseGuidePanel.SetActive(true);
            controlPanel.SetActive(true);
            StartCoroutine(ShowPoseGuide());
        }

        private IEnumerator ShowPoseGuide()
        {
            // 포즈 가이드 표시 로직
            yield return new WaitForSeconds(poseDisplayTime);
            // 다음 포즈로 이동
        }

        public void SkipRehearsal()
        {
            isRehearsing = false;
            OnRehearsalSkip?.Invoke();
        }

        public void CompleteRehearsal()
        {
            isRehearsing = false;
            OnRehearsalComplete?.Invoke();
        }
    }
} 