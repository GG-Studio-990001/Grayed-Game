using System.Collections;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH3.Main
{
    public class CH3Dialogue : MonoBehaviour
    {
        [SerializeField] private ShopUIController _shopUIController;
        private DialogueRunner _runner;
        private LineView _lineView;

        public LineView LineView => _lineView;

        private void Awake()
        {
            _runner = GetComponent<DialogueRunner>();
            _lineView = _runner.dialogueViews[0].GetComponent<LineView>();
            _runner.AddCommandHandler("OpenShop", OpenShop);
            _runner.AddCommandHandler("SetFirstMeet", SetFirstMeet);
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
