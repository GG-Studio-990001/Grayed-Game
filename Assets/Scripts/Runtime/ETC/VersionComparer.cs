using System.IO;
using UnityEngine;

namespace Runtime.ETC
{
    public class VersionComparer : MonoBehaviour
    {
        private string _dataPath;

        void Start()
        {
            _dataPath = Application.persistentDataPath + "/saveData.json";
            // CheckGameData();
        }

        void CheckGameData()
        {
            bool isSameVersion = false;

            if (File.Exists(_dataPath))
            {
                if (Managers.Data.Version != null)
                {
                    if (Managers.Data.Version == Application.version)
                    {
                        isSameVersion = true;
                    }
                }
            }

            if (isSameVersion)
            {
                Managers.Data.LoadGame();
            }
            else
            {
                Managers.Data.NewGame();
            }
        }
    }
}