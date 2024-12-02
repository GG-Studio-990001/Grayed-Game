using Runtime.ETC;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Yarn.Unity;

namespace Runtime.CH2.Main
{
    public class TcgController : MonoBehaviour
    {
        private List<Dictionary<string, object>> _responses = new(); // 캐릭터 반응
        private List<Dictionary<string, object>> _scores = new(); // 호감도 점수

        private void Awake()
        {
            // CSV 데이터를 읽어오기
            _responses = CSVReader.Read("Tcg - Responses");
            _scores = CSVReader.Read("Tcg - Scores");

            Debug.Log("질문과 답변 및 점수 출력:");
            for (int questionIndex = 1; questionIndex < _responses[0].Count; questionIndex++) // 첫 열 제외
            {
                // 질문 가져오기 (responses의 header에서 질문 찾기)
                string question = _responses[0].Keys.ToArray()[questionIndex];

                Debug.Log($"질문: {question}");

                foreach (var responseRow in _responses)
                {
                    // 답변 가져오기 (responses의 첫 번째 열)
                    if (!responseRow.TryGetValue("", out var answerObj) || answerObj == null) continue;
                    string answer = answerObj.ToString();

                    // 반응 가져오기
                    string response = responseRow.ContainsKey(question) ? responseRow[question]?.ToString() : "반응 없음";

                    // 점수 가져오기
                    var scoreRow = _scores.Find(row => row[""]?.ToString() == answer);
                    string scoreValue = scoreRow?.ContainsKey(question) == true ? scoreRow[question]?.ToString() : "X";

                    // 점수 값 파싱
                    string score = (scoreValue == "X") ? "점수 없음" : scoreValue;

                    // 디버그 출력
                    Debug.Log($"답변: {answer}\n반응: {response}\n점수: {score}");
                }

                Debug.Log("------------");
            }
        }
    }
}
