using System;
using Runtime.CH3.Dancepace;

namespace Runtime.Data
{
    [Serializable]
    public class CH3Data
    {
        public bool IsDancepacePlayed;
        public bool IsFirstMeet;

        public CH3Data()
        {
            IsDancepacePlayed = false;
            IsFirstMeet = false;
        }
    }
}