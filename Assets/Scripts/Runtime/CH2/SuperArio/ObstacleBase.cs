using UnityEngine;
using UnityEngine.Serialization;

namespace Runtime.CH2.SuperArio
{
    public class ObstacleBase : MonoBehaviour
    {
        public bool isSitObstacle;
        [SerializeField] private bool _isChild;
        [SerializeField] private Vector2 _startPos;
        [SerializeField] private float _endPos;


        private void OnEnable()
        {
            if (!_isChild)
                transform.position = _startPos;
        }

        private void Update()
        {
            if (!_isChild)
            {
                if (ArioManager.instance.isPlay)
                    transform.Translate(Vector2.left * Time.deltaTime * ArioManager.instance.gameSpeed);

                if (transform.position.x < _endPos)
                    gameObject.SetActive(false);
            }
        }
    }
}