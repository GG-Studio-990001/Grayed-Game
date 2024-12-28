using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Yarn.Unity;
using Runtime.ETC;
using DG.Tweening;

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
        [SerializeField] private GameObject _michael;
        [SerializeField] private CanvasGroup _michaelBubble; // 미카엘 버블
        [SerializeField] private TextMeshProUGUI _michaelBubbleTxt; // 미카엘 대화창 텍스트
        [SerializeField] private TextMeshProUGUI _scoreTxt; // 호감도 텍스트
        [SerializeField] private GameObject[] _cards = new GameObject[4]; // 답변 카드
        [SerializeField] private TextMeshProUGUI[] _cardsTxt = new TextMeshProUGUI[4]; // 답변 카드 텍스트
        [SerializeField] private GameObject _cardBack;
        [SerializeField] private CanvasGroup _cardsCanvasGroup;
        [Header("=ScoreBoard=")]
        [SerializeField] private GameObject _scoreBoard;
        [SerializeField] private Slider _scoreSlider;
        [SerializeField] private RectTransform _heart;
        private Vector3[] _cardInitialPositions; // 카드의 초기 위치를 저장
        private Vector3 _cardBackInitialPosition; // 카드 뒷면의 초기 위치
        private List<Dictionary<string, object>> _responses = new(); // 캐릭터 반응 파일
        private List<Dictionary<string, object>> _scores = new(); // 호감도 점수 파일
        private int _currentQuestionIndex = 0; // 현재 질문 인덱스
        private int _currentScore = 0; // 현재 호감도 점수
        private readonly List<int> _usedAnswers = new(); // 사용된 답변 인덱스 기록
        private int _scoreChange;
        public bool IsCardSelected { get; private set; }

        private void Start()
        {
            // 데이터 로드
            _responses = CSVReader.Read("Tcg - Responses");
            _scores = CSVReader.Read("Tcg - Scores");

            // 카드 초기 위치 저장
            _cardInitialPositions = new Vector3[_cards.Length];
            for (int i = 0; i < _cards.Length; i++)
            {
                _cardInitialPositions[i] = _cards[i].transform.localPosition;
            }

            if (_cardBack != null)
            {
                _cardBackInitialPosition = _cardBack.transform.localPosition;
            }
        }

        #region Dialogue
        public void StartTcg()
        {
            ResetCardPositions();
            ActiveCh2Ui(false);

            // _michael을 중앙으로 이동
            _michael.transform.DOLocalMove(new Vector3(38.9999886f, -70f, 0f), 1f).SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    ActiveTcgUi(true);
                    MoveCardsUp();
                });

            ShowQuestion();
        }

        private void EndTcg()
        {
            // 카드 페이드 아웃
            _cardsCanvasGroup.DOFade(0f, 1f);

            // _michael 오른쪽으로 이동
            _michael.transform.DOLocalMove(new Vector3(355f, -70f, 0f), 1f).SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    ActiveTcgUi(false);
                    ActiveCh2Ui(true);
                    AnswerDialogue();
                });
        }

        private void ActiveCh2Ui(bool active)
        {
            _ch2Ui.SetActive(active);
            _character.SetActive(active);
        }

        private void ActiveTcgUi(bool active)
        {
            if (active)
            {
                _michaelBubble.gameObject.SetActive(true);
                _michaelBubble.DOFade(1f, 0.5f);
            }
            _tcgObject.SetActive(active);
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

        public void ShowScore()
        {
            _scoreBoard.SetActive(true);

            // 새로운 점수 계산
            float newScore = Mathf.Clamp(_currentScore + _scoreChange, 0, 100);

            // 슬라이더 값을 1초 동안 변화
            _scoreSlider.DOValue(newScore / 100, 1f).SetEase(Ease.InOutQuad);

            // _currentScore 업데이트
            _currentScore = (int)newScore;
            Debug.Log($"현재 점수: {_currentScore}");

            HeartAnim();
        }

        private void HeartAnim()
        {
            // 심장 뛰는 애니메이션
            _heart.DOScale(1.2f, 0.3f) // 1.25배로 키움
                .SetEase(Ease.InOutBack)
                .SetLoops(8, LoopType.Yoyo);   // 2번 반복하며 원래 크기로 돌아감
        }

        public void HideScore()
        {
            _scoreBoard.SetActive(false);
        }

        private void ResetCardPositions()
        {
            IsCardSelected = false;

            _cardsCanvasGroup.alpha = 1f;

            for (int i = 0; i < _cards.Length; i++)
            {
                _cards[i].transform.localPosition = _cardInitialPositions[i];
            }

            if (_cardBack != null)
            {
                _cardBack.transform.localPosition = _cardBackInitialPosition;
            }
        }

        private void MoveCardsUp()
        {
            foreach (var card in _cards)
            {
                Vector3 targetPosition = card.transform.localPosition + new Vector3(0, 300, 0);
                card.transform.DOLocalMove(targetPosition, 1f).SetEase(Ease.OutQuad);
            }

            // 카드 뒷면도 Y축으로 300만큼 올라감
            if (_cardBack != null)
            {
                Vector3 targetPosition = _cardBack.transform.localPosition + new Vector3(0, 300, 0);
                _cardBack.transform.DOLocalMove(targetPosition, 1f).SetEase(Ease.OutQuad);
            }
        }

        private void MoveCardsDown()
        {
            foreach (var card in _cards)
            {
                // 카드 앞면의 현재 위치에서 Y축으로 300만큼 내려감
                Vector3 targetPosition = card.transform.localPosition - new Vector3(0, 300, 0);
                card.transform.DOLocalMove(targetPosition, 1f).SetEase(Ease.InQuad);
            }

            // 카드 뒷면도 Y축으로 300만큼 내려감
            if (_cardBack != null)
            {
                Vector3 targetPosition = _cardBack.transform.localPosition - new Vector3(0, 300, 0);
                _cardBack.transform.DOLocalMove(targetPosition, 1f).SetEase(Ease.InQuad);
            }
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
                //foreach (var card in _cards)
                //{
                //    card.SetActive(false);
                //}
                //Debug.Log("게임 종료");
                //EndTcg();
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
            if (IsCardSelected)
                return;

            IsCardSelected = true;

            // 선택된 답변 및 반응, 점수 가져오기
            string answer = _responses[answerIndex][""].ToString();
            string response = _responses[answerIndex][_responses[0].Keys.ToArray()[questionIndex + 1]].ToString();
            string scoreValue = _scores[answerIndex][_responses[0].Keys.ToArray()[questionIndex + 1]].ToString();

            // 점수 처리
            int.TryParse(scoreValue, out _scoreChange);
            _scoreTxt.text = $"+{_scoreChange}";

            // 사용된 답변 기록
            _usedAnswers.Add(answerIndex);

            Debug.Log($"선택된 답변: {answer}, 반응: {response}, 점수 변화: {_scoreChange}");

            // 미카엘 대화창 페이드 아웃
            _michaelBubble.DOFade(0f, 0.5f).OnComplete(() =>
            {
                _michaelBubble.gameObject.SetActive(false);
            });

            // 선택되지 않은 카드와 _cardBack 처리
            Sequence cardsSequence = DOTween.Sequence();
            for (int i = 0; i < _cards.Length; i++)
            {
                if (_cardsTxt[i].text != answer)
                {
                    // 선택되지 않은 카드는 아래로 이동
                    Vector3 downPosition = _cards[i].transform.localPosition - new Vector3(0, 300, 0);
                    cardsSequence.Join(_cards[i].transform.DOLocalMove(downPosition, 1f).SetEase(Ease.InQuad));
                }
            }

            if (_cardBack != null)
            {
                // _cardBack도 아래로 이동
                Vector3 cardBackDownPosition = _cardBack.transform.localPosition - new Vector3(0, 300, 0);
                cardsSequence.Join(_cardBack.transform.DOLocalMove(cardBackDownPosition, 1f).SetEase(Ease.InQuad));
            }
            
            // 선택된 카드 처리 (선택되지 않은 카드가 모두 내려간 후 시작)
            cardsSequence.OnComplete(() =>
            {
                for (int i = 0; i < _cards.Length; i++)
                {
                    if (_cardsTxt[i].text == answer)
                    {
                        // 선택된 카드의 x값만 0으로 설정하고 이동
                        Vector3 centerPosition = _cards[i].transform.localPosition;
                        centerPosition.x = 0; // x값만 0으로 변경
                        _cards[i].transform.DOLocalMove(centerPosition, 1f).SetEase(Ease.OutQuad)
                            .OnComplete(() =>
                            {
                                EndTcg();
                            });
                    }
                }
            });

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