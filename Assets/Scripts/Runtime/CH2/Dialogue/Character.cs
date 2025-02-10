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

        public void SetHighlight(bool highlight, int idx = -1) /// -1: 진도비글 아님 / 0: 진도 / 1: 비글 / 2: 진도비글인데 말안함
        {
            Color bright = new Color32(255, 255, 255, 255);
            Color dark = new Color32(157, 157, 157, 255);

            // TODO: 리팩터링
            if (idx == -1)
            {
                if (_isHighlighted == highlight) return;

                _isHighlighted = highlight;
                _image.color = highlight ? bright : dark;
            }
            else if (idx == 2)
            {
                for (int i = 0; i < 2; i++)
                    transform.GetChild(i).GetComponent<Image>().color = dark;
            }
            else
            {
                transform.GetChild(idx).GetComponent<Image>().color = bright;
                transform.GetChild(1 - idx).GetComponent<Image>().color = dark;
            }
        }

        public void SetExpression(int Index)
        {
            if (Index >= 0 && Index < _expressions.Length)
                _image.sprite = _expressions[Index];
        }
    }
}