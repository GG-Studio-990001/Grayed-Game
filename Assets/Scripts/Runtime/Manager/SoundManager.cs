using Runtime.Common;
using Runtime.ETC;
using Runtime.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.InGameSystem
{
    // 코드에서 SoundType은 정적으로 4개로 고정이기 때문에
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

        public void UpdateBGMVolume(float t = 1.0f)
        {
            float mul = _isReducing ? 0.5f : 1.0f;
            float volume = Managers.Data.BgmVolume * mul * t;

            BGM.volume = volume;
            LuckyBGM.volume = volume;
        }

        private IEnumerator RestoreBGMVolume(float sfxDuration)
        {
            if (_isRestoring) yield break;
            _isRestoring = true;

            float fadeOutDuration = 0.5f;
            float fadeInDuration = 0.5f;

            // 페이드 아웃 (0.5초)
            _isReducing = true;
            for (float t = 0f; t < fadeOutDuration; t += Time.deltaTime)
            {
                float lerp = Mathf.Lerp(1f, 0.5f, t / fadeOutDuration);
                UpdateBGMVolume(lerp);
                yield return null;
            }
            UpdateBGMVolume(0.5f);

            // SFX 종료 0.25초 전까지 대기 (페이드 인 시작 시점까지)
            float waitBeforeFadeIn = sfxDuration - fadeInDuration / 2f - fadeOutDuration;
            if (waitBeforeFadeIn > 0f)
                yield return new WaitForSeconds(waitBeforeFadeIn);

            // 페이드 인 (0.5초)
            for (float t = 0f; t < fadeInDuration; t += Time.deltaTime)
            {
                float lerp = Mathf.Lerp(0.5f, 1f, t / fadeInDuration);
                UpdateBGMVolume(lerp);
                yield return null;
            }
            UpdateBGMVolume(1f);

            _isReducing = false;
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

        public void PauseAllSound()
        {
            SFX.Pause();
            BGM.Pause();
            LuckyBGM.Pause();
        }

        public void UnPauseAllSound()
        {
            SFX.UnPause();
            BGM.UnPause();
            LuckyBGM.UnPause();
        }
    }
}