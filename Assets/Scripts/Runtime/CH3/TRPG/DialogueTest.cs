using UnityEngine;
using System.Collections.Generic;
using Runtime.ETC;

namespace Runtime.CH3.TRPG
{
    public class DialogueTest : MonoBehaviour
    {
        private List<Dictionary<string, object>> dialogueData;
        private int currentDialogueIndex = 0;
        private bool showingChoices = false;

        void Start()
        {
            // Resources 폴더에 CSV 파일이 있어야 함 (확장자 없이 이름만)
            dialogueData = CSVReader.Read("dialogue_test");
            Debug.Log("CSV 로드 완료. 총 " + dialogueData.Count + "개의 행.");
        }

        void Update()
        {
            // 스페이스바: 대사 진행
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                if (!showingChoices)
                {
                    ShowEnemyLine();
                    showingChoices = true;
                }
                else
                {
                    ShowChoices();
                }
            }

            // 숫자키로 선택
            if (showingChoices)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1)) SelectChoice(1);
                if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2)) SelectChoice(2);
                if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3)) SelectChoice(3);
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

            // 같은 DialogueID의 선택지 3개 찾기
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

            foreach (var row in dialogueData)
            {
                if ((int)row["DialogueID"] == dialogueId && (int)row["ChoiceID"] == choiceId)
                {
                    Debug.Log($"[성공 대사] {row["SuccessText"]}");
                    Debug.Log($"[성공 값] {row["SuccessValue"]}");
                    break;
                }
            }

            showingChoices = false;
            currentDialogueIndex += 3; // 다음 DialogueID로 넘어가기 (3개 선택지 건너뜀)
        }
    }
}