using System;
using UnityEngine;

namespace Runtime.UI
{
    public class UITest : MonoBehaviour
    {
        NewControls _newControls;

        private void Awake()
        {
            _newControls = new NewControls();
            
            _newControls.Player.Enable();
            _newControls.Player.Move.performed += ctx => Debug.Log("Move performed");
        }
    }
}