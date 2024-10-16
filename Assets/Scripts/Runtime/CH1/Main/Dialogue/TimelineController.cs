using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Runtime.CH1.Main.Dialogue
{
    // 타임라인 관리 클래스
    public class TimelineController : MonoBehaviour
    {
        [SerializeField] private TimelineAsset[] _timelineAssets;
        [field: SerializeField] public PlayableDirector PlayableDirector { get; private set; }

        // 안 쓰는 중
        // public PlayerData playerData;

        public void PlayTimeline()
        {
            //PlayTimeline(playerData.quarter.minor);
        }

        //public void PlayTimeline(int minor)
        //{
        //    if (_timelineAssets.Length <= minor)
        //    {
        //        Debug.LogError($"TimelineAsset with minor {minor} not found");
        //        return;
        //    }
        //    PlayableDirector.playableAsset = _timelineAssets[minor];
        //    PlayableDirector.Play();
        //}

        //public void PlayTimeline(string timelineName)
        //{
        //    for (int i = 0; i < _timelineAssets.Length; i++)
        //    {
        //        if (_timelineAssets[i].name == timelineName)
        //        {
        //            PlayableDirector.playableAsset = _timelineAssets[i];
        //            PlayableDirector.Play();
        //            return;
        //        }
        //    }
        //    Debug.LogError($"TimelineAsset with name {timelineName} not found");
        //}
    }
}
