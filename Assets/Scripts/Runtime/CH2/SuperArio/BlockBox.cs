using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class BlockBox : ItemBox
    {
        private void Start()
        {
            Init();
        }

        public override void Check()
        {
            StartCoroutine(Delay());
            IsUsed = true;
        }

        protected override void Use()
        {
            if (IsUsed)
                return;
        }
    }
}
