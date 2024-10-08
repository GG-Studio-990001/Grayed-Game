using Runtime.CH1.Main.Player;
using Runtime.ETC;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH1.Main.Dialogue
{
    public class TranslatorPack : MonoBehaviour
    {
        // 기계어 번역팩 관련
        [SerializeField] private TopDownPlayer _player;
        [SerializeField] private GameObject _pack;
        [SerializeField] private DialogueRunner _runner;
        [SerializeField] private SLGActionComponent _slg;
        [SerializeField] private GameObject[] _mamagoUIs;

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
            if (Managers.Data.CH1.PacmomCoin < 10)
                return;

            Managers.Data.CH1.PacmomCoin -= 10;
            _slg.RefreshCoinText();
            StartPackDialogue();

            foreach (GameObject go in _mamagoUIs)
                go.SetActive(false);
        }

        private void StartPackDialogue()
        {
            _runner.Stop();
            _runner.StartDialogue("TranslatorPack");
        }
    }
}