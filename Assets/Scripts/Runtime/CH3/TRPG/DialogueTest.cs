using UnityEngine;
using System.Collections.Generic;
using Runtime.ETC;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace Runtime.CH3.TRPG
{
    public class DialogueTest : MonoBehaviour
    {
        [Header("=Script=")]
        [SerializeField] private TrpgDice _trpgDice;

        [Header("=Dialogue=")]
        [SerializeField] private Transform content;               // Scroll View → Content 오브젝트
        [SerializeField] private GameObject linePrefab;           // TextLine 프리팹
        [SerializeField] private ScrollRect scrollRect;           // Scroll Rect 컴포넌트
        
        [Header("=Options=")]
        [SerializeField] private GameObject optionPrefab;         // 옵션 프리팹
        [SerializeField] private Color optionHoverColor = new(1f, 1f, 1f, 0.3f); // 마우스 오버 색상
        [SerializeField] private Color optionSelectedColor = new(1f, 1f, 1f, 0.5f); // 선택된 색상
        
        private List<GameObject> currentOptions = new();
        private bool isShowingOptions = false;

        private List<Dictionary<string, object>> dialogueData;
        private int currentDialogueIndex = 0;

        private int state = 0; // 0=대사 출력 대기, 1=선택지 출력 대기, 2=선택지 선택 대기
        private bool isDialogueEnded = false;

        private void Start()
        {
            dialogueData = CSVReader.Read("dialogue_test");
            Debug.Log("CSV 로드 완료. 총 " + dialogueData.Count + "개의 행.");

            ContinueDialogue();
        }

        public void ContinueDialogue()
        {
            if (isDialogueEnded || state == 2)
                return; // 대사 끝났거나 선택지 선택 대기 중이면 입력 무시

            if (state == 0)
            {
                ShowEnemyLine();
                state = 1;
            }
            else if (state == 1)
            {
                ShowOptions();
                state = 2;
            }
        }

        #region 대사 (적&반응)
        private void ShowEnemyLine()
        {
            string enemyLine = dialogueData[currentDialogueIndex]["EnemyLine"].ToString();
            // Debug.Log($"[적 대사] {enemyLine}");
            
            ShowLine(enemyLine);
        }

        private void ShowLine(string str)
        {
            // 1. 새 대사 오브젝트 생성
            GameObject lineObj = Instantiate(linePrefab, content);
            TextMeshProUGUI tmp = lineObj.GetComponentInChildren<TextMeshProUGUI>();

            // 2. 텍스트 설정
            tmp.text = str;

            // 3. 텍스트 높이에 맞게 자동 조정
            AdjustTextHeight(lineObj, tmp);

            StartCoroutine(ScrollToBottom());

            // 스페이스가 아닌 클릭으로도 Dialogue 진행할 수 있도록
            Button lineBtn = lineObj.GetComponent<Button>();
            if (lineBtn != null)
            {
                lineBtn.onClick.AddListener(() =>
                {
                    ContinueDialogue();
                });
            }
        }

        private void AdjustTextHeight(GameObject lineObj, TextMeshProUGUI tmp)
        {
            // ContentSizeFitter가 없으면 추가
            ContentSizeFitter fitter = lineObj.GetComponent<ContentSizeFitter>() ?? lineObj.AddComponent<ContentSizeFitter>();

            // 세로 크기만 자동 조정 (가로는 고정)
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            // 레이아웃 업데이트를 강제로 실행
            LayoutRebuilder.ForceRebuildLayoutImmediate(lineObj.GetComponent<RectTransform>());
        }

        private void ReadyToRoll(int choiceId)
        {
            int dialogueId = int.Parse(dialogueData[currentDialogueIndex]["DialogueID"].ToString());
            int choicesCount = 0;
            int targetRowIdx = -1;
            Stat stat = Stat.NONE;
            Difficulty difficulty = Difficulty.NONE;

            // 선택지 찾기
            for (int i = 0; i < dialogueData.Count; i++)
            {
                var row = dialogueData[i];

                if (int.Parse(row["DialogueID"].ToString()) == dialogueId)
                {
                    choicesCount++;

                    if (int.Parse(row["ChoiceID"].ToString()) == choiceId)
                    {
                        targetRowIdx = i;
                        stat = Enum.Parse<Stat>(row["Stat"].ToString());
                        difficulty = Enum.Parse<Difficulty>(row["Difficulty"].ToString());
                    }
                }
            }

            if (targetRowIdx == -1)
            {
                Debug.LogError("선택지를 찾을 수 없습니다.");
                return;
            }

            currentDialogueIndex += choicesCount;

            // 주사위 판정 시작
            _trpgDice.StartRoll(stat, difficulty, resultVal =>
            {
                // 판정 끝난 뒤 대사 처리
                ShowChoiceResult(targetRowIdx, resultVal);
            });
        }

        private void ShowChoiceResult(int rowIdx, ResultVal result)
        {
            var row = dialogueData[rowIdx];
            ShowLine($"{row[result.ToString()+ "Text"]}");

            if (currentDialogueIndex >= dialogueData.Count)
            {
                Debug.Log("대사가 모두 끝났습니다.");
                isDialogueEnded = true; // 더 이상 진행 불가 표시
            }
            else
            {
                state = 0; // 다음 대사 대기 상태로 변경
            }
        }
        #endregion

        #region 선택지
        private void ShowOptions()
        {
            isShowingOptions = true;
            StartCoroutine(nameof(ShowOptionsCoroutine));
        }

        private IEnumerator ShowOptionsCoroutine()
        {
            int dialogueId = (int)dialogueData[currentDialogueIndex]["DialogueID"];

            foreach (var row in dialogueData)
            {
                if ((int)row["DialogueID"] == dialogueId)
                {
                    GameObject optionObj = Instantiate(optionPrefab, content);
                    TextMeshProUGUI optionText = optionObj.GetComponentInChildren<TextMeshProUGUI>();
                    Image optionBackground = optionObj.GetComponent<Image>();

                    // 옵션 텍스트 설정
                    string statStr = (string)row["Stat"] == "NONE" ? "" : $"[{row["StatKor"]}";
                    string DifficultyStr = (string)row["Difficulty"] == "NONE" ? "" : $"/{row["DifficultyKor"]}]";
                    optionText.text = $"{row["ChoiceID"]}. {statStr}{DifficultyStr} {row["ChoiceText"]}";

                    // 옵션 텍스트 높이에 맞게 이미지 박스 높이 조정
                    AdjustOptionHeight(optionObj, optionText);

                    // 옵션 인덱스 저장
                    int optionIndex = (int)row["ChoiceID"] - 1;

                    // EventTrigger 추가
                    EventTrigger eventTrigger = optionObj.GetComponent<EventTrigger>();
                    if (eventTrigger == null)
                    {
                        eventTrigger = optionObj.AddComponent<EventTrigger>();
                    }

                    // 마우스 진입 이벤트
                    EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
                    pointerEnter.eventID = EventTriggerType.PointerEnter;
                    pointerEnter.callback.AddListener((data) =>
                    {
                        if (isShowingOptions)
                        {
                            optionBackground.color = optionHoverColor;
                        }
                    });
                    eventTrigger.triggers.Add(pointerEnter);

                    // 마우스 나감 이벤트
                    EventTrigger.Entry pointerExit = new EventTrigger.Entry();
                    pointerExit.eventID = EventTriggerType.PointerExit;
                    pointerExit.callback.AddListener((data) =>
                    {
                        if (isShowingOptions)
                        {
                            optionBackground.color = Color.clear;
                        }
                    });
                    eventTrigger.triggers.Add(pointerExit);

                    // 클릭 이벤트
                    Button optionBtn = optionObj.GetComponent<Button>();
                    if (optionBtn != null)
                    {
                        optionBtn.onClick.AddListener(() =>
                        {
                            if (isShowingOptions)
                            {
                                SelectOption(optionIndex);
                            }
                        });
                    }

                    currentOptions.Add(optionObj);

                    // 옵션 간 약간의 딜레이
                    yield return new WaitForSeconds(0.1f);
                }

                StartCoroutine(ScrollToBottom());
            }
        }

        private void SelectOption(int idx)
        {
            isShowingOptions = false;

            // 선택된 옵션을 복제해서 보관
            GameObject selectedOption = currentOptions[idx];
            GameObject selectedOptionCopy = Instantiate(selectedOption, content);

            // 선택된 옵션은 반투명 흰색으로 변경
            Image selectedBackground = selectedOptionCopy.GetComponent<Image>();
            if (selectedBackground != null)
            {
                selectedBackground.color = optionSelectedColor;
            }

            // 더 이상 상호작용 불가
            Button selectedButton = selectedOptionCopy.GetComponent<Button>();
            if (selectedButton != null)
            {
                selectedButton.interactable = false;
            }

            // EventTrigger 제거 (더 이상 상호작용하지 않도록)
            EventTrigger eventTrigger = selectedOptionCopy.GetComponent<EventTrigger>();
            if (eventTrigger != null)
            {
                Destroy(eventTrigger);
            }

            // 현재 옵션들 모두 제거
            foreach (GameObject option in currentOptions)
            {
                if (option != null)
                {
                    Destroy(option);
                }
            }
            currentOptions.Clear();

            // 옵션 선택 호출
            ReadyToRoll(idx + 1);
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
        #endregion

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
    }
}