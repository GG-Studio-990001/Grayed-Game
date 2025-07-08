using UnityEngine;
using Runtime.CH3.Dancepace;
using Runtime.ETC;

namespace Runtime.CH3.Dancepace
{
    public class DPTimeline : MonoBehaviour
    {
        [SerializeField]
        private GameFlowManager _gameFlowManager;
        [SerializeField]
        private GameObject _openingUI;
        [SerializeField]
        private GameObject _ingameUI;
        [SerializeField]
        private GameObject[] _dialogueRunner = new GameObject[2];
        [SerializeField]
        private GameObject[] _timeline = new GameObject[2];

        private void Start()
        {
            ControlDisable();
        }

        public void OpeningFinished()
        {
            _openingUI.SetActive(false);

            for (int i = 0; i < _timeline.Length; i++)
                _timeline[i].SetActive(false);

            _dialogueRunner[0].SetActive(false);
            //_dialogueRunner[1].SetActive(true);

            ControlEnable();
            _gameFlowManager.StartGame();
        }

        public void ControlEnable()
        {
            Managers.Data.InGameKeyBinder.PlayerInputEnable();
        }

        public void ControlDisable()
        {
            Managers.Data.InGameKeyBinder.PlayerInputDisable();
        }

        public void PlayTimeline1SFX()
        {
            Managers.Sound.Play(Sound.SFX, "Pacmom/Pacmom_SFX_14");
        }

        public void PlayTimeline2SFX()
        {
            Managers.Sound.Play(Sound.SFX, "Pacmom/Pacmom_BGM_03");
        }

        public void PlayTimeline3SFX()
        {
            Managers.Sound.Play(Sound.SFX, "Pacmom/Pacmom_BGM_04");
        }
    }
}
