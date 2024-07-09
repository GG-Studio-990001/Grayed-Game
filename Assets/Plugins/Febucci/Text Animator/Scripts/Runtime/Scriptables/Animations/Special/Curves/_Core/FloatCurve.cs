using UnityEngine;
using UnityEngine.Serialization;

namespace Febucci.UI.Effects
{
    public class FloatCurveProperty : PropertyAttribute
    {
        
    }
    
    [System.Serializable] //TODO test
    public struct FloatCurve
    {
        public bool enabled;

        readonly float defaultAmplitude;
        public AnimationCurve weightOverTime;
        public float amplitude;
        public float waveSize;
        
        public FloatCurve(float amplitude, float waveSize, float defaultAmplitude)
        {
            this.defaultAmplitude = defaultAmplitude;
            this.enabled = false;
            this.amplitude = amplitude;
            this.weightOverTime = new AnimationCurve(new Keyframe(0, 0), new Keyframe(.5f, .5f), new Keyframe(1, 0));
            this.weightOverTime.preWrapMode = WrapMode.Loop;
            this.weightOverTime.postWrapMode = WrapMode.Loop;
            this.waveSize = 0;
        }

        public float Evaluate(float passedTime, int charIndex)
        {
            if(!enabled) return defaultAmplitude;

            return Mathf.LerpUnclamped(defaultAmplitude, amplitude, weightOverTime.Evaluate(passedTime + waveSize * charIndex));
        }
    }
}