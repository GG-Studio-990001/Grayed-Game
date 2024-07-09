using Febucci.UI.Core;
using Febucci.UI.Effects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Febucci.UI.Effects
{
    [UnityEngine.Scripting.Preserve]
    [CreateAssetMenu(fileName = "Vertex Curve Animation", menuName = "Text Animator/Animations/Special/Vertex Curve Animation")]
    [EffectInfo("", EffectCategory.All)]
    public sealed class VertexCurveAnimation : AnimationScriptableBase
    {
        public TimeMode timeMode = new TimeMode(true);
        [EmissionCurveProperty] public EmissionCurve emissionCurve = new EmissionCurve();

        [SerializeField] AnimationData[] animationPerVertexData = new AnimationData[TextUtilities.verticesPerChar];
        public AnimationData[] VertexData 
        { 
            get => animationPerVertexData; 
            set 
            {
                animationPerVertexData = value;
                ClampVertexDataArray();
            }
        }

        //--- Modifiers ---
        float timeSpeed;
        float weightMult;

        //--- Management ---
        Matrix4x4 matrix;
        Vector3 offset;
        Vector3 movement;
        Vector2 scale;
        Quaternion rot;
        Color32 color;

        public override void ResetContext(TAnimCore animator)
        {
            weightMult = 1;
            timeSpeed = 1;
            ClampVertexDataArray();
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
            if(timePassed<0)
                return;
            
            float weight = weightMult * emissionCurve.Evaluate(timePassed);

            for (byte i = 0; i < TextUtilities.verticesPerChar; i++)
            {
                if(animationPerVertexData[i].TryCalculatingMatrix(character, timePassed, weight, out matrix, out offset))
                {
                    character.current.positions[i] = matrix.MultiplyPoint3x4(character.current.positions[i] - offset) + offset;
                }

                if(animationPerVertexData[i].TryCalculatingColor(character, timePassed, weight, out color))
                {
                    character.current.colors[i] = Color32.LerpUnclamped(character.current.colors[i], color, Mathf.Clamp01(weight));
                }
            }
        }

        public override float GetMaxDuration() => emissionCurve.GetMaxDuration();
        public override bool CanApplyEffectTo(CharacterData character, TAnimCore animator) => true;
        
        void ClampVertexDataArray()
        {
            for (int i = 0; i < animationPerVertexData.Length; i++)
            {
                if (animationPerVertexData[i] == null)
                    animationPerVertexData[i] = new AnimationData();
            }
            
            if (animationPerVertexData.Length != TextUtilities.verticesPerChar)
            {
                Debug.LogError("Vertex data array must have four vertices. Clamping/Resizing to four.");
                
                var newArray = new AnimationData[TextUtilities.verticesPerChar];
                for (int i = 0; i < newArray.Length; i++)
                {
                    if (i < animationPerVertexData.Length)
                        newArray[i] = animationPerVertexData[i];
                    else
                        newArray[i] = new AnimationData();
                }
                animationPerVertexData = newArray;
            }
        }

        void OnValidate() => ClampVertexDataArray();
    }
}