using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Yarn.Unity;
using Runtime.ETC;

namespace Runtime.CH2.Main
{
    public class TcgController : MonoBehaviour
    {
        [Header("=Connect=")]
        [SerializeField] private DialogueRunner _runner;
        [SerializeField] private GameObject _character;
        [SerializeField] private GameObject _tcgObject;
        [Header("=TCG=")]
        [SerializeField] private GameObject _ch2Ui;
        [SerializeField] private GameObject _michaelBubble; // 미카엘 버블
        [SerializeField] private TextMeshProUGUI _michaelBubbleTxt; // 미카엘 대화창 텍스트
        [SerializeField] private TextMeshProUGUI _scoreTxt; // 호감도 텍스트
        [SerializeField] private GameObject[] _cards = new GameObject[4]; // 답변 카드
        [SerializeField] private TextMeshProUGUI[] _cardsTxt = new TextMeshProUGUI[4]; // 답변 카드 텍스트

        private List<Dictionary<string, object>> _responses = new(); // 캐릭터 반응 파일
        private List<Dictionary<string, object>> _scores = new(); // 호감도 점수 파일
        private int _currentQuestionIndex = 0; // 현재 질문 인덱스
        private int _currentScore = 0; // 현재 호감도 점수
        private readonly List<int> _usedAnswers = new(); // 사용된 답변 인덱스 기록

        private void Start()
        {
            // 데이터 로드
            _responses = CSVReader.Read("Tcg - Responses");
            _scores = CSVReader.Read("Tcg - Scores");
        }

        #region Dialogue
        public void StartTcg()
        {
            ActiveTcgUi(true);
            ShowQuestion();
        }

        private void EndTcg()
        {
            // 호감도 UI를 잠시 보여준 뒤 종료
            Invoke(nameof(DeactivateTcgUi), 3f);
        }

        private void ActiveTcgUi(bool active)
        {
            _ch2Ui.SetActive(!active);
            _character.SetActive(!active);
            _tcgObject.SetActive(active);
            _michaelBubble.SetActive(active);
        }

        private void DeactivateTcgUi()
        {
            ActiveTcgUi(false);
            AnswerDialogue();
        }

        private void AnswerDialogue()
        {
            _runner.StartDialogue("TCG_SetAnswer");
        }

        public void DialogueAfterTCG()
        {
            _runner.Stop();
            _runner.StartDialogue($"AfterTcg{_currentQuestionIndex}");
            _currentQuestionIndex++;
        }
        #endregion

        #region TCG
        private void ShowQuestion()
        {
            if (_currentQuestionIndex < _responses[0].Count - 1) // 남은 질문이 있으면
            {
                DisplayQuestionAndAnswers(_currentQuestionIndex);
            }
            else
            {
                // 질문이 끝났을 경우
                _michaelBubbleTxt.text = "(질문 끝)";
                foreach (var card in _cards)
                {
                    card.SetActive(false);
                }
                Debug.Log("게임 종료");
                EndTcg();
            }
        }

        private void DisplayQuestionAndAnswers(int questionIndex)
        {
            // 질문 텍스트 설정
            string question = _responses[0].Keys.ToArray()[questionIndex + 1];
            _michaelBubbleTxt.text = question;

            // 답변 카드 설정
            int answerIndex = 0;
            for (int i = 0; i < _cards.Length; i++)
            {
                while (_usedAnswers.Contains(answerIndex) && answerIndex < _responses.Count)
                {
                    answerIndex++;
                }

                if (answerIndex < _responses.Count)
                {
                    string answer = _responses[answerIndex][""].ToString();
                    _cardsTxt[i].text = answer;

                    // 버튼 클릭 이벤트 설정
                    int cardIndex = answerIndex;
                    _cards[i].SetActive(true);
                    var button = _cards[i].GetComponent<Button>();
                    button.interactable = true;
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => OnAnswerSelected(questionIndex, cardIndex));

                    answerIndex++;
                }
                else
                {
                    _cards[i].SetActive(false);
                }
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
            _currentScore = Mathf.Clamp(_currentScore + scoreChange, 0, int.MaxValue);
            _scoreTxt.text = $"+{scoreChange}";
            Debug.Log($"현재 점수: {_currentScore}");

            // 미카엘 대화창 비활성화 / TODO: 호감도 띄우기
            _michaelBubble.SetActive(false);

            // 답변 카드 비활성화
            for (int i = 0; i < _cards.Length; i++)
            {
                var button = _cards[i].GetComponent<Button>();
                button.interactable = false;

                if (_cardsTxt[i].text != answer)
                {
                    _cards[i].SetActive(false);
                }
            }

            // 사용된 답변 기록
            _usedAnswers.Add(answerIndex);

            Debug.Log($"선택된 답변: {answer}, 반응: {response}, 점수 변화: {scoreChange}");
            Invoke(nameof(EndTcg), 1f);
        }
        #endregion

        #region Yarn Functions
        [YarnFunction("GetAnswer")]
        public static string GetAnswer()
        {
            // TcgController 인스턴스 참조
            var instance = FindObjectOfType<TcgController>();
            if (instance != null && instance._usedAnswers.Count > 0)
            {
                int lastAnswerIndex = instance._usedAnswers.Last();
                return instance._responses[lastAnswerIndex][""].ToString();
            }
            return "";
        }

        [YarnFunction("GetResponse")]
        public static string GetResponse()
        {
            // TcgController 인스턴스 참조
            var instance = FindObjectOfType<TcgController>();
            if (instance == null)
            {
                Debug.LogError("TcgController를 찾을 수 없습니다.");
                return "(오류: 컨트롤러 없음)";
            }

            // 사용된 답변이 있는지 확인
            if (instance._usedAnswers.Count == 0)
            {
                Debug.LogWarning("사용된 답변이 없습니다. 답변이 선택되지 않았습니다.");
                return "(오류: 답변 없음)";
            }

            // 최근 선택된 답변 인덱스 가져오기
            int lastAnswerIndex = instance._usedAnswers.Last();

            // 현재 질문 인덱스가 유효한지 확인
            int questionIndex = instance._currentQuestionIndex; // `OnAnswerSelected`와 동일하게 -1 적용
            if (questionIndex < 0)
            {
                Debug.LogWarning($"현재 질문 인덱스가 0보다 작습니다. questionIndex: {questionIndex}");
                return "(오류: 질문 인덱스 초과)";
            }

            // 현재 질문 키 가져오기
            string[] keys = instance._responses[0].Keys.ToArray();
            if (questionIndex >= keys.Length)
            {
                Debug.LogWarning($"_currentQuestionIndex({questionIndex})가 키 배열 범위를 초과했습니다.");
                return "(오류: 질문 키 초과)";
            }
            string currentQuestionKey = keys[questionIndex + 1]; // OnAnswerSelected와 동일한 오프셋(+1) 적용

            // 응답 데이터에서 해당 키 확인
            if (!instance._responses[lastAnswerIndex].ContainsKey(currentQuestionKey))
            {
                Debug.LogWarning($"키 '{currentQuestionKey}'가 응답 데이터에 존재하지 않습니다.");
                return "(오류: 응답 데이터 없음)";
            }

            // 정상적인 반응 반환
            string response = instance._responses[lastAnswerIndex][currentQuestionKey]?.ToString();
            Debug.Log($"반환된 반응: {response}");
            return response;
        }
    }
    #endregion
}