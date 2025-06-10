using UnityEngine;
using System;
using System.Collections.Generic;

namespace Runtime.CH3.Dancepace
{
    [Serializable]
    public class PoseData
    {
        public string poseId;
        public string poseName;
        public KeyCode inputKey;
        public AudioClip poseSound;
        public Sprite poseImage;
        public string poseDirection; // Up, Down, Left, Right
    }

    [Serializable]
    public class BeatData
    {
        public string poseId;
        public float timing;
        public float restTime;
        public bool isPreview;
        public bool isPlay;
        public bool isRestBeat;

        public BeatData(string poseId, float timing, float restTime, bool isPreview = false, bool isPlay = false, bool isRestBeat = false)
        {
            this.poseId = poseId;
            this.timing = timing;
            this.restTime = restTime;
            this.isPreview = isPreview;
            this.isPlay = isPlay;
            this.isRestBeat = isRestBeat;
        }
    }

    [Serializable]
    public class WaveData
    {
        public string waveId;
        public float duration;
        public bool isRehearsal;
        public string waveMainBGM;
        public List<BeatData> beats;

        public WaveData()
        {
            beats = new List<BeatData>();
        }
    }

    [Serializable]
    public class GameConfig
    {
        public float limitTime;
        public int waveForCount;
        public string waveMainBGM;
        public int lifeCount;
        public int waveClearCoin;
        public int greatCoin;
        public int goodCoin;
        public int badCoin;
        public float greatTimingWindow;
        public float goodTimingWindow;
    }

    [Serializable]
    public class DancepaceData
    {
        // 게임 설정 데이터
        public GameConfig gameConfig;
        public List<WaveData> waveDataList;
        
        // 플레이어 진행 데이터
        public int HighScore;
        public int TotalCoins;
        public bool[] UnlockedPoses;
        public bool[] CompletedWaves;
        public float BestAccuracy;
        public int MaxCombo;
        
        // 현재 게임 상태
        public int CurrentWave;
        public int CurrentLife;
        public float CurrentTime;
        public int CurrentScore;
        public int CurrentCombo;
        public float CurrentAccuracy;
        public bool IsRehearsalMode;
        public int RehearsalCount;

        public DancepaceData()
        {
            // 게임 설정 초기화
            gameConfig = new GameConfig
            {
                limitTime = 60f,
                waveForCount = 6,
                waveMainBGM = "-",
                lifeCount = 3,
                waveClearCoin = 15,
                greatCoin = 3,
                goodCoin = 2,
                badCoin = 0,
                greatTimingWindow = 0.1f,
                goodTimingWindow = 0.2f
            };
            
            waveDataList = new List<WaveData>();
            
            // 플레이어 데이터 초기화
            HighScore = 0;
            TotalCoins = 0;
            UnlockedPoses = new bool[4]; // 기본 4가지 포즈 (WASD)
            CompletedWaves = new bool[10]; // 기본 10개의 웨이브
            BestAccuracy = 0f;
            MaxCombo = 0;
            
            // 현재 게임 상태 초기화
            ResetCurrentData();
            IsRehearsalMode = true;
            RehearsalCount = 0;
        }

        public void ResetCurrentData()
        {
            CurrentWave = 0;
            CurrentLife = gameConfig.lifeCount;
            CurrentTime = gameConfig.limitTime;
            CurrentScore = 0;
            CurrentCombo = 0;
            CurrentAccuracy = 0f;
        }

        public void UpdateHighScore()
        {
            if (CurrentScore > HighScore)
            {
                HighScore = CurrentScore;
            }
        }

        public void UpdateBestAccuracy()
        {
            if (CurrentAccuracy > BestAccuracy)
            {
                BestAccuracy = CurrentAccuracy;
            }
        }

        public void UpdateMaxCombo()
        {
            if (CurrentCombo > MaxCombo)
            {
                MaxCombo = CurrentCombo;
            }
        }

        public void UnlockPose(int poseIndex)
        {
            if (poseIndex >= 0 && poseIndex < UnlockedPoses.Length)
            {
                UnlockedPoses[poseIndex] = true;
            }
        }

        public void CompleteWave(int waveIndex)
        {
            if (waveIndex >= 0 && waveIndex < CompletedWaves.Length)
            {
                CompletedWaves[waveIndex] = true;
            }
        }

        public void AddCoins(int coins)
        {
            TotalCoins += coins;
        }

        public void LoseLife()
        {
            if (CurrentLife > 0)
            {
                CurrentLife--;
            }
        }

        public void AddCombo()
        {
            CurrentCombo++;
            UpdateMaxCombo();
        }

        public void ResetCombo()
        {
            CurrentCombo = 0;
        }

        public void UpdateAccuracy(float accuracy)
        {
            CurrentAccuracy = accuracy;
            UpdateBestAccuracy();
        }

        public bool IsGameOver()
        {
            return CurrentLife <= 0 || CurrentTime <= 0;
        }

        public bool IsWaveComplete()
        {
            return CurrentWave >= gameConfig.waveForCount;
        }
    }

    public enum BeatType
    {
        BEAT_1,
        BEAT_2,
        BEAT_3,
        BEAT_4
    }

    public enum JudgmentType
    {
        Great,
        Good,
        Bad
    }
} 