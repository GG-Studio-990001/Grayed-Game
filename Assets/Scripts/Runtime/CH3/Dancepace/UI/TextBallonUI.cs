using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Runtime.CH3.Dancepace
{
    public class TextBallonUI : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;
        private string push;

        public void SetText(string text, bool showSuffix = true)
        {
            if(push == null)
                push = StringTableManager.Get("TextBallon_Push");
            if (showSuffix)
                _text.text = text + " " + push;
            else
                _text.text = text;
        }
    }
}