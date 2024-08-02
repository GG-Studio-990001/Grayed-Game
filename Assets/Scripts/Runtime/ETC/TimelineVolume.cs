using UnityEngine;

namespace Runtime.ETC
{
    public class TimelineVolume : MonoBehaviour
    {
        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _audioSource.volume = Managers.Sound.SFX.volume;
        }
    }
}