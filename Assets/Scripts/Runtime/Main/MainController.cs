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
            Managers.Data.NewGame();
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
            Managers.Data.NewGame();
            _sceneSystem.LoadScene("Pacmom");
        }

        public void GoCH2()
        {
            Managers.Data.NewGame();
            _sceneSystem.LoadScene("CH2");
        }

        public void AfterIntro()
        {
            Managers.Data.NewGame();
            _testData.DataAfterIntro();
            Managers.Data.SaveGame();
            LoadGame();
        }

        public void AfterPacmom()
        {
            Managers.Data.NewGame();
            _testData.DataAfterPacmom();
            Managers.Data.SaveGame();
            LoadGame();
        }

        public void After3Match()
        {
            Managers.Data.NewGame();
            _testData.DataAfter3Match();
            Managers.Data.SaveGame();
            LoadGame();
        }
    }
}
