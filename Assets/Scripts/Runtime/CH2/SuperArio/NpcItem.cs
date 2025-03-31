using Runtime.CH2.SuperArio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out ArioReward ario))
        {
            gameObject.SetActive(false);
        }
    }
}
