using Runtime.CH1.Main.Player;
using Runtime.ETC;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH1.Main
{
    public class PackController : MonoBehaviour
    {
        // 기계어 번역팩 관련
        [SerializeField] private TopDownPlayer _player;
        [SerializeField] private GameObject _pack;
        [SerializeField] private DialogueRunner _runner;
        [SerializeField] private SLGActionComponent _slg;
        [SerializeField] private GameObject[] _mamagoUIs;
        [SerializeField] private NewBlink[] newBlinks;
        [SerializeField] private GameObject[] _transBtns;

        public void GetPack()
        {
            _player.Animation.SetAnimation(PlayerState.Get.ToString(), Vector2.down);
            _pack.SetActive(true);
            Managers.Sound.Play(ETC.Sound.SFX, "CH1/GetItem_SFX");
        }

        public void FinishPack()
        {
            _pack.SetActive(false);
            _player.Animation.SetAnimation(PlayerState.Idle.ToString(), Vector2.down);
        }

        public void BuyTranslator()
        {
            if (Managers.Data.Common.Coin < 10)
                return;

            Managers.Data.Common.Coin -= 10;
            _slg.RefreshCoinText();

            _runner.Stop();
            _runner.StartDialogue("TranslatorPack");
            Managers.Data.Common.Translator = 1;

            EquipTranslator();

            foreach (GameObject go in _mamagoUIs)
                go.SetActive(false);
        }

        public void EquipTranslator(int step = 0)
        {
            switch (step)
            {
                case 0:
                    // new 깜빡이기 시작
                    newBlinks[0].StartBlinking();
                    break;
                case 1:
                    // 설정창 누름
                    newBlinks[0].StopBlinking();
                    newBlinks[1].StartBlinking();
                    break;
                case 2:
                    // 번역탭 누름
                    newBlinks[1].StopBlinking();
                    newBlinks[2].StartBlinking();
                    break;
                case 3:
                    // 번역기 누름
                    newBlinks[2].StopBlinking();

                    _runner.Stop();
                    _runner.StartDialogue("EquipTranslator");
                    break;
            }

            if (step < 3)
                ActiveStepBtn(step);
        }

        private void ActiveStepBtn(int step)
        {
            _transBtns[step].SetActive(true);
            // 버튼0=설정창>EquipTranslator1, 버튼1=번역탭>EquipTranslator2, 버튼2=번역기>EquipTranslator3
        }
    }
}