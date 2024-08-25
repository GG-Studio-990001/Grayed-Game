using Runtime.ETC;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Runtime.Middle
{
    public class ConnectionController : MonoBehaviour
    {
        [SerializeField] private Volume _volume;
        [SerializeField] private SceneTransform _sceneTransform;
        private LimitlessGlitch3 _glitch;
        private string _sceneName;

        private void Start()
        {
            if (_volume != null)
            {
                _volume.profile.TryGet(out _glitch);
            }
        }

        public void ConnectScene(string sceneName)
        {
            // 접속
            _sceneName = sceneName;
            StartCoroutine(nameof(ActiveGlitch), true);
        }

        public void ReverseConnection()
        {
            // 역접속
            StartCoroutine(nameof(ActiveGlitch), false);
        }

        IEnumerator ActiveGlitch(bool isConnection)
        {
            CheckGlitch();

            if (isConnection)
                Managers.Sound.Play(Sound.SFX, "Connection_SFX");
            else
                Managers.Sound.Play(Sound.SFX, "ReverseConnection_SFX_01");

            Managers.Data.InGameKeyBinder.PlayerInputDisable();
            _glitch.enable.value = true;
            yield return new WaitForSeconds(1f);
            _glitch.enable.value = false;
            Managers.Data.InGameKeyBinder.PlayerInputEnable();

            if (isConnection)
                _sceneTransform.ConnectToScene(_sceneName);
        }

        private void CheckGlitch()
        {
            if (_glitch == null)
                Debug.Log("없음");
            else
                Debug.Log(_glitch);
        }
    }
}