using Febucci.UI.Core;
using Febucci.UI.Effects;
using UnityEngine;

namespace Febucci.UI.Examples
{
    [AddComponentMenu("")]
    public class DefaultEffectsExample : MonoBehaviour
    {
        public TypewriterCore typewriter;
        TextAnimatorSettings settings;

        private void Awake()
        {
            UnityEngine.Assertions.Assert.IsNotNull(typewriter, $"Text Animator Player component is null in {gameObject.name}");
            settings = TextAnimatorSettings.Instance;
            UnityEngine.Assertions.Assert.IsNotNull(settings, $"Text Animator Settings is null.");
        }

        string AddEffect<T>(TextAnimatorSettings.Category<T> category, string tag) where T: ScriptableObject
        {
            return $"{category.openingSymbol}{tag}{category.closingSymbol}{tag}{category.openingSymbol}/{category.closingSymbol}, ";
        }
        
        private void Start()
        {
            const char quote = '"';
            //builds the text with all the default tags
            string builtText = "<b>You can add effects by using <color=red>rich text tags</color>.</b>" +
                               $"\nExample: writing {quote}<noparse><shake>I'm cold</shake></noparse>{quote} will result in {quote}<shake>I'm cold</shake>{quote}." +
                               $"\n\n Effects that animate through time are called {quote}<color=red>Behaviors</color>{quote}, and the default tags are: ";
            
            foreach (var effect in typewriter.TextAnimator.DatabaseBehaviors.Data)
            {
                if(!effect) continue;
                builtText += AddEffect(settings.behaviors, effect.TagID);
            }

            builtText += $"\n\n<b>Effects that animate letters while they appear on screen are called {quote}<color=red>Appearances</color>{quote} and the default tags are</b>: ";

            foreach (var effect in typewriter.TextAnimator.DatabaseAppearances.Data)
            {
                if(!effect) continue;
                builtText += AddEffect(settings.appearances, effect.TagID);
            }
            
            //shows the text dynamically (typewriter like)
            typewriter.ShowText(builtText);

        }
    }

}