using NSubstitute;
using NUnit.Framework;
using Runtime.UI.Settings;

namespace Tests.Editor
{
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
}
