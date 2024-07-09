using Febucci.UI.Core;
using UnityEngine;

namespace Febucci.UI.Effects
{
    [UnityEngine.Scripting.Preserve]
    [CreateAssetMenu(menuName = "Text Animator/Animations/Behaviors/Rotation", fileName = "Rotation Behavior")]
    [EffectInfo("rot", EffectCategory.Behaviors)]
    public sealed class RotationBehavior : BehaviorScriptableBase
    {

        public float baseRotSpeed = 180;
        public float baseDiffBetweenChars = 10;

        float angleSpeed;
        float angleDiffBetweenChars;

        public override void SetModifier(ModifierInfo modifier)
        {
            switch (modifier.name)
            {
                //frequency
                case "f": angleSpeed = baseRotSpeed * modifier.value; break;
                //angle diff
                case "w": angleDiffBetweenChars = baseDiffBetweenChars * modifier.value; break;
            }
        }

        public override void ResetContext(TAnimCore animator)
        {
            angleSpeed = baseRotSpeed;
            angleDiffBetweenChars = baseDiffBetweenChars;
        }

        public override void ApplyEffectTo(ref Core.CharacterData character, TAnimCore animator)
        {
            character.current.positions.RotateChar(-animator.time.timeSinceStart * angleSpeed 
                + angleDiffBetweenChars * character.index);
        }
    }

}