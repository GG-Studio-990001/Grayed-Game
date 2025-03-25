using System;

namespace Runtime.Data
{
    [Serializable]
    public class SettingData
    {
        public float BgmVolume;
        public float SfxVolume;
        public bool isFullScreen;

        public SettingData()
        {
            BgmVolume = 0.5f;
            SfxVolume = 0.5f;
            isFullScreen = true;
        }
    }
}