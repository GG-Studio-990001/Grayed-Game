using Runtime.InGameSystem;
using Runtime.Main.Runtime.ETC;
using UnityEngine;

namespace Runtime.Main
{
    public class MainController : MonoBehaviour
    {
        [SerializeField] private SceneSystem _sceneSystem;
        [SerializeField] private DataCheater _dataCheater;

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

        public void GoPacmom()
        {
            Managers.Data.NewGame();
            _sceneSystem.LoadScene("Pacmom");
        }
    }
}