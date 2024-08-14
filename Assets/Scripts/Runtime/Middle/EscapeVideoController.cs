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
            _videoPlayer.frameReady += OnFrameReady;
            _videoPlayer.Prepare();
        }

        private void OnPrepareCompleted(VideoPlayer vp)
        {
            vp.Play();
        }

        private void OnFrameReady(VideoPlayer vp, long frame)
        {
            if (frame == 0)
            {
                _escapeController.StartEscapeDirecting();
            }
        }
    }
}