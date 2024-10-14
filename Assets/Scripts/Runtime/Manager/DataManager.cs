using Runtime.CH1.SubB._3_Match;
using Runtime.Input;
using System;
using Runtime.ETC;
using UnityEngine;
using SLGDefines;

namespace Runtime.Manager
{
    // TODO: CH1Data 묶기

    [Serializable]
    public class CH2Data
    {
        public int Turn;
        public string Location;

        public CH2Data()
        {
            Turn = 0;
            Location = "";
        }
    }

    [Serializable]
    public class GameData
    {
        public string Version;
        public int Chapter;
        public float BgmVolume;
        public float SfxVolume;
        #region CH1
        // Progress
        public int Stage;
        public int Scene;
        public int SceneDetail; // 씬 세부 진행도
        // CH1
        public bool MeetLucky; // have로 변경
        // 3match
        public bool Is3MatchEntered;
        public bool Is3MatchCleared;
        public ThreeMatchPuzzleStageData[] ThreeMatchPuzzleStageData;
        // Pacmom
        public bool IsPacmomPlayed;
        public bool IsPacmomCleared;
        public int PacmomCoin;
        // SLG
        public long[] SLGConstructionBeginTime;
        public SLGProgress SLGProgressData;
        public SLGBuildingProgress[] SLGBuildingProgressData;
        public int SLGWoodCount;
        public int SLGStoneCount;
        // Else
        public int TranslatorCount;
        #endregion
        public CH2Data CH2;

        public GameData()
        {
            Version = "";
            Chapter = 1;
            BgmVolume = 0.5f;
            SfxVolume = 0.5f;
            #region CH1
            // Progress
            Stage = 1;
            Scene = 0;
            SceneDetail = 0;
            // CH1
            MeetLucky = false;
            Is3MatchEntered = false;
            // 3match
            Is3MatchCleared = false;
            ThreeMatchPuzzleStageData = new ThreeMatchPuzzleStageData[4];
            for (int i = 0; i < ThreeMatchPuzzleStageData.Length; i++)
            {
                ThreeMatchPuzzleStageData[i] = new ThreeMatchPuzzleStageData();
                ThreeMatchPuzzleStageData[i].isClear = false;
                ThreeMatchPuzzleStageData[i].jewelryPositions = new System.Collections.Generic.List<Vector2>();
            }
            // Pacmom
            IsPacmomPlayed = false;
            IsPacmomCleared = false;
            PacmomCoin = 0;
            // SLG
            SLGProgressData = SLGProgress.None;
            SLGWoodCount = 0;
            SLGStoneCount = 0;
            SLGConstructionBeginTime = new long[(int)SLGBuildingType.Max];
            SLGBuildingProgressData = new SLGBuildingProgress[(int)SLGBuildingType.Max];
            // Else
            TranslatorCount = 0;
            #endregion
            CH2 = new CH2Data();
        }
    }
    
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
            set { Mathf.Clamp(value, 0, 1); _gameData.BgmVolume = value; Managers.Sound.BGM.volume = value; }
        }
        public float SfxVolume
        {
            get { return _gameData.SfxVolume; }
            set { Mathf.Clamp(value, 0, 1); _gameData.SfxVolume = value; Managers.Sound.SFX.volume = value; }
        }

        #region CH1
        public int Stage { get { return _gameData.Stage; } set { _gameData.Stage = value; } }
        public int Scene { get { return _gameData.Scene; } set { _gameData.Scene = value; } }
        public int SceneDetail { get { return _gameData.SceneDetail; } set { _gameData.SceneDetail = value; } }

        // CH1
        public bool MeetLucky { get { return _gameData.MeetLucky; } set { _gameData.MeetLucky = value; } }
        public bool Is3MatchEntered { get { return _gameData.Is3MatchEntered; } set { _gameData.Is3MatchEntered = value; } }
        
        // 3match
        public bool Is3MatchCleared { get { return _gameData.Is3MatchCleared; } set { _gameData.Is3MatchCleared = value; } }
        public ThreeMatchPuzzleStageData[] ThreeMatchPuzzleStageData { get { return _gameData.ThreeMatchPuzzleStageData; } set { _gameData.ThreeMatchPuzzleStageData = value; } }

        // Pacmom
        public bool IsPacmomPlayed { get { return _gameData.IsPacmomPlayed; } set { _gameData.IsPacmomPlayed = value; } }
        public bool IsPacmomCleared { get { return _gameData.IsPacmomCleared; } set { _gameData.IsPacmomCleared = value; } }
        public int PacmomCoin { get { return _gameData.PacmomCoin; } set { _gameData.PacmomCoin = value; } }

        // SLG
        public long[] SLGConstructionBeginTime { get { return _gameData.SLGConstructionBeginTime; } set { _gameData.SLGConstructionBeginTime = value; } }
        public SLGProgress SLGProgressData { get { return _gameData.SLGProgressData; } set { _gameData.SLGProgressData = value; } }
        public SLGBuildingProgress[] SLGBuildingProgressData { get { return _gameData.SLGBuildingProgressData; } set { _gameData.SLGBuildingProgressData = value; } }
        public int SLGWoodCount { get { return _gameData.SLGWoodCount; } set { _gameData.SLGWoodCount = value; } }
        public int SLGStoneCount { get { return _gameData.SLGStoneCount; } set { _gameData.SLGStoneCount = value; } }

        // Else
        public int TranslatorCount { get { return _gameData.TranslatorCount; } set { _gameData.TranslatorCount = value; } }
        #endregion

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
            Debug.Log($"Load Game Completed : {path}");
            Debug.Log(Managers.Data.Scene+"."+ Managers.Data.SceneDetail);
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
        }
    }
}