using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CH2.Dialogue
{
    [RequireComponent(typeof(Image))]
    public class FaceSpriteSwitcher : MonoBehaviour
    {
        private Image _image;
        [SerializeField] private Sprite[] _sprites;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void SetFace(int idx)
        {
            _image.sprite = _sprites[idx];
        }
    }
}