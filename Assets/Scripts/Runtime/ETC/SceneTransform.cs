using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.ETC
{
    public class SceneTransform : MonoBehaviour
    {
        private readonly string _connectionScene = "Connection";
        private readonly string _escapeScene = "Escape";
        private readonly float _translationDuration = 2f;
        private string _targetScene;
        private string _middleScene;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void BeforeConnection()
        {
            Managers.Data.InGameKeyBinder.PlayerInputDisable();
            Managers.Sound.StopAllSound();
            Managers.Sound.Play(Sound.SFX, "[CH1] SFX_Connection");
        }

        public void ConnectToScene(string targetScene)
        {
            // ConnectToScene 전에 BeforeConnection() 호출 필수

            _middleScene = _connectionScene;
            _targetScene = targetScene;
            StartCoroutine(nameof(TranslateScene));
        }

        public void EscapeFromScene(string targetScene)
        {
            Managers.Data.InGameKeyBinder.PlayerInputDisable();

            Managers.Sound.StopAllSound();
            // TODO: 효과음 교체
            Managers.Sound.Play(Sound.SFX, "Tmp_Escape");

            _middleScene = _escapeScene;
            _targetScene = targetScene;
            StartCoroutine(nameof(TranslateScene));
        }

        private IEnumerator TranslateScene()
        {
            // 비동기 방식을 쓰지 않으면 씬 로드나 언로드 중에 게임이 멈출 수 있다고 함
            Debug.Log("_targetScene: " + _targetScene);

            // 중간 씬 로드
            yield return SceneManager.LoadSceneAsync(_middleScene, LoadSceneMode.Additive);

            // 대기
            yield return new WaitForSeconds(_translationDuration);

            // 현재 씬 언로드
            Scene currentScene = SceneManager.GetActiveScene();
            yield return SceneManager.UnloadSceneAsync(currentScene);

            // 목표 씬 로드
            yield return SceneManager.LoadSceneAsync(_targetScene, LoadSceneMode.Additive);

            // 중간 씬 언로드
            yield return SceneManager.UnloadSceneAsync(_middleScene);

            // 여기서 처리
            Managers.Data.InGameKeyBinder.PlayerInputEnable();
        }
    }
}