using Runtime.ETC;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Runtime.Middle
{
    public class ConnectionController : MonoBehaviour
    {
        [SerializeField] private Volume _volume;
        private SceneTransform _sceneTransform;
        private string _sceneName;

        private void Start()
        {
            _sceneTransform = FindObjectOfType<SceneTransform>();
            if (_sceneTransform == null)
                Debug.Log("_sceneTransform is null");
        }

        public void ConnectScene(string sceneName)
        {
            // 접속
            _sceneName = sceneName;
            //TODO: 빌드용 임시 씬로드 호출 삭제 요망
            if(sceneName == "SuperArio")
                SceneManager.LoadScene(sceneName);
            else
                StartCoroutine(nameof(ActiveGlitch), true);
        }

        public void ReverseConnection()
        {
            // 역접속
            StartCoroutine(nameof(ActiveGlitch), false);
        }

        IEnumerator ActiveGlitch(bool isConnection)
        {
            if (isConnection)
                Managers.Sound.Play(Sound.SFX, "Connection_SFX");
            else
                Managers.Sound.Play(Sound.SFX, "ReverseConnection_SFX_01");

            Managers.Data.InGameKeyBinder.PlayerInputDisable();
            _volume.weight = 0.59f;
            yield return new WaitForSeconds(1f);
            _volume.weight = 0;
            Managers.Data.InGameKeyBinder.PlayerInputEnable();

            if (isConnection)
            {
                //TODO: sceneTransform에서 MissingReference 에러 발생
                _sceneTransform.ConnectToScene(_sceneName);
            }
        }
    }
}