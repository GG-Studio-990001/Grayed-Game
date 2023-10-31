namespace Runtime.UI
{
    public class TriviaGamePresenter
    {
        public int Score { get; private set; }

        public TriviaGamePresenter()
        {
            Score = 0;
        }
        
        public void ReceiveAnswer(string answer)
        { 
            Score++;
        }
    }
}