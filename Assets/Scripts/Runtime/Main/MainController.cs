using Runtime.InGameSystem;
using UnityEngine;

namespace Runtime.Main
{
    public class MainController : MonoBehaviour
    {
        [SerializeField] private SceneSystem sceneSystem;

        private void Start()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
        }

        // Hard Cordings

        public void NewGame()
        {
            Managers.Data.NewGame();
            sceneSystem.LoadScene("CH1");
        }
        
        public void LoadGame()
        {
            Managers.Data.LoadGame();
            sceneSystem.LoadScene("CH1");
        }

        public void GoPacmom()
        {
            Managers.Data.NewGame();
            sceneSystem.LoadScene("Pacmom");
        }

        public void GoCH2()
        {
            Managers.Data.NewGame();
            sceneSystem.LoadScene("CH2");
        }

        public void AfterIntro()
        {
            Managers.Data.NewGame();
            Managers.Data.Scene = 1;
            Managers.Data.SceneDetail = 1;
            Managers.Data.SaveGame();
            Managers.Data.LoadGame();
            sceneSystem.LoadScene("CH1");
        }

        public void AfterPacmom()
        {
            Managers.Data.NewGame();
            Managers.Data.Scene = 1;
            Managers.Data.SceneDetail = 1;
            Managers.Data.MeetLucky = true;
            Managers.Data.IsPacmomPlayed = true;
            Managers.Data.IsPacmomCleared = true;
            Managers.Data.PacmomCoin = 300;
            Managers.Data.SaveGame();
            Managers.Data.LoadGame();
            sceneSystem.LoadScene("CH1");
        }

        public void After3Match()
        {
            Managers.Data.NewGame();
            Managers.Data.Scene = 3;
            Managers.Data.SceneDetail = 1;
            Managers.Data.MeetLucky = true;
            Managers.Data.IsPacmomPlayed = true;
            Managers.Data.IsPacmomCleared = true;
            Managers.Data.PacmomCoin = 300;
            Managers.Data.Is3MatchEntered = true;
            Managers.Data.Is3MatchCleared = true;
            Managers.Data.SaveGame();
            Managers.Data.LoadGame();
            sceneSystem.LoadScene("CH1");
        }

        public void AfterSLG()
        {
            Managers.Data.NewGame();

        }
    }
}
