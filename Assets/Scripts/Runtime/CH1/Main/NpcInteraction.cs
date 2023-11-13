using Runtime.CH1.Main.Interface;
using Runtime.Common.Presentation;
using System;
using UnityEngine;

namespace Runtime.CH1.Main
{
    public class NpcInteraction : MonoBehaviour, IInteractive
    {
        private DialogueUIPresenter _presenter;
        
        public void Interact()
        {
            Debug.Log("Test");
        }
    }
}