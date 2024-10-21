using Runtime.ETC;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingSFXSlider : MonoBehaviour, IPointerUpHandler
{
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        Managers.Sound.Play(Sound.SFX, "SLG/SLG_Stone_SFX");
    }
}