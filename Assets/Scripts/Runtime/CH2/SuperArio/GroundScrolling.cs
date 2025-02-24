using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class GroundScrolling : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        public Transform[] backgrounds;

        private float _leftPosX;
        private float _rightPosX;
        private float _xScreenHalfSize;
        private float _yScreenHalfSize;
        private bool _isInitialized;
        private Vector2 _lastResolution;

        void Start()
        {
            _lastResolution = new Vector2(Screen.width, Screen.height);
            Invoke(nameof(InitializePositions), 0.1f);
        }

        private void InitializePositions()
        {
            _yScreenHalfSize = _mainCamera.orthographicSize;
            _xScreenHalfSize = _yScreenHalfSize * _mainCamera.aspect;
 
            _leftPosX = -(_xScreenHalfSize * 2);
            _rightPosX = _xScreenHalfSize * 2 * backgrounds.Length;

            RealignBackgrounds();
            _isInitialized = true;
        }

        private void RealignBackgrounds()
        {
            float startX = 0f;
            float width = _xScreenHalfSize * 2;
        
            for (int i = 0; i < backgrounds.Length; i++)
            {
                Vector3 pos = backgrounds[i].position;
                pos.x = startX + (width * i);
                backgrounds[i].position = pos;
            }
        }
    
        void Update()
        {
            // 해상도 변경 감지
            if (_lastResolution.x != Screen.width || _lastResolution.y != Screen.height)
            {
                InitializePositions();
                _lastResolution = new Vector2(Screen.width, Screen.height);
            }

            if (!_isInitialized || !ArioManager.instance.IsPlay) return;

            foreach (var tr in backgrounds)
            {
                tr.position += new Vector3(-ArioManager.instance.GameSpeed, 0, 0) * Time.deltaTime;

                if(tr.position.x < _leftPosX)
                {
                    Vector3 nextPos = tr.position;
                    nextPos = new Vector3(nextPos.x + _rightPosX, nextPos.y, nextPos.z);
                    tr.position = nextPos;
                }
            }
        }
    }
}