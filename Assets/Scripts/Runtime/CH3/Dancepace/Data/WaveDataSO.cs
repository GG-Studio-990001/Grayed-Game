using System.Collections.Generic;
using UnityEngine;
using System;
using Runtime.ETC;
using Runtime.CH3.Dancepace;

namespace Runtime.CH3.Dancepace
{
    [Serializable]
    public class BeatData
    {
        public EPoseType poseData;
        public float timing;
        public float restTime;

        public BeatData(EPoseType poseData, float timing, float restTime)
        {
            this.poseData = poseData;
            this.timing = timing;
            this.restTime = restTime;
        }

        public string EnumToString() => poseData.ToString();
    }

    [Serializable]
    public class WaveData
    {
        public string waveId;
        public List<BeatData> beats;

        public WaveData(string waveId, List<BeatData> beats)
        {
            this.waveId = waveId;
            this.beats = beats;
        }
    }

    [CreateAssetMenu(menuName = "Dancepace/WaveData")]
    public class WaveDataSO : ScriptableObject
    {
        public List<WaveData> waveDatas;
    }
}
