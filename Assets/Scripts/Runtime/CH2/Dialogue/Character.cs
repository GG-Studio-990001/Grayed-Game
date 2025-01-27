using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CH2.Dialogue
{
    [RequireComponent(typeof(Image))]
    public class Character : MonoBehaviour
    {
        public string CharacterName;
        private Image _image;
        [SerializeField] private Sprite[] _expressions; // 표정 리스트
        private bool _isHighlighted = true;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void SetHighlight(bool highlight)
        {
            // Debug.Log($"{CharacterName} {highlight}");
            if (_isHighlighted == highlight) return;
            _isHighlighted = highlight;

            Color bright = new Color32(255, 255, 255, 255);
            Color dark = new Color32(157, 157, 157, 255);
            _image.color = highlight ? bright : dark;
        }

        public void SetExpression(int Index)
        {
            if (Index >= 0 && Index < _expressions.Length)
                _image.sprite = _expressions[Index];
        }
    }
}