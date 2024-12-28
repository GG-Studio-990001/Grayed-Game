using Runtime.CH2.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace Runtime.CH2.Main
{
    public class CH2KeySetting : MonoBehaviour
    {
        [SerializeField] private DialogueRunner _runner;
        [SerializeField] private CH2Dialogue _dialogue;
        [SerializeField] private GameObject[] _uis;
        [SerializeField] private LineView _lineView;
        [SerializeField] private TextMeshProUGUI _lineTxt;
        [SerializeField] private GameObject _skipPanel;
        [SerializeField] private GameObject _continueBtn;
        [SerializeField] private Image _autoImg;
        [SerializeField] private Sprite[] _autoSpr;
        private bool _isHidingUI = false;
        private bool _isAutoAdvanced = false;
        // TODO: CH2Dialogue와 겹치는 변수는 직접 가져오기?

        private void OnContinueClicked()
        {
            if (!_lineView.gameObject.activeInHierarchy)
                return;

            _lineView.OnContinueClicked();
        }

        public void DialogueInput()
        {
            _dialogue.CancelDialogueCoroutine();
            OnContinueClicked();
        }

        public void HideUIToggle()
        {
            if (!_runner.IsDialogueRunning)
                return;

            if (_isHidingUI)
            {
                // 출력하던 중에 숨겨야 한다
                if (_lineTxt.maxVisibleCharacters != _lineTxt.text.Length)
                {
                    _lineTxt.maxVisibleCharacters = _lineTxt.text.Length;
                    _continueBtn.SetActive(true);
                }
            }

            _isHidingUI = !_isHidingUI;

            foreach (GameObject ui in _uis)
                ui.SetActive(!_isHidingUI);
        }

        public void AutoDialogue()
        {
            if (!_runner.IsDialogueRunning)
                return;

            _isAutoAdvanced = !_isAutoAdvanced;
            SetAutoSprite(_isAutoAdvanced);

            _dialogue.AutoDialogueToggle(_isAutoAdvanced);
            // 대사를 치고 1초 뒤에 다음 대사로 넘김
        }

        private void SetAutoSprite(bool isActive)
        {
            _autoImg.sprite = _autoSpr[isActive ? 1 : 0];
        }
        /*
        //TODO: _runner.IsDialogueRunning 말고 직접 다이얼로그 시작과 끝 설정
        public void Skip()
        {
            if (!_runner.IsDialogueRunning)
                return;

            _skipPanel.SetActive(true);
        }
        */
    }
}