using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransform : MonoBehaviour
{
    private readonly string _connectionScene = "ConnectionScene";
    private readonly string _escapeScene = "EscapeScene";
    private string _targetScene;
    private string _middleScene;

    public void ConnectToScene(string targetScene)
    {
        _middleScene = _connectionScene;
        _targetScene = targetScene;
        StartCoroutine(nameof(TranslateScene));
    }

    public void EscapeFromScene(string targetScene)
    {
        _middleScene = _escapeScene;
        _targetScene = targetScene;
        StartCoroutine(nameof(TranslateScene));
    }

    private IEnumerator TranslateScene(string targetScene)
    {
        // 비동기 방식을 쓰지 않으면 씬 로드나 언로드 중에 게임이 멈출 수 있다고 함

        // 중간 씬 로드
        yield return SceneManager.LoadSceneAsync(_middleScene, LoadSceneMode.Additive);

        // 현재 씬 언로드
        Scene currentScene = SceneManager.GetActiveScene();
        yield return SceneManager.UnloadSceneAsync(currentScene);

        // 목표 씬 로드
        yield return SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);

        // 중간 씬 언로드
        yield return SceneManager.UnloadSceneAsync(_middleScene);
    }
}
