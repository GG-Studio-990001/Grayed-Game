using Febucci.UI.Core;

namespace Febucci.UI.Effects
{
    /// <summary>
    /// Base class for animating letters in Text Animator
    /// </summary>
    [System.Serializable]
    public abstract class AppearanceScriptableBase : AnimationScriptableBase
    {
        public float baseDuration = .5f;
        protected float duration;

        public override void ResetContext(TAnimCore animator)
        {
            duration = baseDuration;
        }

        public override float GetMaxDuration() => duration; //TODO improve this, a bit hacky
        public override void SetModifier(ModifierInfo modifier)
        {
            switch (modifier.name)
            {
                case "d": duration = baseDuration * modifier.value; break;
            }
        }

        public override bool CanApplyEffectTo(CharacterData character, TAnimCore animator) => character.passedTime <= duration;
    }

}