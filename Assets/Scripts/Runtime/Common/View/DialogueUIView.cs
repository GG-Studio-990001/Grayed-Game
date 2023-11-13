using Runtime.Common.Presentation;
using System;
using TMPro;
using UnityEngine;

namespace Runtime.Common.View
{
    public class DialogueUIView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dialogueText;
        
        private DialogueUIPresenter _presenter;

        private void Start()
        {
            _presenter = PresenterFactory.CreateDialogueUIPresenter(this);
        }
        
        public virtual void StartDialogue(string text)
        {
            dialogueText.text = text;
        }
        
        public DialogueUIPresenter GetPresenter()
        {
            return _presenter;
        }
    }
}