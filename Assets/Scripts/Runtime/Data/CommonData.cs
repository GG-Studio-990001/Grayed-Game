using System;

namespace Runtime.Data
{
    [Serializable]
    public class CommonData
    {
        public int Coin; // 접속게임
        public int Wood; // SLG
        public int Stone; // SLG
        public int Translator; // 번역팩

        public CommonData()
        {
            Coin = 0;
            Wood = 0;
            Stone = 0;
            Translator = 0;
        }
    }
}