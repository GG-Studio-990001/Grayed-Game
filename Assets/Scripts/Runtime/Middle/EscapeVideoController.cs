using UnityEngine;
using UnityEngine.Video;

namespace Runtime.Middle
{
    [RequireComponent(typeof(EscapeController))]
    public class EscapeVideoController : MonoBehaviour
    {
        private EscapeController _escapeController;
        [SerializeField] private VideoPlayer _videoPlayer;

        private void Awake()
        {
            _escapeController = GetComponent<EscapeController>();
        }

        private void Start()
        {
            _videoPlayer.prepareCompleted += OnPrepareCompleted;
            _videoPlayer.started += OnVideoStarted;
            _videoPlayer.Prepare();
        }

        private void OnPrepareCompleted(VideoPlayer vp)
        {
            Debug.Log("VideoReady");
            vp.Play();
        }

        private void OnVideoStarted(VideoPlayer vp)
        {
            Debug.Log("VideoStart");
            _escapeController.StartEscapeDirecting();
        }
    }
}