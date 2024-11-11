using Runtime.CH2.Dialogue;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH2.Main
{
    public class KeySetting : MonoBehaviour
    {
        [SerializeField] private DialogueRunner _runner;
        [SerializeField] private CH2Dialogue _dialogue;
        [SerializeField] private GameObject[] _uis;
        [SerializeField] private GameObject _skipPanel;
        private bool isHidingUI = false;

        // TODO: _runner.IsDialogueRunning 말고 직접 다이얼로그 시작과 끝 설정
        public void Skip()
        {
            if (!_runner.IsDialogueRunning)
                return;

            _skipPanel.SetActive(true);
        }

        public void HideUI()
        {
            if (!_runner.IsDialogueRunning)
                return;

            isHidingUI = !isHidingUI;
            foreach (GameObject ui in _uis)
                ui.SetActive(!isHidingUI);
        }

        public void AutoDialogue()
        {
            if (!_runner.IsDialogueRunning)
                return;

            _dialogue.AutoDialogueToggle();
            // 대화 자동 진행 활성화
            // 직접 넘길 수 없음
            // 2초 뒤에 다음 대사로 넘김
            // 현재 Auto
        }
    }
}