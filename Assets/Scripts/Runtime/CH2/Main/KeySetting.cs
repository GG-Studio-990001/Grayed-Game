using Runtime.CH2.Dialogue;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH2.Main
{
    public class KeySetting : MonoBehaviour
    {
        [SerializeField] private DialogueRunner _runner;
        [SerializeField] private GameObject _dialogueBox;
        [SerializeField] private GameObject _skipPanel;
        // private bool isHidingUI = false;
        // private bool isAutoDialogue = false;

        // 아래 4개의 기능은 다이얼로그 출력 중에만 실행 가능?

        public void Skip()
        {
            if (!_runner.IsDialogueRunning)
                return;

            _skipPanel.SetActive(true);
        }

        public void HideUI()
        {
            // UI 숨기기 가능한지 체크
            // 다이얼로그 박스를 포함한 모든 UI 숨기기
            // 숨긴 상태라면 UI 활성화
        }

        public void AutoDialogue()
        {
            // 대화 자동 진행 활성화
        }

        public void ViewLog()
        {
            // 대화 내역 보기
        }
    }
}