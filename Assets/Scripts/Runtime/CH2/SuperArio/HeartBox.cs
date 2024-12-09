using System;
using System.Collections;
using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class HeartBox : MonoBehaviour, IStoreBox
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
            if (!ArioManager.instance.LifeCheck())
            {
                // 회색 만들기
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
            yield return new WaitForSeconds(1.25f);
            _col.enabled = true;
        }

        public void Use()
        {
            if (IsUsed)
                return;
            ArioManager.instance.PlusLife();
            IsUsed = true;
            SetColorGray();
        }

        public void SetColorGray()
        {
            if (_spr != null)
            {
                _spr.color = Color.gray; // 회색으로 설정
            }
        }

        public void ResetColor()
        {
            if (_spr != null)
            {
                _spr.color = _originalColor; // 원래 색상으로 복구
            }
        }
    }
}