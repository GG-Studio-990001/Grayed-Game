namespace Runtime.ETC
{
    public enum PlayerState
    {
        Idle,
        Move,
        Get,
    }

    public enum JewelryType
    {
        A,
        B,
        C,
        D,
        None,
        Disappear
    }

    public enum Speaker
    {
        dustA,
        dustB,
        rapley,
        none
    }

    public enum Sound
    {
        BGM,
        SFX,
        Speech,
        LuckyBGM,
        Max,
    }

    //Chapter 관리를 위한 임시 데이터
    public enum Chapter
    {
        CHAP1 = 100,
        CHAP1_PM = 110,
        CHAP2_TMP = 120,
        CHAP1_SLG = 130,
        CHAP2 = 200,
        Max
    }
    
    public enum AspectRatio
    {
        Ratio_8_7,     // 8:7
        Ratio_21_9     // 21:9
    }
}