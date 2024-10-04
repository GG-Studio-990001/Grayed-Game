using Runtime.Middle;
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
        private EscapeController _escapeController;
        private static bool _isInitialized = false;

        private void Awake()
        {
            if (_isInitialized)
            {
                Destroy(gameObject);
                return;
            }

            _isInitialized = true;
            DontDestroyOnLoad(gameObject);
        }

        public void ConnectToScene(string targetScene)
        {
            // 함수 호출 전 효과음 출력, 인풋 막기 필요
            _middleScene = _connectionScene;
            _targetScene = targetScene;
            StartCoroutine(nameof(TranslateScene));
        }

        public void EscapeFromScene(string targetScene)
        {
            Managers.Data.InGameKeyBinder.PlayerInputDisable();

            Managers.Sound.StopAllSound();

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

            // 탈출 연출 시에는 비디오가 로드되는 동안 더 대기
            if (_middleScene == _escapeScene)
            {
                _escapeController = FindObjectOfType<EscapeController>();
                if (_escapeController is null)
                {
                    Debug.LogError("_escapeController 못찾음");
                }
                else
                {
                    Debug.Log("_escapeController 찾음");
                }

                while (!_escapeController.IsDirectionStarted)
                {
                    yield return null;
                }
            }

            // 대기
            yield return new WaitForSeconds(_translationDuration);

            // 현재 씬 언로드
            Scene currentScene = SceneManager.GetActiveScene();
            yield return SceneManager.UnloadSceneAsync(currentScene);

            // 목표 씬 로드
            yield return SceneManager.LoadSceneAsync(_targetScene, LoadSceneMode.Additive);

            // 중간 씬 언로드
            yield return SceneManager.UnloadSceneAsync(_middleScene);

            Managers.Data.InGameKeyBinder.PlayerInputEnable();
        }

        public void ConnectDirection()
        {
            _middleScene = _connectionScene;
            StartCoroutine(nameof(TranslateDirection));
        }

        private IEnumerator TranslateDirection()
        {
            // 중간 씬 로드
            yield return SceneManager.LoadSceneAsync(_middleScene, LoadSceneMode.Additive);

            // 대기
            yield return new WaitForSeconds(_translationDuration);

            // 중간 씬 언로드
            yield return SceneManager.UnloadSceneAsync(_middleScene);

            Managers.Data.InGameKeyBinder.PlayerInputEnable();
        }
    }
}