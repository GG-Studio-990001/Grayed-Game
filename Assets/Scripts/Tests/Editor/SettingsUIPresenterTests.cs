using NSubstitute;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Runtime.UI.Settings;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class SettingsUIPresenterTests
{
    [Test]
    public void DummyTest()
    {
        Assert.IsTrue(true);
    }
    
    [Test]
    public void WhenChangeMusicVolumeThenMusicVolumeIsChanged()
    {
        // Given
        var view = Substitute.For<SettingsUIView>();
        var presenter = new SettingsUIPresenter(view);
        
        // When
        presenter.OnMusicVolumeChanged(0.5f);
        
        // Then
        view.Received().SetMusicVolume(0.5f);
    }
}
