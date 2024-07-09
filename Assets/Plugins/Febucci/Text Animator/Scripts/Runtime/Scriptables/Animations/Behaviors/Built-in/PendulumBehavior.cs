using Febucci.UI.Core;
using UnityEngine;

namespace Febucci.UI.Effects
{
    [UnityEngine.Scripting.Preserve]
    [CreateAssetMenu(fileName = "Pendulum Behavior", menuName = "Text Animator/Animations/Behaviors/Pendulum")]
    [EffectInfo("pend", EffectCategory.Behaviors)]
    [DefaultValue(nameof(baseAmplitude), 24.7f)]
    [DefaultValue(nameof(baseFrequency), 3.1f)]
    [DefaultValue(nameof(baseWaveSize), .2f)]
    public sealed class PendulumBehavior : BehaviorScriptableSine
    {
        public bool anchorBottom;

        int targetVertex1;
        int targetVertex2;

        public override void ResetContext(TAnimCore animator)
        {
            base.ResetContext(animator);

            if (anchorBottom)
            {
                //anchored at the bottom
                targetVertex1 = 0;
                targetVertex2 = 3;
            }
            else
            {
                //anchored at the top
                targetVertex1 = 1;
                targetVertex2 = 2;
            }
        }

        public override void ApplyEffectTo(ref Core.CharacterData character, TAnimCore animator)
        {
            character.current.positions.RotateChar(
                Mathf.Sin(-animator.time.timeSinceStart * frequency + waveSize * character.index) * amplitude,
                (character.current.positions[targetVertex1] + character.current.positions[targetVertex2]) / 2 //bottom center as their rotation pivot
                );
        }
    }

}