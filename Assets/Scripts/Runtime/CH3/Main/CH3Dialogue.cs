using System.Collections;
using TMPro;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH3.Main
{
    public class CH3Dialogue : MonoBehaviour
    {
        [SerializeField] private ShopUIController _shopUIController;
        private DialogueRunner _runner;
        private LineView _lineView;
        private TextMeshProUGUI _lineText;

        public LineView LineView => _lineView;

        private void Awake()
        {
            _runner = GetComponent<DialogueRunner>();
            _lineView = _runner.dialogueViews[0].GetComponent<LineView>();
            
            // LineView의 lineText 필드 가져오기 (리플렉션 사용)
            var lineTextField = typeof(LineView).GetField("lineText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (lineTextField != null)
            {
                _lineText = lineTextField.GetValue(_lineView) as TextMeshProUGUI;
            }
            
            // 대화 완료 시 텍스트 초기화 (마지막 대화를 빈칸으로)
            _runner.onDialogueComplete.AddListener(ClearLineText);
            
            _runner.AddCommandHandler("OpenShop", OpenShop);
            _runner.AddCommandHandler("SetFirstMeet", SetFirstMeet);
        }

        private void OnDestroy()
        {
            if (_runner != null)
            {
                _runner.onDialogueComplete.RemoveListener(ClearLineText);
            }
        }

        private void ClearLineText()
        {
            if (_lineText != null)
            {
                _lineText.text = "";
            }
        }

        public void OnDialogueInput()
        {
            if (_lineView == null || !_lineView.gameObject.activeInHierarchy || !_runner.IsDialogueRunning)
                return;

            if (_shopUIController != null && _shopUIController.IsOpen)
                return;

            StartCoroutine(DelayedContinue());
        }

        private IEnumerator DelayedContinue()
        {
            yield return null;
            
            if (_lineView != null && _lineView.gameObject.activeInHierarchy && _runner.IsDialogueRunning)
            {
                _lineView.OnContinueClicked();
            }
        }

        private void OpenShop()
        {
            _shopUIController.OpenShop();
        }

        private void SetFirstMeet()
        {
            Managers.Data.CH3.IsFirstMeet = true;
            Managers.Data.SaveGame();
        }
    }
}
