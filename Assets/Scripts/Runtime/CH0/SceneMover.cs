using Runtime.ETC;
using Runtime.InGameSystem;
using UnityEngine;

namespace Runtime.CH0
{
    public class SceneMover : MonoBehaviour
    {
        [SerializeField] private SceneSystem _sceneSystem;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(GlobalConst.PlayerStr))
                return;

            Managers.Data.Chapter = 1;
            _sceneSystem.LoadSceneWithFade("CH1");
        }
    }
}