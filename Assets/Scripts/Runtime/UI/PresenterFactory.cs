using Runtime.UI.Settings;

namespace Runtime.UI
{
    public static class PresenterFactory
    {
        public static SettingsUIPresenter CreateSettingsUIPresenter(SettingsUIView view)
        {
            return new SettingsUIPresenter(view);
        }
        
        // 앞으로 등장하는 모든 Presenter를 여기에 추가한다.
    }
}