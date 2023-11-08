using NSubstitute;
using NUnit.Framework;
using Runtime.Common.Domain;
using Runtime.Common.Presentation;
using Runtime.Common.View;

namespace Tests.Editor
{
    [TestFixture]
    public class SettingsUIPresenterTests
    {
        private SettingsUIView _settingsUIView;
        private SettingsData _settingsData;
        private SettingsUIPresenter _settingsUIPresenter;
        
        [SetUp]
        public void SetUp()
        {
            _settingsUIView = Substitute.For<SettingsUIView>();
            _settingsData = new SettingsData();
            _settingsUIPresenter = new SettingsUIPresenter(_settingsUIView, _settingsData);
        }
        
        [Test]
        public void WhenMusicVolumeChangesThenMusicVolumeIsSet()
        {
            _settingsUIPresenter.SetMusicVolume(0.5f);

            Assert.AreEqual(0.5f, _settingsData.MusicVolume);
        }
        
        [Test]
        public void WhenMusicVolumeChangesThenMusicVolumeIsSetOnView()
        {
            _settingsUIPresenter.SetMusicVolume(0.5f);
            
            _settingsUIView.Received(1).SetMusicVolume(0.5f);
        }
        
        [Test]
        public void WhenSfxVolumeChangesThenSfxVolumeIsSet()
        {
            _settingsUIPresenter.SetSfxVolume(0.5f);

            Assert.AreEqual(0.5f, _settingsData.SfxVolume);
        }
        
        [Test]
        public void WhenSfxVolumeChangesThenSfxVolumeIsSetOnView()
        {
            _settingsUIPresenter.SetSfxVolume(0.5f);
            
            _settingsUIView.Received(1).SetSfxVolume(0.5f);
        }
        
        [TearDown]
        public void TearDown()
        {
            _settingsUIView = null;
            _settingsData = null;
            _settingsUIPresenter = null;
        }
    }
}
