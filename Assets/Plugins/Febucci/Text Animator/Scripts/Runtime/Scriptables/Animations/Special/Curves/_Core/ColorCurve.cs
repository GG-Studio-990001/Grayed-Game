using UnityEngine;

namespace Febucci.UI.Effects
{
    public class ColorCurveProperty : PropertyAttribute
    {
        
    }
    
    [System.Serializable]
    public struct ColorCurve
    {
        public bool enabled;

        public Gradient colorOverTime;
        public float waveSize;
        public float duration;

        public ColorCurve(float waveSize)
        {
            this.enabled = false;
            this.waveSize = waveSize;
            this.duration = 1;
            this.colorOverTime = new Gradient();
            colorOverTime.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(Color.white, 0), 
                    new GradientColorKey(Color.cyan, 0.5f), 
                    new GradientColorKey(Color.white, 1) 
                },

                new GradientAlphaKey[] 
                { 
                    new GradientAlphaKey(1, 0),
                    new GradientAlphaKey(1, 1) 
                }
            );
        }

        public Color32 Evaluate(float time, int charIndex)
        {
            time = Mathf.Repeat(time + charIndex * waveSize, duration);
            return colorOverTime.Evaluate(time);
        }
    }
}