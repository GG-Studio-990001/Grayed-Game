using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class GroundScroll : MonoBehaviour
    {
        public Transform[] backgrounds;

        private float _leftPosX;
        private float _rightPosX;
        private float _xScreenHalfSize;
        private float _yScreenHalfSize;
    
        void Start()
        {
            _yScreenHalfSize = Camera.main.orthographicSize;
            _xScreenHalfSize = _yScreenHalfSize * Camera.main.aspect;
 
            _leftPosX = -(_xScreenHalfSize * 2);
            _rightPosX = _xScreenHalfSize * 2 * backgrounds.Length;
        }
    
        void Update()
        {
            if (ArioManager.instance.IsPlay)
            {
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
}