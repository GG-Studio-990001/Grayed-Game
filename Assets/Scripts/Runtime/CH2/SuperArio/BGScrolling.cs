using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class BGScrolling : MonoBehaviour
    {
        private Renderer _renderer;

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
        }

        private void Update()
        {
            if (ArioManager.Instance.IsPlay)
            {
                float move = Time.deltaTime * (ArioManager.Instance.GameSpeed / 50);
                _renderer.material.mainTextureOffset += Vector2.right * move;
            }
        }
    }
}