using Runtime.InGameSystem;
using Runtime.Manager;
using UnityEngine;

namespace Runtime.Main.Runtime.ETC
{
    [System.Serializable] // Unity의 직렬화 지원
    public class DataCheater
    {
        public void LoadCheatData(string file, SceneSystem sceneSystem)
        {
            TextAsset jsonFile = Resources.Load<TextAsset>("Cheat/" + file); // (파일명).json 파일을 불러옴

            if (jsonFile == null)
            {
                Debug.LogError("JSON file not found!");
                return;
            }

            string jsonData = jsonFile.text;
            GameData data = JsonUtility.FromJson<GameData>(jsonData);
            Managers.Data.ChangeData(data);

            sceneSystem.LoadScene($"CH{Managers.Data.Chapter}");
        }
    }
}
