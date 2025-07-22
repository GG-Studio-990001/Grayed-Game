[System.Serializable]
public class GameResultData
{
    public int TotalScore { get; private set; }
    public int PerfectCnt { get; private set; }
    public int GreatCnt { get; private set; }
    public int BadCnt { get; private set; }

    public GameResultData(int total, int perfect, int great, int bad)
    {
        TotalScore = total;
        PerfectCnt = perfect;
        GreatCnt = great;
        BadCnt = bad;
    }
    public void AddScore(int score)
    {
        TotalScore += score;
    }

    public void AddPerfect()
    {
        PerfectCnt++;
    }

    public void AddGreat()
    {
        GreatCnt++;
    }

    public void AddBad()
    {
        BadCnt++;
    }

    public void Reset()
    {
        TotalScore = 0;
        PerfectCnt = 0;
        GreatCnt = 0;
        BadCnt = 0;
    }
}