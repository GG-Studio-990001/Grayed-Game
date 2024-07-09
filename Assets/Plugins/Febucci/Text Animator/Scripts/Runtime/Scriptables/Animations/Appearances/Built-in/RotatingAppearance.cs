using Febucci.UI.Core;
using Febucci.UI.Effects;
using UnityEngine;

namespace Febucci.UI.Effects
{
    [UnityEngine.Scripting.Preserve]
    [CreateAssetMenu(fileName = "Rotating Appearance", menuName = "Text Animator/Animations/Appearances/Rotating")]
    [EffectInfo("rot", EffectCategory.Appearances)]
    [DefaultValue(nameof(baseDuration), .7f)]
    public sealed class RotatingAppearance : AppearanceScriptableBase
    {
        public float baseTargetAngle = 50;
        float targetAngle;
        
        public override void ResetContext(TAnimCore animator)
        {
            base.ResetContext(animator);
            targetAngle = baseTargetAngle;
        }

        public override void ApplyEffectTo(ref Core.CharacterData character, TAnimCore animator)
        {
            character.current.positions.RotateChar(
                Mathf.Lerp(
                    targetAngle,
                    0,
                    Tween.EaseInOut(character.passedTime / duration)
                )
            );
        }

        public override void SetModifier(ModifierInfo modifier)
        {
            switch (modifier.name)
            {
                case "a": targetAngle = baseTargetAngle * modifier.value; break;
                default: base.SetModifier(modifier); break;
            }
        }
    }

}