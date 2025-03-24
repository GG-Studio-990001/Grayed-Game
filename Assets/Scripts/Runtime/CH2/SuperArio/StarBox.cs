using Runtime.ETC;
using System.Collections;
using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class StarBox : MonoBehaviour, IStoreBox
    {
        private SpriteRenderer _spr;
        private Color _originalColor;
        private BoxCollider2D _col;
    
        private void Start()
        {
            _spr = GetComponent<SpriteRenderer>();
            _col = GetComponent<BoxCollider2D>();
            _originalColor = _spr.color;
        }
    
        public bool IsUsed { get; set; }
        
        public void Check()
        {
            StartCoroutine(Delay());
            if (ArioManager.instance.HasItem)
            {
                SetColorGray();
                IsUsed = true;
            }
            else
            {
                ResetColor();
                IsUsed = false;
            }
        }
        
        private IEnumerator Delay()
        {
            _col.enabled = false;
            yield return new WaitForSeconds(0.95f);
            _col.enabled = true;
        }

        public void Use()
        {
            if (IsUsed || !ArioManager.instance.UseCoin(150))
                return;
            Managers.Sound.Play(Sound.SFX, "SuperArio/CH2_SUB_SFX_4");
            ArioManager.instance.GetItem();
            IsUsed = true;
            SetColorGray();
        }

        public void SetColorGray()
        {
            if (_spr != null)
            {
                _spr.color = Color.gray;
            }
        }

        public void ResetColor()
        {
            if (_spr != null)
            {
                _spr.color = _originalColor;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out ArioStore ario))
            {
                Use();
            }
        }
    }
}