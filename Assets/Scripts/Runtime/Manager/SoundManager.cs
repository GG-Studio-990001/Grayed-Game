using Runtime.Common;
using Runtime.ETC;
using Runtime.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.InGameSystem
{
    // 코드에서 SoundType은 정적으로 3개로 고정이기 때문에
    // 구분하지 않고 분기문으로 처리, Sound 타입에 따라 정리
    public class SoundManager
    {
        private AudioSource[] _audioSources = new AudioSource[(int)Sound.Max];
        private Dictionary<string, AudioClip> _audioClips = new();

        private GameObject _soundRoot = null;
        private AudioClip _currentBGM;
        private float _bgmPlayTime = 0f;
        private bool _isReducing = false;
        private bool _isRestoring = false;

        public AudioSource BGM => _audioSources[(int)Sound.BGM];
        public AudioSource SFX => _audioSources[(int)Sound.SFX];
        public AudioSource Speech => _audioSources[(int)Sound.Speech];
        public AudioSource LuckyBGM => _audioSources[(int)Sound.LuckyBGM];

        public void Init()
        {
            if (_soundRoot == null)
            {
                _soundRoot = GameObject.Find("@SoundRoot");
                if (_soundRoot == null)
                {
                    _soundRoot = new GameObject("@SoundRoot");
                    Object.DontDestroyOnLoad(_soundRoot);

                    string[] soundTypeNames = System.Enum.GetNames(typeof(Sound));
                    for (int count = 0; count < soundTypeNames.Length - 1; count++)
                    {
                        GameObject soundObject = new(soundTypeNames[count]);
                        soundObject.transform.parent = _soundRoot.transform;
                        _audioSources[count] = Utils.GetOrAddComponent<AudioSource>(soundObject);
                    }

                    // 볼륨
                    UpdateBGMVolume();
                    SFX.volume = Managers.Sound.SFX.volume;
                    Speech.volume = Managers.Sound.SFX.volume;

                    // 루프
                    BGM.loop = true;
                    LuckyBGM.loop = true;

                    SettingsEvent.OnSettingsToggled += OnSettingsToggled;
                }
            }
        }

        private void OnSettingsToggled(bool isOpen)
        {
            if (isOpen)
            {
                SFX.Pause();
            }
            else
            {
                SFX.UnPause();
            }
        }

        public void Clear()
        {
            foreach (var audioSource in _audioSources)
            {
                audioSource.Stop();
                audioSource.clip = null;
            }

            _audioClips.Clear();
        }

        public bool Play(Sound type, string path, bool isContinue = false) // , float volume = 1.0f, float pitch = 1.0f
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            AudioSource audioSource = _audioSources[(int)type];

            if (path.Contains("Sound/") == false)
                path = $"Sound/{path}";

            AudioClip audioClip = GetAudioClip(path);
            if (audioClip == null)
                return false;

            // 이미 틀고 있는 브금 또 틀기 방지
            if (type == Sound.BGM || type == Sound.LuckyBGM)
            {
                if (_currentBGM == audioClip && BGM.isPlaying)
                {
                    Debug.Log("IsPlaying");
                    return false;
                }
                else
                {
                    _currentBGM = audioClip;
                }
            }
            
            if (type == Sound.SFX)
            {
                if (audioClip.length >= 1.0f)
                {
                    // Debug.Log($"SFX: {BGM.volume}");
                    _isReducing = true;
                    CoroutineRunner.Instance.StartCoroutine(RestoreBGMVolume(audioClip.length)); // 일정 시간 후 복구
                    UpdateBGMVolume();
                }
                    
                audioSource.PlayOneShot(audioClip);
                return true;
            }
            else if (type == Sound.Speech)
            {
                if (audioSource.isPlaying)
                    audioSource.Stop();

                audioSource.clip = audioClip;
                audioSource.Play();
                return true;
            }
            else if (type == Sound.BGM || type == Sound.LuckyBGM)
            {
                // Debug.Log($"BGM: {BGM.volume}");
                CheckBGMTime();
                StopBGM();
                
                audioSource.clip = audioClip;
                audioSource.time = (isContinue ? _bgmPlayTime : 0);
                audioSource.Play();
                return true;
            }

            return false;
        }

        public void UpdateBGMVolume()
        {
            float mul = _isReducing ? 0.5f : 1.0f;

            BGM.volume = Managers.Data.BgmVolume * mul;
            LuckyBGM.volume = Managers.Data.BgmVolume * mul;
        }

        private IEnumerator RestoreBGMVolume(float delay)
        {
            if (_isRestoring) yield break; // 이미 실행 중이면 중단
            _isRestoring = true;

            yield return new WaitForSeconds(delay);

            // Debug.Log($"RestoreBGMVolume 실행됨, SFX.isPlaying: {SFX.isPlaying}");

            // 모든 SFX가 끝날 때까지 대기
            while (SFX.isPlaying)
            {
                // Debug.Log("아직 SFX가 재생 중... 0.5초 후 다시 확인");
                yield return new WaitForSeconds(0.5f);
            }

            // 모든 SFX가 종료되었으므로 볼륨 복구
            _isReducing = false;
            Managers.Sound.UpdateBGMVolume();
            // Debug.Log($"볼륨 복구 완료: {BGM.volume}");

            _isRestoring = false;
        }

        private void CheckBGMTime()
        {
            if (BGM.isPlaying)
            {
                if (BGM.clip.name.Equals("Main_BGM") || BGM.clip.name.Equals("Main(Cave)_BGM"))
                    _bgmPlayTime = BGM.time;
            }
        }

        public void PlayRandomSpeech(string folder)
        {
            string path = $"Sound/{folder}";
            AudioClip audioClip = Managers.Resource.LoadRandom<AudioClip>(path);

            path += $"/{audioClip.name}";

            Play(Sound.Speech, path);
        }

        private AudioClip GetAudioClip(string path)
        {
            if (_audioClips.TryGetValue(path, out AudioClip audioClip))
            {
                return audioClip;
            }

            audioClip = Managers.Resource.Load<AudioClip>(path);
            _audioClips.Add(path, audioClip);
            return audioClip;
        }

        public void ResetPlayTime()
        {
            _bgmPlayTime = 0f;
        }

        public void StopAllSound()
        {
            StopBGM();
            StopSFX();
        }

        public void StopBGM()
        {
            BGM.Stop();
            LuckyBGM.Stop();
        }

        public void StopSFX()
        {
            SFX.Stop();
        }
    }
}