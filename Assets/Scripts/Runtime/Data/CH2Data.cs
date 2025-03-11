using System;
using System.Collections.Generic;

namespace Runtime.Data
{
    [Serializable]
    public class CH2Data
    {
        public int Turn;
        public string Location;
        public int TcgNum;
        public int TcgScore;
        public List<int> UsedTcgAnswers;
        public string ArioStage;

        public CH2Data()
        {
            // 비주얼 노벨
            Turn = 0;
            Location = "";
            // TCG
            TcgNum = 0;
            TcgScore = 0;
            UsedTcgAnswers = new List<int>();
            ArioStage = "1-1";
        }
    }
}