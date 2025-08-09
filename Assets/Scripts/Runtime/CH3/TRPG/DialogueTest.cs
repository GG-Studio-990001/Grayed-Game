using UnityEngine;
using System.Collections.Generic;
using Runtime.ETC;

public class DialogueTest : MonoBehaviour
{
    private List<Dictionary<string, object>> dialogueData;
    private int currentDialogueIndex = 0;

    private int state = 0; // 0=대사 출력 대기, 1=선택지 출력 대기, 2=선택지 선택 대기
    private bool isDialogueEnded = false;

    void Start()
    {
        dialogueData = CSVReader.Read("dialogue_test");
        Debug.Log("CSV 로드 완료. 총 " + dialogueData.Count + "개의 행.");
    }

    void Update()
    {
        if (isDialogueEnded)
            return; // 대사 끝났으면 입력 무시

        if (Input.GetKeyDown(KeyCode.Space))
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
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectChoice(1);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) SelectChoice(2);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) SelectChoice(3);
        }
    }

    void ShowEnemyLine()
    {
        string enemyLine = dialogueData[currentDialogueIndex]["EnemyLine"].ToString();
        Debug.Log($"[적 대사] {enemyLine}");
    }

    void ShowChoices()
    {
        int dialogueId = (int)dialogueData[currentDialogueIndex]["DialogueID"];
        Debug.Log("=== 선택지를 고르세요 ===");

        foreach (var row in dialogueData)
        {
            if ((int)row["DialogueID"] == dialogueId)
            {
                Debug.Log($"{row["ChoiceID"]}. {row["ChoiceText"]}");
            }
        }
    }

    void SelectChoice(int choiceId)
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