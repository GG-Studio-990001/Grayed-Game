using Runtime.Common.View;
using UnityEngine;

namespace Runtime.Common.Presentation
{
    public class DialogueUIPresenter
    {
        private readonly DialogueUIView _view;
        
        public DialogueUIPresenter(DialogueUIView view)
        {
            _view = view;
        }
        
        public void StartDialogue(string text)
        {
            _view.StartDialogue(text);
        }
    }
}