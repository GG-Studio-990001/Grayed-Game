using NSubstitute;
using NUnit.Framework;
using Runtime.Common.Presentation;
using Runtime.Common.View;

namespace Tests.Editor
{
    [TestFixture]
    public class DialogueUITests
    {
        private DialogueUIView _view;
        private DialogueUIPresenter _presenter;
        
        [SetUp]
        public void SetUp()
        {
            _view = Substitute.For<DialogueUIView>();
            _presenter = new DialogueUIPresenter(_view);
        }
        
        [Test]
        public void WhenDialogueStartsViewIsUpdated()
        {
            _presenter.StartDialogue("Hello World!");
            _view.Received(1).StartDialogue("Hello World!");
        }
    }
}