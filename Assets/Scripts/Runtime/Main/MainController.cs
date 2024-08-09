using Runtime.InGameSystem;
using UnityEngine;

namespace Runtime.Main
{
    public class MainController : MonoBehaviour
    {
        [SerializeField] private SceneSystem _sceneSystem;
        private TestData _testData;

        private void Start()
        {
            _testData = new TestData();
            Managers.Data.InGameKeyBinder.GameControlReset();
        }

        public void NewGame()
        {
            Managers.Data.NewGame(Application.version);
            _sceneSystem.LoadScene("CH1");
        }
        
        public void LoadGame()
        {
            Managers.Data.LoadGame();

            if (Managers.Data.Chapter == 1)
                _sceneSystem.LoadScene("CH1");
            else if (Managers.Data.Chapter == 2)
                _sceneSystem.LoadScene("CH2");
        }

        public void GoPacmom()
        {
            Managers.Data.NewGame(Application.version);
            _sceneSystem.LoadScene("Pacmom");
        }

        public void GoCH2()
        {
            Managers.Data.NewGame(Application.version);
            _sceneSystem.LoadScene("CH2");
        }

        public void AfterIntro()
        {
            Managers.Data.NewGame(Application.version);
            _testData.DataAfterIntro();
            Managers.Data.SaveGame();
            LoadGame();
        }

        public void AfterPacmom()
        {
            Managers.Data.NewGame(Application.version);
            _testData.DataAfterPacmom();
            Managers.Data.SaveGame();
            LoadGame();
        }

        public void After3Match()
        {
            Managers.Data.NewGame(Application.version);
            _testData.DataAfter3Match();
            Managers.Data.SaveGame();
            LoadGame();
        }
    }
}
