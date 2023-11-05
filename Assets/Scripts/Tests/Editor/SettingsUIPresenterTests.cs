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
        public void InitializeSettingsSetsMusicVolume()
        {
            // var view = Substitute.For<SettingsUIView>();
            // var presenter = new SettingsUIPresenter(view);
            //
            // presenter.InitializeSettings();
            //
            // view.Received().SetMusicVolume(0.5f);
        }
    }
}
