using TMPro;
using System.Collections.Generic;
using UnityEngine;
using Runtime.ETC;
using System.Linq;

namespace Runtime.CH2.Main
{
    public class TcgController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _questionNumTxt; // 질문 번호 텍스트
        [SerializeField] private TextMeshProUGUI _michaelBubble; // 미카엘 대화창 텍스트
        [SerializeField] private TextMeshProUGUI _scoreTxt; // 호감도 텍스트
        [SerializeField] private GameObject[] _cards = new GameObject[4]; // 답변 카드
        [SerializeField] private TextMeshProUGUI[] _cardsTxt = new TextMeshProUGUI[4]; // 답변 카드 텍스트

        private List<Dictionary<string, object>> _responses = new(); // 캐릭터 반응 파일
        private List<Dictionary<string, object>> _scores = new(); // 호감도 점수 파일
        private int _currentQuestionIndex = 0; // 현재 질문 인덱스
        private int _heartScore = -100; // 현재 호감도 점수

        private void Start()
        {
            // 데이터 로드
            _responses = CSVReader.Read("Tcg - Responses");
            _scores = CSVReader.Read("Tcg - Scores");

            // 첫 번째 질문과 답변 초기화
            DisplayQuestionAndAnswers(_currentQuestionIndex);

            // 텍스트 초기 설정
            _questionNumTxt.text = $"{_currentQuestionIndex + 1}번 질문";
            _scoreTxt.text = $"호감도: {_heartScore}";
        }

        private void DisplayQuestionAndAnswers(int questionIndex)
        {
            // 질문 텍스트 설정
            string question = _responses[0].Keys.ToArray()[questionIndex + 1]; // 질문 가져오기
            _michaelBubble.text = question;

            // 답변 텍스트 설정
            for (int i = 0; i < _cards.Length; i++)
            {
                string answer = _responses[i][""].ToString(); // i번째 답변 가져오기
                _cardsTxt[i].text = answer;

                // 버튼 클릭 이벤트 설정
                int answerIndex = i; // 버튼 인덱스 캡처
                _cards[i].GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
                _cards[i].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnAnswerSelected(questionIndex, answerIndex));
            }
        }

        private void OnAnswerSelected(int questionIndex, int answerIndex)
        {
            // 선택된 답변 및 반응, 점수 가져오기
            string answer = _responses[answerIndex][""].ToString();
            string response = _responses[answerIndex][_responses[0].Keys.ToArray()[questionIndex + 1]].ToString();
            string scoreValue = _scores[answerIndex][_responses[0].Keys.ToArray()[questionIndex + 1]].ToString();

            // 점수 처리
            int.TryParse(scoreValue, out int scoreChange);

            // 미카엘 대화창 업데이트
            _michaelBubble.text = response;

            // 다른 답변 카드 비활성화
            for (int i = 0; i < _cards.Length; i++)
            {
                if (i != answerIndex)
                {
                    _cards[i].SetActive(false);
                }
            }

            // 점수 텍스트 업데이트
            _heartScore += scoreChange;
            _scoreTxt.text = $"호감도: {_heartScore}";

            // 디버그 출력
            Debug.Log($"선택된 답변: {answer}\n반응: {response} ({scoreChange})");
        }
    }
}
