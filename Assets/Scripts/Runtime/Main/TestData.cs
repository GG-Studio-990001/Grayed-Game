namespace Runtime.Main
{
    public class TestData
    {
        public void DataAfterIntro()
        {
            Managers.Data.Chapter = 1;
            Managers.Data.Stage = 1;
            Managers.Data.Scene = 1;
            Managers.Data.SceneDetail = 1;
        }

        public void DataAfterPacmom()
        {
            DataAfterIntro();
            Managers.Data.MeetLucky = true;
            Managers.Data.IsPacmomPlayed = true;
            Managers.Data.IsPacmomCleared = true;
            Managers.Data.PacmomCoin = 300;
        }

        public void DataAfter3Match()
        {
            DataAfterPacmom();
            Managers.Data.Stage = 2;
            Managers.Data.Scene = 3;
            Managers.Data.SceneDetail = 1;
            Managers.Data.Is3MatchEntered = true;
            Managers.Data.Is3MatchCleared = true;
        }
    }
}