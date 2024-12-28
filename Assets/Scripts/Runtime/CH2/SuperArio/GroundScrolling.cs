using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class GroundScrolling : MonoBehaviour
    {
        public Transform[] backgrounds;

        private float _leftPosX;
        private float _rightPosX;
        private float _xScreenHalfSize;
        private float _yScreenHalfSize;
        private bool _isInitialized;

        void Start()
        {
            // 첫 프레임 이후에 초기화하도록 설정
            Invoke(nameof(InitializePositions), 0.1f);
        }

        private void InitializePositions()
        {
            _yScreenHalfSize = Camera.main.orthographicSize;
            _xScreenHalfSize = _yScreenHalfSize * Camera.main.aspect;
 
            _leftPosX = -(_xScreenHalfSize * 2);
            _rightPosX = _xScreenHalfSize * 2 * backgrounds.Length;

            // 초기 위치 재정렬
            float startX = 0f;
            float width = _xScreenHalfSize * 2;
        
            for (int i = 0; i < backgrounds.Length; i++)
            {
                Vector3 pos = backgrounds[i].position;
                pos.x = startX + (width * i);
                backgrounds[i].position = pos;
            }

            _isInitialized = true;
        }
    
        void Update()
        {
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