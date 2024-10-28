using System;

namespace Runtime.Data.Original
{
    [Serializable]
    public class CH2Data
    {
        public int Turn;
        public int Progress;
        public string Location;
        public bool IsSpecialDialogue;

        public CH2Data()
        {
            Turn = 0; // 추후 삭제 예정
            Progress = 0;
            Location = "";
            IsSpecialDialogue = false;
        }
    }
}