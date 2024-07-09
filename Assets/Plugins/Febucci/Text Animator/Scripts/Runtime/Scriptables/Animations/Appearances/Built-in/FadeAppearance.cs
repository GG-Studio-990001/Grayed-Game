using Febucci.UI.Core;
using Febucci.UI.Effects;
using UnityEngine;

namespace Febucci.UI.Effects
{
    [UnityEngine.Scripting.Preserve]
    [CreateAssetMenu(fileName = "Fade Appearance", menuName = "Text Animator/Animations/Appearances/Fade")]
    [EffectInfo("fade", EffectCategory.Appearances)]
    public sealed class FadeAppearance : AppearanceScriptableBase
    {
        Color32 temp;
        
        public override void ApplyEffectTo(ref Core.CharacterData character, TAnimCore animator)
        {
            //from transparent to real color
            for (int i = 0; i < TextUtilities.verticesPerChar; i++)
            {
                temp = character.current.colors[i];
                temp.a = 0;

                character.current.colors[i] = Color32.LerpUnclamped(character.current.colors[i], temp,
                    Tween.EaseInOut(1 - (character.passedTime / duration)));
            }
            
        }
    }

}