using UnityEngine;

namespace Runtime.CH4
{
    public class TeleportCamera : MonoBehaviour
    {
        private Camera _mainCamera;
        [SerializeField] private Transform _player;
        [SerializeField] private Transform[] _stages;

        private void Awake()
        {
            _mainCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (_player.position.y < 10)
                SetCamPos(0);
            else if (_player.position.y < 28)
                SetCamPos(1);
            else
                SetCamPos(2);
        }

        private void SetCamPos(int val)
        {
            _mainCamera.transform.position = new(_stages[val].position.x, _stages[val].position.y, _mainCamera.transform.position.z);
        }
    }
}
