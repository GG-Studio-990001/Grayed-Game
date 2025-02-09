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
        [SerializeField] private CardFlipper[] _tcgCards = new CardFlipper[4]; // 답변 카드 속성
        [SerializeField] private GameObject _cardBack;
        [SerializeField] private GameObject _cardBlock;
        [SerializeField] private CanvasGroup _cardsCanvasGroup;
        [Header("=ScoreBoard=")]
        [SerializeField] private GameObject _scoreBoard;
        [SerializeField] private Slider _scoreSlider;
        [SerializeField] private RectTransform _heart;
        [Header("=Else=")]
        private Vector3 _michaleCenter = new(39f, -70f, 0f);
        private Vector3 _michaleRight = new(355f, -70f, 0f);
        private Vector3 _cardBackInitialPosition; // 카드 뒷면의 초기 위치
        private List<Dictionary<string, object>> _responses = new(); // 캐릭터 반응 파일
        private List<Dictionary<string, object>> _scores = new(); // 호감도 점수 파일
        private int _currentQuestionIndex = 0; // 현재 질문 인덱스
        private int _currentScore = 0; // 현재 호감도 점수
        private List<int> _usedAnswers = new(); // 사용된 답변 인덱스 기록
        private int _scoreChange;
        public bool IsCardSelected { get; private set; }
        private bool ShouldUseLastCard => _currentQuestionIndex < 4;
        private static readonly Dictionary<int, (float[], float[], float[])> _cardLayouts = new()
        {
            { 4, (new float[] { -234f, -80f, 80f, 234f }, new float[] { -521f, -496f, -496f, -521f }, new float[] { 15f, 5f, -5f, -15f }) },
            { 3, (new float[] { -160f, 0f, 160f }, new float[] { -511f, -496f, -511f }, new float[] { 10f, 0f, -10f }) },
            { 2, (new float[] { -80f, 80f }, new float[] { -496f, -496f }, new float[] { 5f, -5f }) },
            { 1, (new float[] { 0f }, new float[] { -496f }, new float[] { 0f }) }
        };

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
        private void MoveMichael(Vector3 targetPosition, TweenCallback onComplete)
        {
            _michael.transform.DOLocalMove(targetPosition, 1f)
                .SetEase(Ease.InOutQuad)
                .OnComplete(onComplete);
        }

        public void StartTcg()
        {
            StartTcgInternal(false);
        }

        public void StartLastTcg()
        {
            StartTcgInternal(true);
        }

        private void StartTcgInternal(bool isLast)
        {
            if (!isLast) ResetAndArrangeCards();
            ActiveCh2Ui(false);

            MoveMichael(_michaleCenter, () =>
            {
                ActiveTcgUi(true);
                if (!isLast) MoveCardsUp();
            });

            ShowQuestion();
        }


        private void TcgFinish()
        {
            _michaelBubble.DOFade(0f, 0.5f).OnComplete(() =>
            {
                _michaelBubble.gameObject.SetActive(false);
            });

            // _michael 오른쪽
            MoveMichael(_michaleRight, () =>
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
            MoveMichael(_michaleRight, () =>
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
            _heart.DOScale(1.2f, 0.3f) // 1.2배로 키움
                .SetEase(Ease.InOutBack)
                .SetLoops(8, LoopType.Yoyo); // 2번 반복하며 원래 크기로 돌아감
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

            ResetCardBack();

            if (_currentQuestionIndex == 0)
            {
                PositionAllCardsToDeck();
                return;
            }

            ArrangeCardPositions(cardCount);
        }

        private void ResetCardBack()
        {
            if (_cardBack != null)
            {
                _cardBack.transform.localPosition = _cardBackInitialPosition;
                if (_currentQuestionIndex >= 4)
                    _cardBack.SetActive(false);
            }
        }

        private void PositionAllCardsToDeck()
        {
            for (int i = 0; i < _cards.Length; i++)
            {
                _cards[i].transform.SetLocalPositionAndRotation(_cardBack.transform.localPosition, _cardBack.transform.localRotation);
                _cards[i].transform.localScale = _cardBack.transform.localScale;
                _tcgCards[i].SetCardBack();
            }
        }

        // 공통적으로 카드 이동을 처리하는 함수 (애니메이션 적용 가능)
        private void MoveCardToPosition(Transform card, Vector3 targetPosition, Vector3 targetRotation, float duration, TweenCallback onComplete = null)
        {
            card.DOLocalMove(targetPosition, duration).SetEase(Ease.OutQuad);
            card.DOLocalRotate(targetRotation, duration).SetEase(Ease.OutQuad);
            card.DOScale(Vector3.one, duration).SetEase(Ease.OutQuad).OnComplete(onComplete);
        }

        private void ArrangeCardPositions(int cardCount)
        {
            if (!_cardLayouts.TryGetValue(cardCount, out var layout))
            {
                Debug.LogError($"올바른 카드 배치 데이터를 찾을 수 없습니다. cardCount: {cardCount}");
                return;
            }

            float[] xPositions = layout.Item1;
            float[] yPositions = layout.Item2;
            float[] zRotations = layout.Item3;

            for (int i = 0; i < cardCount; i++)
            {
                Vector3 targetPosition = new(xPositions[i], yPositions[i], 0f);
                Vector3 targetRotation = new(0f, 0f, zRotations[i]);

                // 공통 함수 사용 (애니메이션 없이 즉시 배치)
                _cards[i].transform.localPosition = targetPosition;
                _cards[i].transform.localRotation = Quaternion.Euler(targetRotation);
                _cards[i].transform.localScale = Vector3.one;
                _tcgCards[i].SetCardFront();
            }

            if (ShouldUseLastCard)
                PositionLastCard(cardCount);
        }

        private void PositionLastCard(int cardCount)
        {
            Transform lastCard = _cards[cardCount - 1].transform;
            lastCard.localPosition = _cardBack.transform.localPosition;
            lastCard.localRotation = _cardBack.transform.localRotation;
            lastCard.localScale = _cardBack.transform.localScale;
        }

        private void MoveCardsUp()
        {
            if (ShouldUseLastCard)
                _cardBlock.SetActive(true);

            foreach (var card in _cards)
            {
                Vector3 targetPosition = card.transform.localPosition + new Vector3(0, 300, 0);
                MoveCardToPosition(card.transform, targetPosition, Vector3.zero, 1f);
            }

            if (_cardBack != null)
            {
                Vector3 targetPosition = _cardBack.transform.localPosition + new Vector3(0, 300, 0);
                MoveCardToPosition(_cardBack.transform, targetPosition, Vector3.zero, 1f);
            }

            if (_currentQuestionIndex == 0)
                Invoke(nameof(MoveAllCardsFromDeck), 1f);
            else if (ShouldUseLastCard)
                Invoke(nameof(MoveLastCardFromDeck), 1f);
        }

        private void MoveLastCardFromDeck()
        {
            Transform lastCard = _cards[_cards.Length - 1].transform;
            Vector3 targetPosition = new(234f, -221f, 0f);
            Vector3 targetRotation = new(0f, 0f, -15f);

            MoveCardToPosition(lastCard, targetPosition, targetRotation, 1f, () => _cardBlock.SetActive(false));

            DOVirtual.DelayedCall(0.1f, () =>
            {
                _tcgCards[_cards.Length - 1].SetCardFront();
            });
        }

        private void MoveAllCardsFromDeck()
        {
            if (!_cardLayouts.TryGetValue(_cards.Length, out var layout))
            {
                Debug.LogError("올바른 카드 배치 데이터를 찾을 수 없습니다.");
                return;
            }

            float delayBetweenCards = 0.3f;
            float[] xPositions = layout.Item1;
            float[] yPositions = layout.Item2;
            float[] zRotations = layout.Item3;

            Sequence sequence = DOTween.Sequence();

            for (int i = 0; i < _cards.Length; i++)
            {
                int index = i;
                sequence.AppendInterval(delayBetweenCards * i)
                    .AppendCallback(() =>
                    {
                        Vector3 targetPosition = new(xPositions[index], yPositions[index], 0f);
                        Vector3 targetRotation = new(0f, 0f, zRotations[index]);

                        // 공통 함수 사용 (애니메이션 적용)
                        MoveCardToPosition(_cards[index].transform, targetPosition, targetRotation, 0.5f);

                        DOVirtual.DelayedCall(0.1f, () =>
                        {
                            _tcgCards[index].SetCardFront();
                        });
                    });
            }

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

            // 선택되지 않은 카드들과 카드덱 Down
            Sequence cardsSequence = DOTween.Sequence();
            for (int i = 0; i < _cards.Length; i++)
            {
                if (_tcgCards[i].Index != answerIndex)
                {
                    Vector3 downPosition = _cards[i].transform.localPosition - new Vector3(0, 300, 0);
                    cardsSequence.Join(_cards[i].transform.DOLocalMove(downPosition, 1f).SetEase(Ease.InQuad));
                }
            }

            if (_cardBack != null)
            {
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
                        Vector3 centerPosition = _cards[i].transform.localPosition;
                        centerPosition.x = 0;

                        Sequence moveAndRotateSequence = DOTween.Sequence();
                        moveAndRotateSequence.Append(_cards[i].transform.DOLocalMove(centerPosition, 1f).SetEase(Ease.OutQuad));
                        moveAndRotateSequence.Join(_cards[i].transform.DOLocalRotate(Vector3.zero, 1f, RotateMode.Fast).SetEase(Ease.OutQuad));
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