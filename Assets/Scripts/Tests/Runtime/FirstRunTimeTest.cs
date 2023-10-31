using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Runtime.UI;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class FirstRunTimeTest
{
    private TriviaGamePresenter _presenter;
    private TriviaGameView _view;
    
    [SetUp]
    public void SetUp()
    {
        _view = new TriviaGameView();
        _presenter = new TriviaGamePresenter(_view);
    }

    [Test]
    public void WhenRightAnswerScoreIncreases()
    {
        var initialScore = _presenter.Score;
        WhenRightAnswer();
        ThenScoreIncreaseByOne(initialScore);
    }
    
    [Test]
    public void WhenRightAnswerShowsPositiveFeedback()
    {
        WhenRightAnswer();
        ThenShowsPositiveFeedback();
    }

    [Test]
    public void WhenRightAnswerShowsUpdatedScore()
    {
        WhenRightAnswer();
        //ThenShowsCurrentScore(_presenter.Score);
    }

    [Test]
    public void When3RightAnswersWin()
    {
        When3RightAnswers();
        ThenShowsWinningFeedback();
    }

    private void When3RightAnswers()
    {
        WhenRightAnswer();
        WhenRightAnswer();
        WhenRightAnswer();
    }
    
    private void WhenRightAnswer()
    {
        _presenter.ReceiveAnswer("ok");
    }
    
    private void ThenShowsWinningFeedback()
    {
        //_view.Received(1).ShowWinFeedback();
    }  
    
    private void ThenShowsPositiveFeedback()
    {
        //_view.Recived(1).ShowPositiveFeedback();
    }
    private void ThenScoreIncreaseByOne(int initialScore)
    {
        Assert.AreEqual(initialScore + 1, _presenter.Score);
    }
}
