using UnityEngine;

namespace Runtime.CH3.Dancepace
{
    [CreateAssetMenu(menuName = "Dancepace/GameConfig")]
    public class DPGameConfigSO : ScriptableObject
    {
        public float limitTime;
        public int waveForCount;
        public int waveClearCoin;
        public int greatCoin;
        public int goodCoin;
        public int badCoin;
        public float greatTimingWindow;
        public float goodTimingWindow;
    }
} 