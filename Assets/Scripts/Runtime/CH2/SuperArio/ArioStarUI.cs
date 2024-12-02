using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CH2.SuperArio
{
    public class ArioStarUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _count;
        private Image _image;
        private Color _originalColor;
    
        private void Start()
        {
            _image = GetComponent<Image>();
            _count.gameObject.SetActive(false);
            _originalColor = _image.color;
        }
    
        public void StartCount()
        {
            _count.gameObject.SetActive(true);
            StartCoroutine(ItemCount());
            SetImageColorGray();
        }
        
        private IEnumerator ItemCount()
        {
            int num = 20;
    
            while (num >= 0)
            {
                _count.text = num.ToString();
                yield return new WaitForSeconds(1f);
                num--;
            }
            _count.gameObject.SetActive(false);
        }
        
        private void SetImageColorGray()
        {
            if (_image != null)
            {
                _image.color = Color.gray; // 회색으로 설정
            }
        }
        
        public void ResetImageColor()
        {
            if (_image != null)
            {
                _image.color = _originalColor; // 원래 색상으로 복구
            }
        }
    }
}