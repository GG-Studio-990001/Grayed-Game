using Febucci.UI.Core;
using Febucci.UI.Effects;
using UnityEngine;

namespace Febucci.UI.Effects
{
    /// <summary>
    /// Applies multiples animations, allowing user to use one tag for all of them
    /// </summary>
    [UnityEngine.Scripting.Preserve]
    [CreateAssetMenu(fileName = "Composite With Emission", menuName = "Text Animator/Animations/Special/Composite With Emission")]
    [EffectInfo("", EffectCategory.All)]
    public sealed class CompositeWithEmission : AnimationScriptableBase
    {
        public TimeMode timeMode = new TimeMode(true);
        [EmissionCurveProperty] public EmissionCurve emissionCurve = new EmissionCurve();
        public AnimationScriptableBase[] animations = new AnimationScriptableBase[0];

        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            ValidateArray();
            
            foreach (var anim in animations)
            {
                anim.InitializeOnce();
            }

            prev = new MeshData();
            prev.colors = new Color32[TextUtilities.verticesPerChar];
            prev.positions = new Vector3[TextUtilities.verticesPerChar];
        }

        public override void ResetContext(TAnimCore animator)
        {
            foreach (var anim in animations)
            {
                anim.ResetContext(animator);
            }
        }

        public override void SetModifier(ModifierInfo modifier)
        {
            base.SetModifier(modifier);
            foreach (var anim in animations)
            {
                anim.SetModifier(modifier);
            }
        }

        MeshData prev;
        public override void ApplyEffectTo(ref CharacterData character, TAnimCore animator)
        {
            float timePassed = timeMode.GetTime(animator.time.timeSinceStart, character.passedTime, character.index);
            if(timePassed<0) return;
            
            for (int i = 0; i < TextUtilities.verticesPerChar; i++)
            {
                prev.positions[i] = character.current.positions[i];
                prev.colors[i] = character.current.colors[i];
            }
            
            float weight = emissionCurve.Evaluate(timePassed);
            foreach (var anim in animations)
            {
                if(anim.CanApplyEffectTo(character, animator)) 
                    anim.ApplyEffectTo(ref character, animator);
            }

            for (int i = 0; i < TextUtilities.verticesPerChar; i++)
            {
                character.current.positions[i] = Vector3.LerpUnclamped(prev.positions[i], character.current.positions[i], weight);
                character.current.colors[i] = Color32.LerpUnclamped(prev.colors[i], character.current.colors[i], weight);
            }
        }

        //Prevents double check
        public override bool CanApplyEffectTo(CharacterData character, TAnimCore animator) => true;

        public override float GetMaxDuration() => emissionCurve.GetMaxDuration();

        void ValidateArray()
        { 
            //prevents recursion
            var validated = new System.Collections.Generic.List<AnimationScriptableBase>();
             
            for (int i = 0; i < animations.Length; i++)
            {
                if(animations[i]!=this) validated.Add(animations[i]);
            }

            animations = validated.ToArray();
        }
        
        void OnValidate() => ValidateArray();
    }
}