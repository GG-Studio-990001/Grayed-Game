using Runtime.InGameSystem;
using Runtime.Manager;
using UnityEngine;

namespace Runtime.Main
{
    namespace Runtime.ETC
    {
        public class DataCheater : MonoBehaviour
        {
            [SerializeField] private SceneSystem _sceneSystem;

            public void LoadCheatData(string file)
            {
                TextAsset jsonFile = Resources.Load<TextAsset>("Cheat/" + file); // (파일명).json 파일을 불러옴

                if (jsonFile == null)
                {
                    Debug.LogError("JSON file not found!");
                }

                string jsonData = jsonFile.text;
                GameData data = JsonUtility.FromJson<GameData>(jsonData);
                Managers.Data.ChangeData(data);

                _sceneSystem.LoadScene($"CH{Managers.Data.Chapter}");
            }
        }
    }
}