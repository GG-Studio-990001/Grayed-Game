using Febucci.UI.Core;
using UnityEngine;

namespace Febucci.UI.Effects
{
    /// <summary>
    /// Applies multiples animations, allowing user to use one tag for all of them
    /// </summary>
    [UnityEngine.Scripting.Preserve]
    [CreateAssetMenu(fileName = "Composite Animation", menuName = "Text Animator/Animations/Special/Composite")]
    [EffectInfo("", EffectCategory.All)]
    public sealed class CompositeAnimation : AnimationScriptableBase
    {
        public AnimationScriptableBase[] animations = new AnimationScriptableBase[0];

        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            ValidateArray();
            
            foreach (var anim in animations)
            {
                anim.InitializeOnce();
            }
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

        public override void ApplyEffectTo(ref CharacterData character, TAnimCore animator)
        {
            foreach (var anim in animations)
            {
                if(anim.CanApplyEffectTo(character, animator)) 
                    anim.ApplyEffectTo(ref character, animator);
            }
        }

        //Prevents double check
        public override bool CanApplyEffectTo(CharacterData character, TAnimCore animator) => true;
        
        public override float GetMaxDuration()
        {
            //Calculates max duration between animations
            float maxDuration = -1;
            foreach (var anim in animations)
            {
                maxDuration = Mathf.Max(maxDuration, anim.GetMaxDuration());
            }

            return maxDuration;
        }

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