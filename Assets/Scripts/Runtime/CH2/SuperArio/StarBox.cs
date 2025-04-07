using Runtime.ETC;
using System.Collections;
using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class StarBox : ItemBox
    {
        private void Start()
        {
            Init();
            _cost = 150;
        }

        public override void Check()
        {
            StartCoroutine(Delay());
            if (ArioManager.Instance.HasItem || !CanBuy())
            {
                ChangeSprite();
                IsUsed = true;
                return;
            }

            ResetSprite();
            IsUsed = false;
        }

        protected override void Use()
        {
            if (IsUsed || !ArioManager.Instance.UseCoin(_cost))
                return;
            Managers.Sound.Play(Sound.SFX, "SuperArio/CH2_SUB_SFX_4");
            ArioManager.Instance.GetItem();
            IsUsed = true;
            ChangeSprite();
        }
    }
}