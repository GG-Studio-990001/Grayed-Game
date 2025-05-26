using Runtime.ETC;
using System;
using System.Collections;
using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class HeartBox : ItemBox
    {
        private void Start()
        {
            Init();
            _cost = 30;
        }

        public override void Check()
        {
            StartCoroutine(Delay());
            if (!ArioManager.Instance.LifeCheck() || !CanBuy())
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
            ArioManager.Instance.PlusLife();
            IsUsed = true;
            ChangeSprite();
        }
    }
}