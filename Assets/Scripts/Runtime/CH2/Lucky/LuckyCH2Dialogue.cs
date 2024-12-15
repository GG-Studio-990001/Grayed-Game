using DG.Tweening;
using Runtime.ETC;
using Runtime.Lucky;
using Runtime.Middle;
using System;
using TMPro;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH2.Lucky
{
    public class LuckyCH2Dialogue : DialogueViewBase
    {
        [Header("=Lucky=")]
        private DialogueRunner _runner;
        [SerializeField] private GameObject[] _luckys;
        [SerializeField] private LuckyBody _lucky;
        [SerializeField] private RectTransform _bubble;
        [SerializeField] private RectTransform _bubbleImg;
        [SerializeField] private TextMeshProUGUI _lineTxt;
        [SerializeField] private GameObject _skipBtn;
        [SerializeField] private Vector3[] _leftPositions;
        [SerializeField] private Vector3[] _rightPositions;
        [SerializeField] private Vector3[] _bubblePositions;
        [Header("=Else=")]
        [SerializeField] private DialogueRunner _ch2DialogueRunner;
        [SerializeField] private ConnectionController _connectionController;
        private int _luckyProgress = 0;

        private void Awake()
        {
            _runner = GetComponent<DialogueRunner>();
            _runner.AddCommandHandler("ReverseConnection", _connectionController.ReverseConnection);

            _runner.AddCommandHandler("LuckyEnter", LuckyEnter);
            _runner.AddCommandHandler<int>("WalkLeft", WalkLeft);
            _runner.AddCommandHandler<int>("WalkRight", WalkRight);
            _runner.AddCommandHandler("Idle", Idle);
            _runner.AddCommandHandler<bool>("ActiveBubble", ActiveBubble);

            _runner.AddCommandHandler<int>("SetLuckyPos", SetLuckyPos);
            _runner.AddCommandHandler<int>("SetBubblePos", SetBubblePos);

            _runner.AddCommandHandler("ExitTcgPack", ExitTcgPack);

            _runner.AddCommandHandler<int>("SetLuckyProgress", SetLuckyProgress);
            _runner.AddCommandHandler("ActiveSkipBtn", ActiveSkipBtn);
        }

        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            ClearLineText();
            onDialogueLineFinished();
        }

        private void ClearLineText()
        {
            _lineTxt.text = "";
        }

        private void SetLuckyProgress(int idx)
        {
            _luckyProgress = idx;
        }

        private void ActiveSkipBtn()
        {
            _skipBtn.SetActive(true);
        }

        public void SkipBtnClicked()
        {
            _runner.Stop();

            switch (_luckyProgress)
            {
                case 0:
                    ExitTcgPack();
                    break;
            }
        }

        #region Common
        private void LuckyEnter()
        {
            Managers.Data.InGameKeyBinder.PlayerInputDisable();
            for (int i = 0; i < _luckys.Length; i++)
                _luckys[i].SetActive(true);

            Managers.Sound.Play(Sound.LuckyBGM, "Lucky_BGM_4");
        }

        private void LuckyExit()
        {
            Managers.Data.InGameKeyBinder.PlayerInputEnable();
            for (int i = 0; i < _luckys.Length; i++)
                _luckys[i].SetActive(false);
            Managers.Data.SaveGame();
        }

        private void WalkLeft(int idx)
        {
            _lucky.SetFlipX(false);
            _lucky.Anim.SetAnimation("Walking");
            _lucky.transform.DOLocalMove(_leftPositions[idx], 3f).SetEase(Ease.Linear);

            _bubbleImg.transform.localScale = new(1, 1, 1);
        }

        private void WalkRight(int idx)
        {
            _lucky.SetFlipX(true);
            _lucky.Anim.SetAnimation("Walking");
            _lucky.transform.DOLocalMove(_rightPositions[idx], 3f).SetEase(Ease.Linear);

            _bubbleImg.transform.localScale = new(-1, 1, 1);
        }

        private void Idle()
        {
            _lucky.Anim.ResetAnim();
            Debug.Log("Idle");
        }

        private void ActiveBubble(bool active)
        {
            _bubble.gameObject.SetActive(active);
        }
        #endregion

        #region 상황 따라 다르게
        private void SetLuckyPos(int idx)
        {
            _lucky.transform.position = _rightPositions[idx];
        }

        private void SetBubblePos(int idx)
        {
            _bubble.anchoredPosition = _bubblePositions[idx];
        }
        #endregion

        #region Exit
        // LuckyExit() 호출 필수
        private void ExitTcgPack()
        {
            _skipBtn.SetActive(false);
            LuckyExit();

            Managers.Data.SaveGame();
            Managers.Sound.Play(Sound.BGM, "CH2/BGM_#1_02", true);

            _ch2DialogueRunner.StartDialogue("Turn0_1");
        }
        #endregion
    }
}