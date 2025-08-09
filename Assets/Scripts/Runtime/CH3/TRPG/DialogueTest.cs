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
        [Header("=Dialogue=")]
        [SerializeField] private Transform content;               // Scroll View → Content 오브젝트
        [SerializeField] private GameObject linePrefab;           // TextLine 프리팹
        [Header("=Options=")]
        [SerializeField] private GameObject optionPrefab;         // 옵션 프리팹
        [SerializeField] private Color optionHoverColor = new(1f, 1f, 1f, 0.3f); // 마우스 오버 색상
        [SerializeField] private Color optionSelectedColor = new(1f, 1f, 1f, 0.5f); // 선택된 색상
        private List<GameObject> currentOptions = new();
        private bool isShowingOptions = false;

        // 
        private List<Dictionary<string, object>> dialogueData;
        private int currentDialogueIndex = 0;

        private int state = 0; // 0=대사 출력 대기, 1=선택지 출력 대기, 2=선택지 선택 대기
        private bool isDialogueEnded = false;

        private void Start()
        {
            dialogueData = CSVReader.Read("dialogue_test");
            Debug.Log("CSV 로드 완료. 총 " + dialogueData.Count + "개의 행.");
        }

        private void Update()
        {
            if (isDialogueEnded)
                return; // 대사 끝났으면 입력 무시

            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                if (state == 0)
                {
                    ShowEnemyLine();
                    state = 1;
                }
                else if (state == 1)
                {
                    ShowChoices();
                    state = 2;
                }
            }

            if (state == 2)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) SelectChoice(1);
                else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) SelectChoice(2);
                else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3)) SelectChoice(3);
            }
        }

        private void ShowEnemyLine()
        {
            string enemyLine = dialogueData[currentDialogueIndex]["EnemyLine"].ToString();
            Debug.Log($"[적 대사] {enemyLine}");

            // 1. 새 대사 오브젝트 생성
            GameObject lineObj = Instantiate(linePrefab, content);
            TextMeshProUGUI tmp = lineObj.GetComponentInChildren<TextMeshProUGUI>();

            // 2. 텍스트 설정
            tmp.text = enemyLine;

            // 3. 텍스트 높이에 맞게 자동 조정
            AdjustTextHeight(lineObj, tmp);
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

        private void ShowChoices()
        {
            //int dialogueId = (int)dialogueData[currentDialogueIndex]["DialogueID"];
            //Debug.Log("=== 선택지를 고르세요 ===");

            //foreach (var row in dialogueData)
            //{
            //    if ((int)row["DialogueID"] == dialogueId)
            //    {
            //        Debug.Log($"{row["ChoiceID"]}. {row["ChoiceText"]}");
            //    }
            //}
            StartCoroutine(nameof(ShowOptions));
        }

        private IEnumerator ShowOptions()
        {
            int dialogueId = (int)dialogueData[currentDialogueIndex]["DialogueID"];
            // int dialogueId = (int)dialogueData[currentDialogueIndex]["DialogueID"];
            // int choicesCount = 0;

            foreach (var row in dialogueData)
            {
                if ((int)row["DialogueID"] == dialogueId)
                {
                    GameObject optionObj = Instantiate(optionPrefab, content);
                    TextMeshProUGUI optionText = optionObj.GetComponentInChildren<TextMeshProUGUI>();
                    Image optionBackground = optionObj.GetComponent<Image>();

                    // 옵션 텍스트 설정
                    optionText.text = $"{row["ChoiceID"]}. {row["ChoiceText"]}";

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
                    Button optionButton = optionObj.GetComponent<Button>();
                    if (optionButton != null)
                    {
                        optionButton.onClick.AddListener(() =>
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

            // 옵션 선택 콜백 호출
            // SelectChoice(idx);
            // onOptionSelected?.Invoke(optionIndex);
        }

        private void SelectChoice(int choiceId)
        {
            int dialogueId = (int)dialogueData[currentDialogueIndex]["DialogueID"];
            int choicesCount = 0;

            foreach (var row in dialogueData)
            {
                if ((int)row["DialogueID"] == dialogueId)
                {
                    choicesCount++;
                    if ((int)row["ChoiceID"] == choiceId)
                    {
                        Debug.Log($"[성공 대사] {row["SuccessText"]}");
                        Debug.Log($"[성공 값] {row["SuccessValue"]}");
                    }
                }
            }

            currentDialogueIndex += choicesCount;

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
    }
}