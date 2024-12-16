using Runtime.Input;
using System;
using UnityEngine;
using Runtime.Data;
using SLGDefines;

namespace Runtime.Manager
{
    [Serializable]
    public class GameData
    {
        public string Version;
        public int Chapter;
        public float BgmVolume;
        public float SfxVolume;
        public bool isFullScreen;
        public CH1Data CH1;
        public CH2Data CH2;

        public GameData()
        {
            Version = "";
            Chapter = 1;
            BgmVolume = 0.5f;
            SfxVolume = 0.5f;
            isFullScreen = true;
            CH1 = new CH1Data();
            CH2 = new CH2Data();
        }
    }
    
    [DefaultExecutionOrder(-100)]
    public class DataManager
    {
        private GameData SaveData { get { return _gameData; } set { _gameData = value; } }
        public InGameKeyBinder InGameKeyBinder { get; set; }
        
        private GameData _gameData = new();
        private GameOverControls _gameOverControls;

        #region properties
        public string Version { get { return _gameData.Version; } set { _gameData.Version = value; } }
        public int Chapter { get { return _gameData.Chapter; } set { _gameData.Chapter = value; } }
        public float BgmVolume
        {
            get { return _gameData.BgmVolume; }
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                _gameData.BgmVolume = value;
                Managers.Sound.BGM.volume = value;
            }
        }
        
        public float SfxVolume
        {
            get { return _gameData.SfxVolume; }
            set 
            { 
                value = Mathf.Clamp(value, 0, 1);
                _gameData.SfxVolume = value;
                Managers.Sound.SFX.volume = value;
            }
        }

        public bool IsFullscreen
        {
            get { return _gameData.isFullScreen; }
            set { _gameData.isFullScreen = value; }
        }

        public CH1Data CH1 { get { return _gameData.CH1; } set { _gameData.CH1 = value; } }

        public CH2Data CH2 { get { return _gameData.CH2; } set { _gameData.CH2 = value; } }
        #endregion

        public void Init()
        {
            _gameOverControls = new GameOverControls();
            InGameKeyBinder = new InGameKeyBinder(_gameOverControls);
        }
        
        public void SaveGame()
        {
            string path = Application.persistentDataPath + "/saveData.json";

            string jsonStr = JsonUtility.ToJson(Managers.Data.SaveData);
            System.IO.File.WriteAllText(path, jsonStr);
            Debug.Log($"Save Game Completed : {path}");
        }
        
        public bool LoadGame()
        {
            string path = Application.persistentDataPath + "/saveData.json";

            if (System.IO.File.Exists(path) == false)
            {
                return false;
            }

            string jsonStr = System.IO.File.ReadAllText(path);
            GameData data = JsonUtility.FromJson<GameData>(jsonStr);
            if (data == null)
            {
                return false;
            }
            
            Managers.Data.SaveData = data;
            Screen.fullScreen = data.isFullScreen;
            Managers.Sound.BGM.volume = data.BgmVolume;
            Managers.Sound.SFX.volume = data.SfxVolume;
            Debug.Log($"Load Game Completed : {path}");
            Debug.Log(Managers.Data.CH1.Scene+"."+ Managers.Data.CH1.SceneDetail);
            return true;
        }

        public void ChangeData(GameData data)
        {
            Managers.Data.SaveData = data;
            SaveGame();
        }

        public void NewGame()
        {
            _gameData = new GameData { Version = Application.version };
            SaveGame();
            Managers.Sound.ResetPlayTime();
        }
    }
}