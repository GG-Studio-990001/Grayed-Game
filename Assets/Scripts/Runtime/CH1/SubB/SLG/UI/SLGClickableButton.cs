using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Runtime.ETC;

public class SLGClickableButton : MonoBehaviour
{
    private Button _button;
    void Start()
    {
        _button = GetComponent<Button>();
        if(_button != null )
        {
            _button.onClick.AddListener(PlayClickSound);
        }
    }

    void PlayClickSound()
    {
        Managers.Sound.Play(Sound.SFX, "SLG/SLG_Click_SFX");
    }
}
