using System.Collections;
using TMPro;
using UnityEngine;
using Yarn.Unity;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Runtime.CH3.TRPG
{
    public class TrpgDialogue : DialogueViewBase
    {
        [Header("=Script=")]
        [SerializeField] private DialogueRunner _runner;
        [SerializeField] private TrpgLine _trpgLine;

        [Header("=Dialogue=")]
        [SerializeField] private Transform content;               // Scroll View → Content 오브젝트
        [SerializeField] private GameObject linePrefab;           // TextLine 프리팹
        [SerializeField] private ScrollRect scrollRect;           // Scroll Rect 컴포넌트

        [Header("=Options=")]
        [SerializeField] private GameObject optionPrefab;         // 옵션 프리팹
        [SerializeField] private Color optionHoverColor = new(1f, 1f, 1f, 0.3f); // 마우스 오버 색상
        [SerializeField] private Color optionSelectedColor = new(1f, 1f, 1f, 0.5f); // 선택된 색상

        [Header("=Result=")]
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private GameObject _diceRollObjects;
        [SerializeField] private DiceRoll _dice10;
        [SerializeField] private DiceRoll _dice1;
        [SerializeField] private TextMeshProUGUI _resultTxt_0;
        [SerializeField] private TextMeshProUGUI _resultTxt_1;
        [SerializeField] private TextMeshProUGUI _resultTxt_2;

        private List<GameObject> currentOptions = new();
        private bool isShowingOptions = false;
        private bool isWaitingForInput = false; // 키 입력 대기 상태
        private Action currentLineFinishedCallback; // 현재 대사 완료 콜백 저장

        private void Awake()
        {
            _runner.AddCommandHandler<string, string, int>("RollDice", RollDice);
            _runner.AddCommandHandler<int>("NextDialogue", NextDialogue);
        }

#region system
        // Yarn Spinner에서 대사를 받아서 처리하는 메서드
        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            Debug.Log("대사 출력");
            // 1. 새 대사 오브젝트 생성
            GameObject lineObj = Instantiate(linePrefab, content);
            TextMeshProUGUI tmp = lineObj.GetComponentInChildren<TextMeshProUGUI>();

            // 2. 텍스트 설정
            tmp.text = dialogueLine.Text.Text;

            // 3. 텍스트 높이에 맞게 자동 조정
            AdjustTextHeight(lineObj, tmp);

            // 4. 버튼에 ContinueDialogue 연결 (버튼이 있다면)
            Button lineButton = lineObj.GetComponent<Button>();
            if (lineButton != null)
            {
                lineButton.onClick.RemoveAllListeners();
                lineButton.onClick.AddListener(ContinueDialogue);
            }

            // 5. 스크롤을 가장 밑으로 내리기
            StartCoroutine(ScrollToBottom());
            
            // 6. 키 입력 대기 상태로 설정 (자동으로 다음 대사로 넘어가지 않음)
            isWaitingForInput = true;
            currentLineFinishedCallback = onDialogueLineFinished;
        }

        // 다음 대사를 호출하는 함수 (키에 할당할 용도)
        public void ContinueDialogue()
        {
            if (isWaitingForInput)
            {
                isWaitingForInput = false;
                // 저장된 콜백을 호출하여 다음 대사로 진행
                currentLineFinishedCallback?.Invoke();
                currentLineFinishedCallback = null;
            }
        }

        // 옵션 표시 메서드
        public override void RunOptions(DialogueOption[] options, Action<int> onOptionSelected)
        {
            isShowingOptions = true;
            StartCoroutine(ShowOptions(options, onOptionSelected));
        }

        private IEnumerator ShowOptions(DialogueOption[] options, Action<int> onOptionSelected)
        {
            // 기존 옵션들 제거
            ClearOptions();

            // 각 옵션 생성
            for (int i = 0; i < options.Length; i++)
            {
                GameObject optionObj = Instantiate(optionPrefab, content);
                TextMeshProUGUI optionText = optionObj.GetComponentInChildren<TextMeshProUGUI>();
                Image optionBackground = optionObj.GetComponent<Image>();
                
                // 옵션 텍스트 설정
                optionText.text = options[i].Line.Text.Text;
                
                // 옵션 텍스트 높이에 맞게 이미지 박스 높이 조정
                AdjustOptionHeight(optionObj, optionText);
                
                // 옵션 인덱스 저장
                int optionIndex = i;
                
                // EventTrigger 추가
                EventTrigger eventTrigger = optionObj.GetComponent<EventTrigger>();
                if (eventTrigger == null)
                {
                    eventTrigger = optionObj.AddComponent<EventTrigger>();
                }
                
                // 마우스 진입 이벤트
                EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
                pointerEnter.eventID = EventTriggerType.PointerEnter;
                pointerEnter.callback.AddListener((data) => {
                    if (isShowingOptions)
                    {
                        optionBackground.color = optionHoverColor;
                    }
                });
                eventTrigger.triggers.Add(pointerEnter);
                
                // 마우스 나감 이벤트
                EventTrigger.Entry pointerExit = new EventTrigger.Entry();
                pointerExit.eventID = EventTriggerType.PointerExit;
                pointerExit.callback.AddListener((data) => {
                    if (isShowingOptions)
                    {
                        optionBackground.color = Color.clear;
                    }
                });
                eventTrigger.triggers.Add(pointerExit);
                
                // 클릭 이벤트
                Button optionButton = optionObj.GetComponent<Button>();
                if (optionButton != null)
                {
                    optionButton.onClick.AddListener(() => {
                        if (isShowingOptions)
                        {
                            SelectOption(optionIndex, onOptionSelected);
                        }
                    });
                }
                
                currentOptions.Add(optionObj);
                
                // 옵션 간 약간의 딜레이
                yield return new WaitForSeconds(0.1f);
            }
            
            // 모든 옵션 생성 후 스크롤을 가장 밑으로 내리기
            StartCoroutine(ScrollToBottom());
        }

        private void SelectOption(int optionIndex, Action<int> onOptionSelected)
        {
            isShowingOptions = false;
            
            // 선택된 옵션만 남기고 나머지 제거
            for (int i = 0; i < currentOptions.Count; i++)
            {
                if (i == optionIndex)
                {
                    // 선택된 옵션은 반투명 흰색으로 변경
                    Image selectedBackground = currentOptions[i].GetComponent<Image>();
                    if (selectedBackground != null)
                    {
                        selectedBackground.color = optionSelectedColor;
                    }
                    
                    // 더 이상 상호작용 불가
                    Button selectedButton = currentOptions[i].GetComponent<Button>();
                    if (selectedButton != null)
                    {
                        selectedButton.interactable = false;
                    }
                }
                else
                {
                    // 나머지 옵션들 제거
                    Destroy(currentOptions[i]);
                }
            }
            
            // 선택된 옵션만 남기고 리스트 정리
            GameObject selectedOption = currentOptions[optionIndex];
            currentOptions.Clear();
            currentOptions.Add(selectedOption);
            
            // 옵션 선택 콜백 호출
            onOptionSelected?.Invoke(optionIndex);
        }

        private void ClearOptions()
        {
            foreach (GameObject option in currentOptions)
            {
                if (option != null)
                {
                    Destroy(option);
                }
            }
            currentOptions.Clear();
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

        private void AdjustOptionHeight(GameObject optionObj, TextMeshProUGUI optionText)
        {
            // 텍스트의 실제 높이 계산
            Vector2 textSize = optionText.GetPreferredValues();
            
            // 이미지 박스의 현재 높이
            RectTransform imageRect = optionObj.GetComponent<RectTransform>();
            float currentHeight = imageRect.sizeDelta.y;
            
            // 텍스트 높이 + 5픽셀 여유가 현재 이미지 높이보다 크면 이미지 높이를 조정
            float targetHeight = textSize.y + 5f;
            if (targetHeight > currentHeight)
            {
                imageRect.sizeDelta = new Vector2(imageRect.sizeDelta.x, targetHeight);
            }
            
            // 레이아웃 업데이트를 강제로 실행
            LayoutRebuilder.ForceRebuildLayoutImmediate(imageRect);
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
        #endregion
        private void NextDialogue(int val)
        {
            _runner.Stop();
            _runner.StartDialogue($"Dollar_{val}");
        }

        private void RollDice(string type, string difficulty, int val)
        {
            if (val % 10 == 1)
            {
                _runner.Stop();
                _runner.StartDialogue($"Dollar_{val / 10}_{val % 10}_Success");
            }
            else
            {
                if (!System.Enum.TryParse(difficulty, ignoreCase: true, out Difficulty parsed))
                {
                    Debug.LogWarning($"Unknown difficulty '{difficulty}', defaulting to Normal");
                    parsed = Difficulty.Normal;
                }

                _trpgLine.StartRoll(type, parsed, val, resultKey =>
                {
                    _runner.StartDialogue(resultKey);
                });
            }
        }
    }
}