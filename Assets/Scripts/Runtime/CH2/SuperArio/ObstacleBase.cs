using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class ObstacleBase : MonoBehaviour
    {
        public Vector2 startPos;
        public bool isSitObstacle;

        private void OnEnable()
        {
            transform.position = startPos;
        }

        private void Update()
        {
            if(ArioManager.instance.isPlay)
                transform.Translate(Vector2.left * Time.deltaTime * ArioManager.instance.gameSpeed);

            if (transform.position.x < -6)
            {
                gameObject.SetActive(false);
            }
        }
    }
}