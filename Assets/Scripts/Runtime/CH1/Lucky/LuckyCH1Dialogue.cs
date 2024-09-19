using Codice.CM.Common.Merge;
using DG.Tweening;
using Runtime.CH1.SubB;
using Runtime.ETC;
using Runtime.Lucky;
using Runtime.Middle;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH1.Lucky
{
    public class LuckyCH1Dialogue : DialogueViewBase
    {
        private DialogueRunner _runner;
        [SerializeField] private GameObject[] _luckys;
        [SerializeField] private LuckyBody _lucky;
        [SerializeField] private RectTransform _bubble;
        [SerializeField] private RectTransform _bubbleImg;
        [SerializeField] private Vector3[] _leftPositions;
        [SerializeField] private Vector3[] _rightPositions;
        [SerializeField] private Vector3[] _bubblePositions;
        [SerializeField] private GameObject _fish;
        [SerializeField] private Transform _player;
        [SerializeField] private TextMeshProUGUI _lineTxt;
        [SerializeField] private GameObject _drawing;
        [SerializeField] private ConnectionController _connectionController;
        [SerializeField] private DialogueRunner _ch1Runner;
        [SerializeField] private GameObject _cursorImage;

        private void Awake()
        {
            _runner = GetComponent<DialogueRunner>();
            _runner.AddCommandHandler("LuckyEnter", LuckyEnter);
            _runner.AddCommandHandler<int>("WalkLeft", WalkLeft);
            _runner.AddCommandHandler<int>("WalkRight", WalkRight);
            _runner.AddCommandHandler("Idle", Idle);
            _runner.AddCommandHandler<bool>("ActiveBubble", ActiveBubble);

            _runner.AddCommandHandler("Showing", Showing);
            _runner.AddCommandHandler<bool>("ShowDrawing", ShowDrawing);

            _runner.AddCommandHandler<int>("SetLuckyPos", SetLuckyPos);
            _runner.AddCommandHandler<int>("SetBubblePos", SetBubblePos);

            _runner.AddCommandHandler<bool>("ShowCursor", ShowCursor);
            _runner.AddCommandHandler("WaitAssetInput", WaitAssetInput);
            _runner.AddCommandHandler("WaitWindowInput", WaitWindowInput);

            _runner.AddCommandHandler("ExitTranslator", ExitTranslator);
            _runner.AddCommandHandler("ExitFirstMeet", ExitFirstMeet);
            _runner.AddCommandHandler("Exit3Match", Exit3Match);
            _runner.AddCommandHandler("ExitSLG", ExitSLG);

            _runner.AddCommandHandler("ActiveFish", ActiveFish);
            _runner.AddCommandHandler("ReverseConnection", _connectionController.ReverseConnection);
        }

        private void Showing()
        {
            _lucky.Anim.SetAnimation("Showing");
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

        private void ShowDrawing(bool active)
        {
            if (active)
            {
                Invoke(nameof(ActiveDrawing), 1f);
            }
            else
            {
                CancelInvoke(nameof(ActiveDrawing));
                _drawing.SetActive(false);
            }
        }

        private void ActiveDrawing()
        {
            _drawing.SetActive(true);
        }

        private void ShowCursor(bool InFlag)
        {
            if(_cursorImage)
            {
                _cursorImage.SetActive(InFlag);
            }
        }

        private void WaitAssetInput()
        {
            SLGActionComponent slgAction = FindObjectOfType<SLGActionComponent>();
            if (slgAction != null)
            {
                slgAction.WaitAssetInput();
            }
        }

        private void WaitWindowInput()
        {
            SLGActionComponent slgAction = FindObjectOfType<SLGActionComponent>();
            if (slgAction != null)
            {
                slgAction.WaitWindowInput();
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
            _lucky.Anim.SetAnimation("Walking"); // enum으로 변경
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
            if (idx == 0)
            {
                _lucky.transform.position = _leftPositions[idx];
            }
            else
            {
                _lucky.transform.position = _rightPositions[idx];
            }
        }

        private void SetBubblePos(int idx)
        {
            // 예외처리 하드코딩
            if (idx == 2 && _player.position.y > 0)
            {
                _bubble.anchoredPosition = new Vector3(_bubblePositions[idx].x, 114, _bubblePositions[idx].z);
                return;
            }

            _bubble.anchoredPosition = _bubblePositions[idx];
        }
        #endregion

        #region Exit
        // LuckyExit() 호출 필수
        private void ExitTranslator()
        {
            LuckyExit();

            _ch1Runner.StartDialogue("S6");

            Managers.Data.SaveGame();

            Managers.Sound.Play(Sound.BGM, "Mamago_BGM_1", true);
        }

        private void ExitFirstMeet()
        {
            LuckyExit();

            Managers.Data.MeetLucky = true;
            Managers.Data.SaveGame();

            Managers.Sound.Play(Sound.BGM, "CH1/Main_BGM", true);
        }

        private void Exit3Match()
        {
            LuckyExit();

            Managers.Sound.Play(Sound.BGM, "CH1/Main(Cave)_BGM", true);
        }

        private void ExitSLG()
        {
            LuckyExit();

            SLGActionComponent slgAction = FindObjectOfType<SLGActionComponent>();
            if (slgAction != null)
            {
                slgAction.MoveOnNextProgress();
            }

            Managers.Sound.Play(Sound.BGM, "CH1/Main_BGM", true);
        }
        #endregion

        private void ActiveFish()
        {
            Managers.Sound.Play(Sound.SFX, "CH1/FishJelly_SFX");
            
            var jewelry = _fish.GetComponent<Jewelry>();
            
            _fish.transform.localPosition = new(-3.5f, -0.5f, 0);
            jewelry.ChangeOriginalPosition(_fish.transform.position);
            jewelry.JewelryType = JewelryType.B;
            jewelry.gameObject.SetActive(true      );
            _fish.name = "Jewelry_B";
            
            jewelry.Controller.CheckMatching();
        }
    }
}