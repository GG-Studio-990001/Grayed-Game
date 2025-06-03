using UnityEngine;
using System;

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
        public float timing;
        public string poseId;
        public bool isPreview;
        public bool isPlay;
        public bool isRestBeat;
    }

    [Serializable]
    public class WaveData
    {
        public string waveId;
        public float duration;
        public BeatData[] previewBeats;
        public BeatData[] restBeats;
        public BeatData[] playBeats;
        public bool isRehearsal;
    }

    [Serializable]
    public class GameConfig
    {
        public float limitTime;
        public int waveForCount;
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
        public int HighScore;
        public int TotalCoins;
        public bool[] UnlockedPoses;
        public bool[] CompletedWaves;
        public float BestAccuracy;
        public int MaxCombo;
        public int CurrentWave;
        public int CurrentLife;
        public float CurrentTime;
        public int CurrentScore;
        public int CurrentCombo;
        public float CurrentAccuracy;

        public DancepaceData()
        {
            HighScore = 0;
            TotalCoins = 0;
            UnlockedPoses = new bool[4]; // 기본 4가지 포즈 (WASD)
            CompletedWaves = new bool[10]; // 기본 10개의 웨이브
            BestAccuracy = 0f;
            MaxCombo = 0;
            CurrentWave = 0;
            CurrentLife = 3;
            CurrentTime = 0f;
            CurrentScore = 0;
            CurrentCombo = 0;
            CurrentAccuracy = 0f;
        }

        public void ResetCurrentData()
        {
            CurrentWave = 0;
            CurrentLife = 3;
            CurrentTime = 0f;
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
    }

    public enum BeatType
    {
        BEAT_1,
        BEAT_2,
        BEAT_3,
        BEAT_4,
        BEAT_5,
        BEAT_6,
        BEAT_7,
        BEAT_8
    }

    public enum JudgmentType
    {
        Great,
        Good,
        Bad
    }
} 