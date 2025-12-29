using Runtime.Data;
using Yarn.Unity;

namespace Runtime.CH3.Main
{
    // Yarn Spinner에서 사용할 정적 함수들을 모아놓은 클래스
    public static class CH3YarnFunction
    {
        [YarnFunction("IsFirstMeetR2Mon")]
        public static bool IsFirstMeetR2Mon()
        {
            return !Managers.Data.CH3.IsFirstMeet;
        }
    }
}

