using Runtime.InGameSystem;
using UnityEngine;
using Sound = Runtime.ETC.Sound;

namespace Runtime.CH1.Title
{
    public class TitleController : MonoBehaviour
    {
        [SerializeField] private SceneSystem _sceneSystem;
        [SerializeField] private GameObject _ch1Obj;
        [SerializeField] private GameObject _ch2Obj;

        private void Start()
        {
            _ch1Obj.SetActive(false);
            _ch2Obj.SetActive(false);

            switch (Managers.Data.Chapter)
            {
                case 0:
                case 1:
                    Managers.Sound.Play(Sound.BGM, "Title/Title_BGM_CH1");
                    _ch1Obj.SetActive(true);
                    break;
                case 2:
                case 3:
                    Managers.Sound.Play(Sound.BGM, "Title/Title_BGM_CH2");
                    _ch2Obj.SetActive(true);
                    break;
            }
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