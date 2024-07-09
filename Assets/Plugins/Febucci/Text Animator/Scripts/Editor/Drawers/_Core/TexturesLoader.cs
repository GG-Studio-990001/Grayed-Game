using UnityEditor;
using UnityEngine;

namespace Febucci.UI
{
    public static class TexturesLoader
    {
        const string localPath_resourcesFolder = "Text Animator";
        
        static Texture aboutLogo;
        public static Texture AboutLogo
        {
            get
            {
                if(!aboutLogo) aboutLogo = Resources.Load<Texture>(localPath_resourcesFolder+"/about_logo");
                return aboutLogo;
            }
        }

        
        static Texture stopIcon;
        public static Texture StopIcon
        {
            get
            {
                if(!stopIcon) stopIcon = Resources.Load<Texture>(localPath_resourcesFolder+"/stop_icon");
                return stopIcon;
            }
        }
        
        static Texture restartIcon;
        public static Texture RestartIcon
        {
            get
            {
                if(!restartIcon) restartIcon = Resources.Load<Texture>(localPath_resourcesFolder+"/restart_icon");
                return restartIcon;
            }
        }

        static Texture saveIcon;
        public static Texture SaveIcon
        {
            get
            {
                if(!saveIcon) saveIcon = Resources.Load<Texture>(localPath_resourcesFolder+"/save_icon");
                return saveIcon;
            }
        }
        
        static Texture playIcon;
        public static Texture PlayIcon
        {
            get
            {
                if(!playIcon) playIcon = Resources.Load<Texture>(localPath_resourcesFolder+"/play_icon");
                return playIcon;
            }
        }
        
        static Texture pauseIcon;
        public static Texture PauseIcon
        {
            get
            {
                if(!pauseIcon) pauseIcon = Resources.Load<Texture>(localPath_resourcesFolder+"/pause_icon");
                return pauseIcon;
            }
        }
        

        public static Texture WarningIcon => EditorGUIUtility.IconContent("Warning").image;
        public static Texture ErrorIcon => EditorGUIUtility.IconContent("Error").image;
    }
}