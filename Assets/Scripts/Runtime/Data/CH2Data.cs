using System;

namespace Runtime.Data
{
    [Serializable]
    public class CH2Data
    {
        public int Turn;
        public string Location;
        public bool IsSpecialDialogue;

        public CH2Data()
        {
            Turn = 0;
            Location = "";
            IsSpecialDialogue = false;
        }
    }
}