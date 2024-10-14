using System;

namespace Runtime.Data.Original
{
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
}