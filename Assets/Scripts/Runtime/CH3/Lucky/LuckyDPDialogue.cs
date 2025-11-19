using DG.Tweening;
using Runtime.ETC;
using Runtime.Lucky;
using UnityEngine;
using Yarn.Unity;

public class LuckyDPDialogue : MonoBehaviour
{
        private DialogueRunner _runner;
        [SerializeField] private LuckyBody _lucky;
        [SerializeField] private GameObject _bubble;
        [SerializeField] private Vector3 _outPosition;
        [SerializeField] private Vector3 _inPosition;
        [SerializeField] private GameObject[] _luckyObjs;
        [SerializeField] private GameObject _timeline;

        private void Awake()
        {
            _lucky.transform.localPosition = _outPosition;

            _runner = GetComponent<DialogueRunner>();
            _runner.AddCommandHandler("LuckyEnter", LuckyEnter);
            _runner.AddCommandHandler("LuckyExit", LuckyExit);
            _runner.AddCommandHandler("WalkIn", WalkIn);
            _runner.AddCommandHandler("WalkOut", WalkOut);
            _runner.AddCommandHandler<bool>("ActiveBubble", ActiveBubble);
            _runner.AddCommandHandler("Idle", Idle);
        }

        private void Start()
        {
            _outPosition = new Vector3(773, -201, 0);
            _inPosition = new Vector3(475, -201, 0);
            
            if (Managers.Data.CH3.IsDancepacePlayed)
                LuckyExit();
        }

        private void LuckyEnter()
        {
            Managers.Sound.Play(Sound.LuckyBGM, "Lucky_BGM_4");
        }

        private void LuckyExit()
        {
            Managers.Sound.StopBGM();
            for (int i=0; i< _luckyObjs.Length; i++)
                _luckyObjs[i].SetActive(false);
            _timeline.SetActive(true);
        }

        private void WalkIn()
        {
            _lucky.Anim.SetAnimation("Walking"); // enum으로 변경
            _lucky.transform.DOLocalMove(_inPosition, 3f).SetEase(Ease.Linear);
        }

        private void WalkOut()
        {
            _lucky.Anim.SetAnimation("Walking");
            _lucky.SetFlipX(true);
            _lucky.transform.DOLocalMove(_outPosition, 3f).SetEase(Ease.Linear);
        }

        private void Idle()
        {
            _lucky.SetFlipX(false);
            _lucky.Anim.ResetAnim();
        }

        private void ActiveBubble(bool active)
        {
            _bubble.SetActive(active);
        }
}
