using Runtime.ETC;
using Runtime.Interface;
using UnityEngine;

namespace Runtime.CH1.SubB
{
    public class Jewelry : MonoBehaviour
    {
        [field:SerializeField] public JewelryType JewelryType { get; set; }
        [SerializeField] private Transform spriteTransform;
        [SerializeField] private float moveTime = 1.0f;

        public ThreeMatchPuzzleController Controller { get; set; }
        
        private Vector3 _firstPosition;
        private IMovement _movement;
        private float _pushTime;

        private void Awake()
        {
            _firstPosition = transform.position;
            _movement = new JewelryMovement(this.transform, spriteTransform, moveTime);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            if (JewelryType == JewelryType.None)
            {
                return;
            }
            
            _pushTime += Time.deltaTime;
            
            if (other.gameObject.CompareTag(GlobalConst.PlayerStr))
            {
                if (_pushTime > 2f)
                {
                    _pushTime = 0f;
                    
                    Vector2 playerDetectionPos = new Vector2(other.transform.position.x, other.transform.position.y + other.collider.offset.y);

                    Vector2 direction = (transform.position - (Vector3)playerDetectionPos);
                    
                    direction.x = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? Mathf.Sign(direction.x) : 0f;
                    direction.y = Mathf.Abs(direction.y) > Mathf.Abs(direction.x) ? Mathf.Sign(direction.y) : 0f;

                    if (Controller.ValidateMovement(this, direction))
                    {
                        _movement.Move(direction);
                        Invoke(nameof(CallCheckMatching), moveTime);
                    }
                }
            }
        }
        
        private void OnCollisionExit2D(Collision2D other) => _pushTime = 0f;
        
        public void ResetPosition() => transform.position = _firstPosition;
        public void DestroyJewelry() => Destroy(gameObject);
        private void CallCheckMatching() => Controller.CheckMatching();
    }
}
