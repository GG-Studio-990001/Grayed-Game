namespace Runtime.UI
{
    public class TriviaGamePresenter
    {
        public int Score { get; private set; }
        private TriviaGameView _view;

        public TriviaGamePresenter(TriviaGameView view)
        {
            _view = view;
            Score = 0;
        }
        
        public void ReceiveAnswer(string answer)
        { 
            OnRightAnswerReceived();
        }
        
        private void OnRightAnswerReceived()
        {
            Score++;
            _view.UpdateScore(Score);

            if (Score == 3)
            {
                _view.ShowWinFeedback();
            }
            else
            {
                _view.ShowPositiveFeedback();
            }
        }
    }
}