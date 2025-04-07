using Runtime.ETC;
using System.Collections;
using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class ExitBox : ItemBox
    {
        private void Start()
        {
            Init();
        }

        public override void Check()
        {
            StartCoroutine(Delay());
            ResetSprite();
            IsUsed = false;
        }

        protected override void Use()
        {
            if (IsUsed)
                return;
            Managers.Sound.Play(Sound.SFX, "SuperArio/CH2_SUB_SFX_4");
            // 벽 열기
            ArioManager.Instance.StoreOpenEvent();
            IsUsed = true;
            ChangeSprite();
        }
    }
}