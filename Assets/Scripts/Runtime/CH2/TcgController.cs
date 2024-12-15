using TMPro;
using System.Collections.Generic;
using UnityEngine;
using Runtime.ETC;
using System.Linq;
using Yarn.Unity;

namespace Runtime.CH2.Main
{
    public class TcgController : MonoBehaviour
    {
        [Header("=Connect=")]
        [SerializeField] private DialogueRunner _runner;
        [SerializeField] private GameObject _character;
        [SerializeField] private GameObject _tcgObject;
        [Header("=TCG=")]
        [SerializeField] private TextMeshProUGUI _questionNumTxt; // 질문 번호 텍스트
        [SerializeField] private TextMeshProUGUI _michaelBubble; // 미카엘 대화창 텍스트
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
            // TODO: 호감도 UI 적용 (3초? 보여주기)

            ActiveTcgUi(false);
            StartNextDialogue();
        }

        private void ActiveTcgUi(bool active)
        {
            _character.SetActive(!active);
            _tcgObject.SetActive(active);
        }

        private void StartNextDialogue()
        {
            switch(_currentQuestionIndex)
            {
                case 0:
                    _runner.StartDialogue("Turn2_S_1");
                    break;
                case 1:
                    _runner.StartDialogue("Turn2_S_2");
                    break;
            }

            // 질문 인덱스 증가 (여기서 세이브?)
            _currentQuestionIndex++;
        }
        #endregion

        #region TCG
        private void ShowQuestion()
        {
            // 질문 표시
            if (_currentQuestionIndex < _responses[0].Count - 1) // 질문이 남아있으면
            {
                UpdateQuestionIndexUI();
                DisplayQuestionAndAnswers(_currentQuestionIndex);
            }
            else
            {
                _michaelBubble.text = "(질문 끝)";

                // 모든 카드 비활성화
                foreach (var card in _cards)
                {
                    card.SetActive(false);
                }

                Debug.Log("게임 종료");
            }
        }

        private void DisplayQuestionAndAnswers(int questionIndex)
        {
            // 질문 텍스트 설정
            string question = _responses[0].Keys.ToArray()[questionIndex + 1]; // 질문 가져오기
            _michaelBubble.text = question;

            // 답변 텍스트 설정
            int answerIndex = 0; // 답변 인덱스 초기화
            for (int i = 0; i < _cards.Length; i++)
            {
                // 사용되지 않은 답변만 채우기
                while (_usedAnswers.Contains(answerIndex) && answerIndex < _responses.Count)
                {
                    answerIndex++;
                }

                // 유효한 답변만 설정
                if (answerIndex < _responses.Count)
                {
                    string answer = _responses[answerIndex][""].ToString(); // 답변 가져오기
                    _cardsTxt[i].text = answer;

                    // 버튼 클릭 이벤트 설정
                    int cardIndex = answerIndex; // 현재 카드의 실제 인덱스 저장
                    _cards[i].SetActive(true); // 답변 카드 활성화
                    _cards[i].GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
                    _cards[i].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnAnswerSelected(questionIndex, cardIndex));

                    answerIndex++;
                }
                else
                {
                    _cards[i].SetActive(false); // 유효하지 않은 카드 비활성화
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

            // 미카엘 대화창 업데이트
            _michaelBubble.text = response;

            // 답변 카드 비활성화
            for (int i = 0; i < _cards.Length; i++)
            {
                if (_cardsTxt[i].text != answer)
                {
                    _cards[i].SetActive(false);
                }
            }

            // 사용된 답변 기록
            _usedAnswers.Add(answerIndex);

            // 점수 텍스트 업데이트
            _currentScore = Mathf.Clamp(_currentScore + scoreChange, 0, int.MaxValue);
            UpdateScoreUI();

            // 디버그 출력
            Debug.Log($"선택된 답변: {answer}\n반응: {response} ({scoreChange})");

            Invoke(nameof(EndTcg), 3f);
        }

        private void UpdateScoreUI()
        {
            _scoreTxt.text = $"호감도: {_currentScore}";
        }

        private void UpdateQuestionIndexUI()
        {
            _questionNumTxt.text = $"{_currentQuestionIndex + 1}번 질문";
        }
        #endregion
    }
}
