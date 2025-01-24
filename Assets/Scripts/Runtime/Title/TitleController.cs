using Runtime.InGameSystem;
using UnityEngine;
using Sound = Runtime.ETC.Sound;

namespace Runtime.CH1.Title
{
    public class TitleController : MonoBehaviour
    {
        [SerializeField] private SceneSystem _sceneSystem;
        [SerializeField] private GameObject[] _chObjs = new GameObject[3];

        private void Start()
        {
            foreach (GameObject obj in _chObjs)
                obj.SetActive(false);

            int ch = Managers.Data.Chapter;

            if (ch > 3) return;

            _chObjs[ch - 1].SetActive(true);

            if (ch > 2) return;

            Managers.Sound.Play(Sound.BGM, $"Title/Title_BGM_CH{ch}");
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