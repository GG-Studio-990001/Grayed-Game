using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CH2.Location
{
    [RequireComponent(typeof(Image))]
    public class BGSpriteSwitcher : MonoBehaviour
    {
        private Image _image;
        [SerializeField] private Sprite[] _sprites;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void SetBG()
        {
            string loc = Managers.Data.CH2.Location;

            if (loc == "마마고 컴퍼니")
            {
                _image.sprite = _sprites[0];
            }
            else
            {
                _image.sprite = _sprites[1];
            }
        }
    }
}