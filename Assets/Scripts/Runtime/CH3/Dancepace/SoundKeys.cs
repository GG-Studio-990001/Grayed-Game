namespace Runtime.CH3.Dancepace
{
    /// <summary>
    /// Dancepace 씬에서 사용하는 사운드 경로 상수 관리 클래스
    /// 하드코딩된 문자열 대신 이 클래스를 사용하여 유지보수성 향상
    /// </summary>
    public static class SoundKeys
    {
        // SFX
        public const string AudienceCry = "Dancepace/CH3_SUB_SFX_AudienceCry";
        public const string Button = "Dancepace/CH3_SUB_SFX_Button";
        public const string Spotlight = "Dancepace/CH3_SUB_SFX_Spotlight";
        
        // BGM
        public const string BGM_01_ver3 = "Dancepace/CH3_SUB_BGM_01_ver3";
        public const string WaveIntroOutro = "Dancepace/New/CH3_SUB_BGM_WAVE_Intro_Outro";
        public const string Wave20Bar = "Dancepace/New/CH3_SUB_BGM_WAVE_20Bar";
        
        // Wave SFX (방향별)
        public const string WaveSFX_Up = "Dancepace/New/CH3_SUB_BGM_WAVE_SFX_C_W";
        public const string WaveSFX_Down = "Dancepace/New/CH3_SUB_BGM_WAVE_SFX_F_S";
        public const string WaveSFX_Left = "Dancepace/New/CH3_SUB_BGM_WAVE_SFX_E_A";
        public const string WaveSFX_Right = "Dancepace/New/CH3_SUB_BGM_WAVE_SFX_G_D";
    }
}

