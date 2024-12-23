using UnityEngine;

namespace Runtime.CH2.SuperArio
{
    public class ArioReward : MonoBehaviour
    {
        [SerializeField] private GameObject rewardUi;
        private Vector2 _initPos;
        private Rigidbody2D _rb;

        private void Start()
        {
            _initPos = transform.position;
            _rb = GetComponent<Rigidbody2D>();
            _rb.isKinematic = true;
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            ArioManager.instance.OnEnterReward += EnterReward;
        }
    
        private void EnterReward(bool isTrue)
        {
            _rb.isKinematic = false;
            gameObject.SetActive(true);
            rewardUi.SetActive(true);
        }
        
        public void ExitReward()
        {
            _rb.isKinematic = true;
            transform.position = _initPos;
            
            ArioManager.instance.ExitReward();
            gameObject.SetActive(false);
            rewardUi.SetActive(false);
        }
    }
}