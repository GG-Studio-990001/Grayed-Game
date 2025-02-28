using Runtime.Middle;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.ETC
{
    public class SceneTransform : MonoBehaviour
    {
        private static SceneTransform _instance;

        private readonly string _connectionScene = "Connection";
        private readonly string _escapeScene = "Escape";
        private readonly float _translationDuration = 2f;
        private string _targetScene;
        private string _middleScene;
        private EscapeController _escapeController;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ConnectToScene(string targetScene, bool disablePlayerInput = false)
        {
            _middleScene = _connectionScene;
            _targetScene = targetScene;
            StartCoroutine(nameof(TranslateScene), disablePlayerInput);
        }

        public void EscapeFromScene(string targetScene, bool disablePlayerInput = false)
        {
            Managers.Data.InGameKeyBinder.PlayerInputDisable();
            Managers.Sound.StopAllSound();

            _middleScene = _escapeScene;
            _targetScene = targetScene;
            StartCoroutine(nameof(TranslateScene), disablePlayerInput);
        }

        private IEnumerator TranslateScene(bool disablePlayerInput = false)
        {
            Debug.Log("_targetScene: " + _targetScene);

            yield return SceneManager.LoadSceneAsync(_middleScene, LoadSceneMode.Additive);

            if (_middleScene == _escapeScene)
            {
                _escapeController = FindObjectOfType<EscapeController>();
                if (_escapeController == null)
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

            yield return new WaitForSeconds(_translationDuration);

            // 현재 활성 씬 변경
            Scene currentScene = SceneManager.GetActiveScene();
            yield return SceneManager.UnloadSceneAsync(currentScene);

            yield return SceneManager.LoadSceneAsync(_targetScene, LoadSceneMode.Additive);

            // **강제로 활성 씬 변경**
            Scene newScene = SceneManager.GetSceneByName(_targetScene);
            if (newScene.IsValid())
            {
                SceneManager.SetActiveScene(newScene);
                Debug.Log("Set active scene: " + _targetScene);
            }
            else
            {
                Debug.LogError("Failed to set active scene!");
            }

            // **카메라 강제 전환**
            yield return new WaitForSeconds(0.1f); // 씬 언로드 전에 딜레이
            Camera newCamera = FindObjectOfType<Camera>();
            if (newCamera != null)
            {
                newCamera.gameObject.SetActive(true);
                newCamera.depth = 1; // Escape 씬 카메라의 depth를 높게 설정
                Debug.Log("New camera activated: " + newCamera.name);
            }
            else
            {
                Debug.LogError("No camera found in new scene!");
            }

            // UI 카메라의 depth를 낮게 설정
            Camera uiCamera = FindObjectOfType<Camera>(true); // 비활성 카메라 포함
            if (uiCamera != null)
            {
                uiCamera.depth = 0; // UI 카메라의 depth를 낮게 설정
                Debug.Log("UI camera depth set to: " + uiCamera.depth);
            }

            yield return SceneManager.UnloadSceneAsync(_middleScene);

            if (!disablePlayerInput)
            {
                Managers.Data.InGameKeyBinder.PlayerInputEnable();
            }
        }

        public void ConnectDirection()
        {
            _middleScene = _connectionScene;
            StartCoroutine(nameof(TranslateDirection));
        }

        private IEnumerator TranslateDirection()
        {
            yield return SceneManager.LoadSceneAsync(_middleScene, LoadSceneMode.Additive);
            yield return new WaitForSeconds(_translationDuration);
            yield return SceneManager.UnloadSceneAsync(_middleScene);

            Managers.Data.InGameKeyBinder.PlayerInputEnable();
        }
    }
}