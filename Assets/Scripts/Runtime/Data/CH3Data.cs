using System;
using Runtime.CH3.Dancepace;

namespace Runtime.Data
{
    [Serializable]
    public class CH3Data
    {
        public DancepaceData Dancepace;

        public CH3Data()
        {
            Dancepace = new DancepaceData();
        }
    }
}