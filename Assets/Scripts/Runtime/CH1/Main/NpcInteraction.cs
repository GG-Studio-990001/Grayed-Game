using Runtime.CH1.Main.Interface;
using Runtime.Common.Presentation;
using System;
using UnityEngine;

namespace Runtime.CH1.Main
{
    public class NpcInteraction : MonoBehaviour, IInteractive
    {
        public bool Interact()
        {
            return true;
            
            Debug.Log("Test");
        }
    }
}