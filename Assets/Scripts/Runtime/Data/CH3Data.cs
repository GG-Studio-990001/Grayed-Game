using System;
using Runtime.CH3.Dancepace;

namespace Runtime.Data
{
    [Serializable]
    public class CH3Data
    {
        public bool IsDancepacePlayed;
        public bool IsDancepaceCleared;

        public CH3Data()
        {
            IsDancepacePlayed = false;
            IsDancepaceCleared = false;
        }
    }
}