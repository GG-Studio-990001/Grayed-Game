using System.Collections;
using TMPro;
using UnityEngine;
using Yarn.Unity;
using System;
using UnityEngine.UI;

namespace Runtime.CH3.TRPG
{
    public class TrpgDialogue : LineView
    {
        [Header("=Script=")]
        [SerializeField] private DialogueRunner _runner;

        [Header("=Dialogue=")]
        [SerializeField] private Transform content;               // Scroll View → Content 오브젝트
        [SerializeField] private GameObject linePrefab;           // TextLine 프리팹
        [SerializeField] private ScrollRect scrollRect;           // Scroll Rect 컴포넌트

        [Header("=Result=")]
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private GameObject _diceRollObjects;
        [SerializeField] private DiceRoll _dice10;
        [SerializeField] private DiceRoll _dice1;
        [SerializeField] private TextMeshProUGUI _resultTxt_0;
        [SerializeField] private TextMeshProUGUI _resultTxt_1;
        [SerializeField] private TextMeshProUGUI _resultTxt_2;

        private void Awake()
        {
            _runner.AddCommandHandler<int>("RollDice", RollDice);
        }

        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            Debug.Log("대사 출력");
            // 1. 새 대사 오브젝트 생성
            GameObject lineObj = Instantiate(linePrefab, content);
            TextMeshProUGUI tmp = lineObj.GetComponentInChildren<TextMeshProUGUI>();
            CanvasGroup cg = lineObj.GetComponent<CanvasGroup>();

            // 2. 초기 설정
            if (cg == null)
            {
                cg = lineObj.AddComponent<CanvasGroup>();
            }
            tmp.text = dialogueLine.Text.Text;

            // 3. 텍스트 높이에 맞게 자동 조정
            AdjustTextHeight(lineObj, tmp);

            // 4. LineView의 필드들을 설정
            this.lineText = tmp;
            this.canvasGroup = cg;

            // 5. LineView의 기본 동작 사용 (페이드 인 + 타이핑)
            base.RunLine(dialogueLine, onDialogueLineFinished);

            // 6. 스크롤을 가장 밑으로 내리기
            StartCoroutine(ScrollToBottom());
        }

        private void AdjustTextHeight(GameObject lineObj, TextMeshProUGUI tmp)
        {
            // ContentSizeFitter가 없으면 추가
            ContentSizeFitter fitter = lineObj.GetComponent<ContentSizeFitter>();
            if (fitter == null)
            {
                fitter = lineObj.AddComponent<ContentSizeFitter>();
            }
            
            // 세로 크기만 자동 조정 (가로는 고정)
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            
            // 레이아웃 업데이트를 강제로 실행
            LayoutRebuilder.ForceRebuildLayoutImmediate(lineObj.GetComponent<RectTransform>());
        }

        private IEnumerator ScrollToBottom()
        {
            // 한 프레임 대기하여 레이아웃이 업데이트되도록 함
            yield return null;
            
            if (scrollRect != null)
            {
                // 스크롤을 가장 밑으로 내리기
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        // 페이드 아웃을 방지하기 위해 DismissLine을 오버라이드
        public override void DismissLine(Action onDismissalComplete)
        {
            // 페이드 아웃 없이 즉시 완료
            onDismissalComplete?.Invoke();
        }

        private void RollDice(int val)
        {
            StartCoroutine(ShowResult(val));
        }

        private IEnumerator ShowResult(int val)
        {
            if (val == 1)
            {
                yield return null;
                _runner.StartDialogue($"Warlocker_1_{val}");
                yield break;
            }

            _resultPanel.SetActive(true);
            _diceRollObjects.SetActive(true);

            string type = (val == 2 ? "근력" : "지능");
            _resultTxt_0.text = type + "\n40 / 100";

            _dice10.RollDice();
            while (_dice10.DiceFaceNum == -1) yield return null;
            int dice10 = _dice10.DiceFaceNum;
            _resultTxt_1.text = $"{dice10} + __ = ___";

            _dice1.RollDice();
            while (_dice1.DiceFaceNum == -1) yield return null;
            int dice1 = _dice1.DiceFaceNum;
            _resultTxt_1.text = $"{dice10} + {dice1} = ___";

            yield return new WaitForSeconds(1f);
            int sum = dice10 + dice1;
            sum = (sum == 0) ? 100 : sum;
            _resultTxt_1.text = $"{dice10} + {dice1} = {sum}";

            yield return new WaitForSeconds(1f);
            bool result = sum <= 40;
            string resultStr = result ? "성공" : "실패";
            string resultL = result ? "S" : "F";
            _resultTxt_2.text = "결과: " + resultStr;

            yield return new WaitForSeconds(2f);
            _resultPanel.SetActive(false);
            _diceRollObjects.SetActive(false);

            _runner.StartDialogue($"Warlocker_1_{val}_{resultL}");
        }
    }
}
