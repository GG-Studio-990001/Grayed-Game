using System;

namespace Runtime.Common.Domain
{
    public class SettingsData
    {
        public float MusicVolume
        {
            get => _musicVolume;
            set { Math.Clamp(value, 0, 1); _musicVolume = value; }
        }
        
        public float SfxVolume
        {
            get => _sfxVolume;
            set { Math.Clamp(value, 0, 1); _sfxVolume = value; }
        }
        
        private float _musicVolume = 1;
        private float _sfxVolume = 1;
    }
}