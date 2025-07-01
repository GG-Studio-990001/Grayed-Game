using UnityEngine;
using Runtime.CH3.Dancepace;

namespace Runtime.CH3.Dancepace
{
    [CreateAssetMenu(menuName = "Dancepace/GameConfig")]
    public class GameConfigSO : ScriptableObject
    {
        public float limitTime;
        public int waveForCount;
        public int waveClearCoin;
        public int greatCoin;
        public int goodCoin;
        public int badCoin;
        public float perfectTimingWindow;
        public float greatTimingWindow;
    }
} 