using Febucci.UI.Core;
using Febucci.UI.Effects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Febucci.UI.Effects
{
    [UnityEngine.Scripting.Preserve]
    [CreateAssetMenu(fileName = "Uniform Curve Animation", menuName = "Text Animator/Animations/Special/Uniform Curve")]
    [EffectInfo("", EffectCategory.All)]
    public sealed class UniformCurveAnimation : AnimationScriptableBase
    {
        public TimeMode timeMode = new TimeMode(true);
        [EmissionCurveProperty] public EmissionCurve emissionCurve = new EmissionCurve();
        public AnimationData animationData = new AnimationData();

        //--- Modifiers ---
        float weightMult;
        float timeSpeed;

        bool hasTransformEffects;


        public override void ResetContext(TAnimCore animator)
        {
            weightMult = 1;
            timeSpeed = 1;
        }

        public override void SetModifier(ModifierInfo modifier)
        {
            switch (modifier.name)
            {
                case "f": //frequency, increases the time speed
                    timeSpeed = modifier.value;
                    break;

                case "a": //increase the amplitude
                    weightMult = modifier.value;
                    break;
            }
        }

        float timePassed;
        public override void ApplyEffectTo(ref Core.CharacterData character, TAnimCore animator)
        {
            timePassed = timeMode.GetTime(animator.time.timeSinceStart * timeSpeed, character.passedTime * timeSpeed, character.index);
            if (timePassed < 0)
                return;
            
            float weight = weightMult * emissionCurve.Evaluate(timePassed);
            
            if(animationData.TryCalculatingMatrix(character, timePassed, weight, out var matrix, out var offset))
            {
                for (byte i = 0; i < TextUtilities.verticesPerChar; i++)
                {
                    character.current.positions[i] = matrix.MultiplyPoint3x4(character.current.positions[i] - offset) + offset;
                }
            }

            if(animationData.TryCalculatingColor(character, timePassed, weight, out var color))
            {
                character.current.colors.LerpUnclamped(color, Mathf.Clamp01(weight));
            }

        }

        public override float GetMaxDuration() => emissionCurve.GetMaxDuration();

        public override bool CanApplyEffectTo(CharacterData character, TAnimCore animator) => true;
    }
}