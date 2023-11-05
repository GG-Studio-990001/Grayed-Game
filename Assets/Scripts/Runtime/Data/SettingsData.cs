using UnityEngine;

namespace Runtime.Data
{
    [CreateAssetMenu(fileName = "SettingsData", menuName = "Scriptable Object/SettingsData", order = 1)]
    public class SettingsData : ScriptableObject
    {
        [SerializeField] private float _musicVolume;
        [SerializeField] private float _sfxVolume;

        public float MusicVolume => _musicVolume;
        public float SfxVolume => _sfxVolume;

        public void SetMusicVolume(float volume)
        {
            _musicVolume = volume;
        }
    
        public void SetSfxVolume(float volume)
        {
            _sfxVolume = volume;
        }
    }
}
