using DG.Tweening;
using Runtime.ETC;
using Runtime.Lucky;
using Runtime.Middle;
using System;
using TMPro;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH1.Lucky
{
    public class LuckyCH0Dialogue : DialogueViewBase
    {
        private DialogueRunner _runner;
        [SerializeField] private GameObject[] _luckys;
        [SerializeField] private LuckyBody _lucky;
        [SerializeField] private RectTransform _bubble;
        [SerializeField] private RectTransform _bubbleImg;
        [SerializeField] private Vector3 _leftPosition;
        [SerializeField] private Vector3 _rightPosition;
        [SerializeField] private Vector3 _bubblePosition;
        [SerializeField] private Transform _player;
        [SerializeField] private TextMeshProUGUI _lineTxt;
        [SerializeField] private ConnectionController _connectionController;
        [SerializeField] private GameObject _object;

        private void Awake()
        {
            _runner = GetComponent<DialogueRunner>();
            _runner.AddCommandHandler("LuckyEnter", LuckyEnter);
            _runner.AddCommandHandler("WalkLeft", WalkLeft);
            _runner.AddCommandHandler("WalkRight", WalkRight);
            _runner.AddCommandHandler("Idle", Idle);
            _runner.AddCommandHandler<bool>("ActiveBubble", ActiveBubble);

            _runner.AddCommandHandler("SetLuckyPos", SetLuckyPos);
            _runner.AddCommandHandler("SetBubblePos", SetBubblePos);

            _runner.AddCommandHandler("RemoveObject", RemoveObject);
            _runner.AddCommandHandler("ExitFirstMeet", ExitFirstMeet);
            _runner.AddCommandHandler("ReverseConnection", _connectionController.ReverseConnection);
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

        private void WalkLeft()
        {
            _lucky.SetFlipX(false);
            _lucky.Anim.SetAnimation("Walking"); // enum으로 변경
            _lucky.transform.DOLocalMove(_leftPosition, 3f).SetEase(Ease.Linear);

            _bubbleImg.transform.localScale = new(1, 1, 1);
        }

        private void WalkRight()
        {
            _lucky.SetFlipX(true);
            _lucky.Anim.SetAnimation("Walking");
            _lucky.transform.DOLocalMove(_rightPosition, 3f).SetEase(Ease.Linear);

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

        private void SetLuckyPos()
        {
            _lucky.transform.position = _rightPosition;
        }

        private void SetBubblePos()
        {
            _bubble.anchoredPosition = _bubblePosition;
        }

        private void RemoveObject()
        {
            _object.SetActive(false);
        }

        private void ExitFirstMeet()
        {
            LuckyExit();

            Managers.Data.CH1.MeetLucky = true; // TODO: 데이터 제거?
            Managers.Data.SaveGame();

            Managers.Sound.StopBGM();
        }
    }
}