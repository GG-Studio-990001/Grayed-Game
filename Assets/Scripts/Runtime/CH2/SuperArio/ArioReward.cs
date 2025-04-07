using System;
using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class ArioReward : MonoBehaviour
    {
        [SerializeField] private SurfaceEffector2D _surface;
        private Vector2 _initPos;
        private Rigidbody2D _rb;

        private void Start()
        {
            _initPos = transform.position;
            _rb = GetComponent<Rigidbody2D>();
            _rb.isKinematic = true;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            ArioManager.Instance.OnEnterReward += EnterReward;
        }
        
        private void EnterReward()
        {
            _rb.isKinematic = false;
            gameObject.SetActive(true);
        }
        
        public void ExitReward()
        {
            _rb.isKinematic = true;
            transform.position = _initPos;
            
            ArioManager.Instance.ExitReward();
            gameObject.SetActive(false);
        }

        public void SurfaceControl()
        {
            if(_surface.speed > 0)
                _surface.speed = 0;
            else
            {
                _surface.speed = 3.5f;
            }
        }
    }
}