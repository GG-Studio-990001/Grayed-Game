using System.Collections;
using System.Collections.Generic;
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

        public LineView LineView => _lineView;

        private void Awake()
        {
            _runner = GetComponent<DialogueRunner>();
            _lineView = _runner.dialogueViews[0].GetComponent<LineView>();
            _runner.AddCommandHandler("OpenShop", OpenShop);
        }

        public void OnDialogueInput()
        {
            if (_lineView == null || !_lineView.gameObject.activeInHierarchy || !_runner.IsDialogueRunning)
                return;

            // 상점이 열려있을 때는 대화 입력을 무시
            if (_shopUIController != null && _shopUIController.IsOpen)
                return;

            // InputSystem의 .performed는 프레임의 특정 시점에 호출되어
            // DialogueRunner의 커맨드 처리 타이밍과 맞지 않을 수 있습니다.
            // 다음 프레임으로 지연시켜 Button 클릭과 동일한 타이밍으로 실행되도록 합니다.
            StartCoroutine(DelayedContinue());
        }


        private IEnumerator DelayedContinue()
        {
            // 한 프레임 대기하여 DialogueRunner가 커맨드를 처리할 수 있도록 함
            yield return null;
            
            if (_lineView != null && _lineView.gameObject.activeInHierarchy && _runner.IsDialogueRunning)
            {
                _lineView.OnContinueClicked();
            }
        }

        public void OpenShop()
        {
            _shopUIController.OpenShop();
        }
    }
}
