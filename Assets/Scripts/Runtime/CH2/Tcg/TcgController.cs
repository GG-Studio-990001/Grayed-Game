using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Yarn.Unity;
using Runtime.ETC;
using DG.Tweening;

namespace Runtime.CH2.Tcg
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
        [SerializeField] private TcgCard[] _tcgCards = new TcgCard[4]; // 답변 카드 속성
        [SerializeField] private GameObject _cardBack;
        [SerializeField] private GameObject _cardBlock;
        [SerializeField] private CanvasGroup _cardsCanvasGroup;
        [Header("=ScoreBoard=")]
        [SerializeField] private GameObject _scoreBoard;
        [SerializeField] private Slider _scoreSlider;
        [SerializeField] private RectTransform _heart;
        private Vector3 _cardBackInitialPosition; // 카드 뒷면의 초기 위치
        private List<Dictionary<string, object>> _responses = new(); // 캐릭터 반응 파일
        private List<Dictionary<string, object>> _scores = new(); // 호감도 점수 파일
        private int _currentQuestionIndex = 0; // 현재 질문 인덱스
        private int _currentScore = 0; // 현재 호감도 점수
        private List<int> _usedAnswers = new(); // 사용된 답변 인덱스 기록
        private int _scoreChange;
        public bool IsCardSelected { get; private set; }

        private void Start()
        {
            // 데이터 로드
            _currentQuestionIndex = Managers.Data.CH2.TcgNum;
            _currentScore = Managers.Data.CH2.TcgScore;
            _usedAnswers = Managers.Data.CH2.UsedTcgAnswers;

            _scoreSlider.value = (float)_currentScore / 100;

            // CSV 읽어오기
            _responses = CSVReader.Read("Tcg - Responses");
            _scores = CSVReader.Read("Tcg - Scores");

            // 카드뒷면 초기 위치 저장
            if (_cardBack != null)
            {
                _cardBackInitialPosition = _cardBack.transform.localPosition;
            }
        }

        #region Dialogue
        public void StartLastTcg()
        {
            ActiveCh2Ui(false);

            // _michael을 중앙으로 이동
            _michael.transform.DOLocalMove(new Vector3(39f, -70f, 0f), 1f).SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    ActiveTcgUi(true);
                });

            ShowQuestion();
        }

        public void StartTcg()
        {
            ResetAndArrangeCards();
            ActiveCh2Ui(false);

            // _michael을 중앙으로 이동
            _michael.transform.DOLocalMove(new Vector3(39f, -70f, 0f), 1f).SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    ActiveTcgUi(true);
                    MoveCardsUp();
                });

            ShowQuestion();
        }

        private void TcgFinish()
        {
            _michaelBubble.DOFade(0f, 0.5f).OnComplete(() =>
            {
                _michaelBubble.gameObject.SetActive(false);
            });

            // _michael 오른쪽으로 이동
            _michael.transform.DOLocalMove(new Vector3(355f, -70f, 0f), 1f).SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    ActiveTcgUi(false);
                    ActiveCh2Ui(true);
                    DialogueAfterTCG();
                });
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
            Managers.Data.SaveGame();
        }

        public void DialogueAfterTCG()
        {
            _runner.Stop();
            _runner.StartDialogue($"AfterTcg{_currentQuestionIndex}");
            _currentQuestionIndex++;
            Managers.Data.CH2.TcgNum = _currentQuestionIndex;
        }

        public void ShowScore()
        {
            _scoreBoard.SetActive(true);

            Debug.Log($"{_currentScore} {_scoreChange}");

            // 새로운 점수 계산
            float newScore = Mathf.Clamp(_currentScore + _scoreChange, 0, 100);

            // 슬라이더 값을 1초 동안 변화
            _scoreSlider.DOValue(newScore / 100, 1f).SetEase(Ease.InOutQuad);

            // _currentScore 업데이트
            _currentScore = (int)newScore;
            Managers.Data.CH2.TcgScore = _currentScore;
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

        private void ResetAndArrangeCards()
        {
            IsCardSelected = false;

            _cardsCanvasGroup.alpha = 1f;

            int cardCount = _cards.Length;

            // 카드뒷면 초기 위치 리셋
            if (_cardBack != null)
            {
                _cardBack.transform.localPosition = _cardBackInitialPosition;
                if (_currentQuestionIndex >= 4)
                    _cardBack.SetActive(false);
            }

            if (_currentQuestionIndex == 0)
            {
                // 모든 카드를 카드 덱에 위치
                for (int i = 0; i < cardCount; i++)
                {
                    _cards[i].transform.localPosition = _cardBack.transform.localPosition;  // 위치 설정
                    _cards[i].transform.localRotation = _cardBack.transform.localRotation;  // 회전 설정
                    _cards[i].transform.localScale = _cardBack.transform.localScale;  // 크기 설정

                    _tcgCards[i].SetCardBack();
                }
                return;
            }

            // 카드앞면 배치의 기본 설정 (팬 형태로 배치)
            float[] xPositions = new float[] { -234f, -80f, 80f, 234f }; // X 좌표
            float[] yPositions = new float[] { -521f, -496f, -496f, -521f }; // Y 좌표
            float[] zRotations = new float[] { 15f, 5f, -5f, -15f }; // Z축 각도

            if (_currentQuestionIndex == 4)
            {
                // 3장
                xPositions = new float[] { -160f, 0f, 160f, 0f };
                yPositions = new float[] { -511f, -496f, -511f, 0f };
                zRotations = new float[] { 10f, 0f, -10f, 0f };
            }
            else if (_currentQuestionIndex == 5)
            {
                // 2장
                xPositions = new float[] { -80f, 80f, 0f, 0f };
                yPositions = new float[] { -496f, -496f, 0f, 0f };
                zRotations = new float[] { 5f, -5f, 0f, 0f };
            }
            else if (_currentQuestionIndex == 6)
            {
                // 1장
                xPositions = new float[] { 0f, 0f, 0f, 0f };
                yPositions = new float[] { -496f, 0f, 0f, 0f };
                zRotations = new float[] { 0f, 0f, 0f, 0f };
            }

            for (int i = 0; i < cardCount; i++)
            {
                // 카드의 위치 및 회전 설정
                _cards[i].transform.localPosition = new(xPositions[i], yPositions[i], 0f);
                _cards[i].transform.localRotation = Quaternion.Euler(0f, 0f, zRotations[i]);
                _cards[i].transform.localScale = Vector3.one;

                _tcgCards[i].SetCardFront();
            }

            if (_currentQuestionIndex < 4)
            {
                // 카드 1장만 날아올 경우
                // 마지막 카드만 위치, 회전, 크기 설정
                Transform lastCard = _cards[cardCount - 1].transform;

                lastCard.localPosition = _cardBack.transform.localPosition;  // 위치 설정
                lastCard.localRotation = _cardBack.transform.localRotation;  // 회전 설정
                lastCard.localScale = _cardBack.transform.localScale;  // 크기 설정
            }
        }

        private void MoveCardsUp()
        {
            if (_currentQuestionIndex < 4)
                _cardBlock.SetActive(true);

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

            if (_currentQuestionIndex == 0)
                Invoke(nameof(MoveAllCardsFromDeck), 1f);
            else if (_currentQuestionIndex < 4)
                Invoke(nameof(MoveLastCardFromDeck), 1f);
        }

        private void MoveLastCardFromDeck()
        {
            // 마지막 카드 (가장 오른쪽에 위치할 카드)를 덱에서 목표 위치로 이동
            Transform lastCard = _cards[_cards.Length - 1].transform;

            lastCard.DOLocalMove(new Vector3(234f, -521f + 300f, 0f), 1f).SetEase(Ease.OutQuad);  // 위치 이동
            lastCard.DOScale(Vector3.one, 1f).SetEase(Ease.OutQuad);  // 크기 변화 (0.5 -> 1.0)
            lastCard.DOLocalRotate(new Vector3(0f, 0f, -15f), 1f).SetEase(Ease.OutQuad)          // 회전 (z값 변화)
                .OnComplete(() => _cardBlock.SetActive(false));  // 애니메이션 종료 시 _cardBlock 비활성화

            DOVirtual.DelayedCall(0.1f, () =>
            {
                _tcgCards[_cards.Length - 1].SetCardFront();
            });
        }

        private void MoveAllCardsFromDeck()
        {
            float delayBetweenCards = 0.3f; // 각 카드 간의 딜레이
            float[] xPositions = { -234f, -80f, 80f, 234f }; // X 좌표
            float[] yPositions = { -221f, -196f, -196f, -221f }; // Y 좌표
            float[] zRotations = { 15f, 5f, -5f, -15f }; // Z축 각도

            Sequence sequence = DOTween.Sequence(); // DOTween 시퀀스 생성

            for (int i = 0; i < _cards.Length; i++)
            {
                int index = i; // 클로저 문제 방지
                sequence.AppendInterval(delayBetweenCards * i)
                    .AppendCallback(() =>
                    {
                        // 카드의 목표 위치 및 회전 설정
                        Vector3 targetPosition = new(xPositions[index], yPositions[index], 0f);
                        Quaternion targetRotation = Quaternion.Euler(0f, 0f, zRotations[index]);

                        // 애니메이션 실행
                        _cards[index].transform.DOLocalMove(targetPosition, 0.5f).SetEase(Ease.OutQuad);
                        _cards[index].transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutQuad);
                        _cards[index].transform.DOLocalRotate(targetRotation.eulerAngles, 0.5f).SetEase(Ease.OutQuad);

                        DOVirtual.DelayedCall(0.1f, () =>
                        {
                            _tcgCards[index].SetCardFront();
                        });
                    });
            }

            // 모든 애니메이션이 끝난 후 _cardBlock 비활성화
            sequence.OnComplete(() => _cardBlock.SetActive(false));
            sequence.Play();
        }
        #endregion

        #region TCG
        private void ShowQuestion()
        {
            if (_currentQuestionIndex == _responses[0].Count - 1) // 마지막 질문
            {
                _michaelBubbleTxt.text = "소중한 친구를 구하지 못한 바보 같은 나한테도...<br> 아직 기회가 있을까?";
                Invoke(nameof(TcgFinish), 3f);
            }
            else
            {
                DisplayQuestionAndAnswers(_currentQuestionIndex);
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
                    _tcgCards[i].SetCardIndex(answerIndex);

                    // 버튼 클릭 이벤트 설정
                    int cardIndex = answerIndex;
                    _cards[i].gameObject.SetActive(true);
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
            _scoreTxt.text = $"{(_scoreChange >= 0 ? "+" : "")}{_scoreChange}";

            // 사용된 답변 기록
            _usedAnswers.Add(answerIndex);
            Managers.Data.CH2.UsedTcgAnswers = _usedAnswers;

            Debug.Log($"선택된 답변: {answer}, 반응: {response}, 점수 변화: {_scoreChange}");

            // 미카엘 대화창 페이드 아웃
            _michaelBubble.DOFade(0f, 0.5f).OnComplete(() =>
            {
                _michaelBubble.gameObject.SetActive(false);
            });

            // CardsMoveDown
            // 선택되지 않은 카드와 _cardBack 처리
            Sequence cardsSequence = DOTween.Sequence();
            for (int i = 0; i < _cards.Length; i++)
            {
                if (_tcgCards[i].Index != answerIndex)
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
                    if (_tcgCards[i].Index == answerIndex)
                    {
                        // 선택된 카드의 x값만 0으로 설정하고 이동
                        Vector3 centerPosition = _cards[i].transform.localPosition;
                        centerPosition.x = 0; // x값만 0으로 변경

                        Sequence moveAndRotateSequence = DOTween.Sequence();

                        // 위치 이동
                        moveAndRotateSequence.Append(_cards[i].transform.DOLocalMove(centerPosition, 1f).SetEase(Ease.OutQuad));

                        // Z각도 0으로 회전
                        moveAndRotateSequence.Join(_cards[i].transform.DOLocalRotate(Vector3.zero, 1f, RotateMode.Fast).SetEase(Ease.OutQuad));

                        // 완료 시 EndTcg 호출
                        moveAndRotateSequence.OnComplete(() =>
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