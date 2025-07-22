using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Runtime.CH3.Dancepace
{
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string stringKey;

        private void Awake()
        {
            string localized = StringTableManager.Get(stringKey);
            var tmp = GetComponent<TMP_Text>();
            if (tmp != null) { tmp.text = localized; return; }
            var uiText = GetComponent<Text>();
            if (uiText != null) { uiText.text = localized; }
        }

        public void Refresh()
        {
            string localized = StringTableManager.Get(stringKey);
            var tmp = GetComponent<TMP_Text>();
            if (tmp != null) { tmp.text = localized; return; }
            var uiText = GetComponent<Text>();
            if (uiText != null) { uiText.text = localized; }
        }
    }
} 