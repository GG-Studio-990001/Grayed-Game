using System;

namespace Runtime.Data.Original
{
    [Serializable]
    public class CH2Data
    {
        public int Turn;
        public int Progress;
        public string Location;

        public CH2Data()
        {
            Turn = 0;
            Progress = 0;
            Location = "";
        }
    }
}