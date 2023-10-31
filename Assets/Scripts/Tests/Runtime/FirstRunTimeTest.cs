using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Runtime.UI;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class FirstRunTimeTest
{
    [Test]
    public void DummyTest()
    {
        Assert.IsTrue(true);
    }

    [Test]
    public void WhenRightAnswerScoreIncreases()
    {
        // Given
        TriviaGamePresenter presenter = new TriviaGamePresenter();
        var initialScore = presenter.Score;
        
        // When
        presenter.ReceiveAnswer("ok");
        
        // Then
        Assert.AreEqual(initialScore + 1, presenter.Score);
    }

}
