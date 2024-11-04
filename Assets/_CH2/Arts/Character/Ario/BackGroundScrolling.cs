using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScrolling : MonoBehaviour
{
    private Renderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (ArioManager.instance.isPlay)
        {
            float move = Time.deltaTime * (ArioManager.instance.gameSpeed / 50);
            _renderer.material.mainTextureOffset += Vector2.right * move;
        }
    }
}