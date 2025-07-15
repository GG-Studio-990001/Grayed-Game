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

        public void SetText(string text, bool showSuffix = true)
        {
            if (showSuffix)
                _text.text = text + " 눌러!";
            else
                _text.text = text;
        }
    }
}