using Runtime.CH1.SubB._3_Match;
using System;
using UnityEngine;
using SLGDefines;

namespace Runtime.Data
{
    [Serializable]
    public class CH1Data
    {
        // Progress
        public int Stage;
        public int Scene;
        public int SceneDetail; // 씬 세부 진행도
        // 3match
        public bool Is3MatchEntered;
        public bool Is3MatchCleared;
        public ThreeMatchPuzzleStageData[] ThreeMatchPuzzleStageData;
        // Pacmom
        public bool IsPacmomPlayed;
        public bool IsPacmomCleared;
        // SLG
        public long[] SLGConstructionBeginTime;
        public SLGProgress SLGProgressData;
        public SLGBuildingProgress[] SLGBuildingProgressData;

        public CH1Data()
        {
            // Progress
            Stage = 1;
            Scene = 0;
            SceneDetail = 0;
            // 3match
            Is3MatchEntered = false;
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
            // SLG
            SLGProgressData = SLGProgress.None;
            SLGConstructionBeginTime = new long[(int)SLGBuildingType.Max];
            SLGBuildingProgressData = new SLGBuildingProgress[(int)SLGBuildingType.Max];
        }
    }
}