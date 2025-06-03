using Runtime.Input;
using System;
using UnityEngine;
using Runtime.Data;

namespace Runtime.Manager
{
    [Serializable]
    public class GameData
    {
        public string Version;
        public int Chapter;
        public SettingData Setting;
        public CommonData Common;
        public CH1Data CH1;
        public CH2Data CH2;
        public CH3Data CH3;

        public GameData()
        {
            Version = "";
            Chapter = 0;
            Setting = new SettingData();
            Common = new CommonData();
            CH1 = new CH1Data();
            CH2 = new CH2Data();
            CH3 = new CH3Data();
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
            get { return _gameData.Setting.BgmVolume; }
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                _gameData.Setting.BgmVolume = value;
                Managers.Sound.BGM.volume = value;
            }
        }

        public float SfxVolume
        {
            get { return _gameData.Setting.SfxVolume; }
            set
            {
                value = Mathf.Clamp(value, 0, 1);
                _gameData.Setting.SfxVolume = value;
                Managers.Sound.SFX.volume = value;
            }
        }

        public bool IsFullscreen
        {
            get { return _gameData.Setting.isFullScreen; }
            set { _gameData.Setting.isFullScreen = value; }
        }

        public CommonData Common { get { return _gameData.Common; } set { _gameData.Common = value; } }

        public CH1Data CH1 { get { return _gameData.CH1; } set { _gameData.CH1 = value; } }

        public CH2Data CH2 { get { return _gameData.CH2; } set { _gameData.CH2 = value; } }

        public CH3Data CH3 { get { return _gameData.CH3; } set { _gameData.CH3 = value; } }
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
            Screen.fullScreen = data.Setting.isFullScreen;
            Managers.Sound.BGM.volume = data.Setting.BgmVolume;
            Managers.Sound.SFX.volume = data.Setting.SfxVolume;
            Debug.Log($"Load Game Completed : {path}");

            Debug.Log($"Chapter: {Managers.Data.Chapter}");
            switch (Managers.Data.Chapter)
            {
                case 0:
                case 1:
                    Debug.Log($"{Managers.Data.CH1.Scene}.{Managers.Data.CH1.SceneDetail}");
                    break;
                case 2:
                    Debug.Log($"{Managers.Data.CH2.Turn}");
                    break;
                case 3:
                    Debug.Log($"Dancepace High Score: {Managers.Data.CH3.Dancepace.HighScore}");
                    break;
            }
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