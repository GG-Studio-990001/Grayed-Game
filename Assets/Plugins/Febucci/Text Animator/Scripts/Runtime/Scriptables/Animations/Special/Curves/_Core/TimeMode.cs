namespace Febucci.UI.Effects
{
    [System.Serializable]
    public struct TimeMode
    {
        public float startDelay;
        public bool useUniformTime;
        public float waveSize;
        public float timeSpeed;

        public TimeMode(bool useUniformTime)
        {
            this.useUniformTime = useUniformTime;
            waveSize = 0;
            timeSpeed = 1;
            startDelay = 0;
            tempTime = 0;
        }

        float tempTime;
        public float GetTime(float animatorTime, float charTime, int charIndex)
        {
            tempTime = ((useUniformTime ? animatorTime : charTime) - startDelay) * timeSpeed - waveSize * charIndex;
            if (tempTime < startDelay)
                return -1;
            return tempTime;
        }
    }
}