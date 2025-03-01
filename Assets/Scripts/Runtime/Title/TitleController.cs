using Runtime.InGameSystem;
using UnityEngine;
using Sound = Runtime.ETC.Sound;

namespace Runtime.CH1.Title
{
    public class TitleController : MonoBehaviour
    {
        [SerializeField] private SceneSystem _sceneSystem;
        [SerializeField] private FadeController _fadeController;
        [SerializeField] private GameObject[] _chObjs = new GameObject[3];

        private void Start()
        {
            foreach (GameObject obj in _chObjs)
                obj.SetActive(false);

            int ch = Managers.Data.Chapter;

            switch(ch)
            {
                case 0:
                case 1:
                    _fadeController.StartFadeIn();
                    _chObjs[0].SetActive(true);
                    Managers.Sound.Play(Sound.BGM, $"Title/Title_BGM_CH1");
                    break;
                case 2:
                    _chObjs[ch - 1].SetActive(true);
                    Managers.Sound.Play(Sound.BGM, $"Title/Title_BGM_CH{ch}");
                    break;
                case 3:
                    _chObjs[ch - 1].SetActive(true);
                    Managers.Sound.Play(Sound.BGM, $"Mamago_BGM_1");
                    break;
            }
        }

        public void StopBGM()
        {
            Managers.Sound.StopBGM();
        }

        public void LoadSceneByData()
        {
            // CH0 또는 CH1
            _sceneSystem.LoadScene($"CH{Managers.Data.Chapter}");
        }
    }
}