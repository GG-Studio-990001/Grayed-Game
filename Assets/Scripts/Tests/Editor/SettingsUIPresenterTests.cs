using NSubstitute;
using NUnit.Framework;
using Runtime.UI.Settings;

namespace Tests.Editor
{
    [TestFixture]
    public class SettingsUIPresenterTests
    {
        private SettingsUIView _view;
        private SettingsUIPresenter _presenter;
        
        [SetUp]
        public void SetUp()
        {
            _view = Substitute.For<SettingsUIView>();
            _presenter = new SettingsUIPresenter(_view);
        }
        
        [Test]
        public void InitializeSettings()
        {
            _presenter.InitializeSettings();
            
            _view.Received().SetMusicVolume(0.5f);
        }
    }
}
