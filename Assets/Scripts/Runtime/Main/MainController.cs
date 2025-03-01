using Runtime.InGameSystem;
using Runtime.Main.Runtime.ETC;
using UnityEngine;

namespace Runtime.Main
{
    public class MainController : MonoBehaviour
    {
        [SerializeField] private SceneSystem _sceneSystem;
        private DataCheater _dataCheater = new();

        private void Start()
        {
            Managers.Data.InGameKeyBinder.GameControlReset();
        }

        public void NewGame()
        {
            Managers.Data.NewGame();
            _sceneSystem.LoadScene("CH1");
        }
        
        public void LoadGame()
        {
            Managers.Data.LoadGame();
            _sceneSystem.LoadScene($"CH{Managers.Data.Chapter}");
        }

        public void GoCh0()
        {
            Managers.Data.NewGame();
            _sceneSystem.LoadScene("CH0");
        }

        public void GoPacmom()
        {
            Managers.Data.NewGame();
            _sceneSystem.LoadScene("Pacmom");
        }
        
        public void GoSuperArio(string stage)
        {
            Managers.Data.NewGame();
            Managers.Data.CH2.ArioStage = stage;
            _sceneSystem.LoadScene("SuperArio");
        }

        public void GoCh3()
        {
            Managers.Data.NewGame();
            Managers.Data.Chapter = 3;
            Managers.Data.SaveGame();
            _sceneSystem.LoadScene("Title");
        }

        public void LoadCheatData(string file)
        {
            _dataCheater.LoadCheatData(file, _sceneSystem);
        }
    }
}