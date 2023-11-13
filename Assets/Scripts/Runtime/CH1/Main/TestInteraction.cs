using Runtime.CH1.Main.Interface;
using UnityEngine;

namespace Runtime.CH1.Main
{
    public class TestInteraction : MonoBehaviour, IInteractive
    {
        public void Interact()
        {
            Debug.Log("Test");
        }
    }
}