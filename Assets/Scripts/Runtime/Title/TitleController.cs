using Runtime.InGameSystem;
using UnityEngine;
using Sound = Runtime.ETC.Sound;

namespace Runtime.CH1.Title
{
    public class TitleController : MonoBehaviour
    {
        [SerializeField] private SceneSystem _sceneSystem;

        private void Start()
        {
            Managers.Sound.Play(Sound.BGM, "Title_BGM_04");
        }

        public void StopBGM()
        {
            Managers.Sound.StopBGM();
        }

        public void LoadSceneByData()
        {
            _sceneSystem.LoadScene($"CH{Managers.Data.Chapter}");
        }
    }
}