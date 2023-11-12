using Runtime.Common;
using UnityEngine;

namespace Runtime.CH1.Main
{
    public class TestInteraction : MonoBehaviour, Interactive
    {
        public void Interact()
        {
            Debug.Log("Test");
        }
    }
}