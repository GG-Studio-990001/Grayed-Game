using Runtime.CH2.SuperArio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArioStore : MonoBehaviour
{
    [SerializeField] private GameObject _ario;

    private void Start()
    {
        ArioManager.instance.OnEnterStore += EnterStore;
    }

    private void OnDestroy()
    {
        ArioManager.instance.OnEnterStore -= EnterStore;
    }

    public void EnterStore(bool isTrue)
    {
        _ario.SetActive(true);
    }
}
