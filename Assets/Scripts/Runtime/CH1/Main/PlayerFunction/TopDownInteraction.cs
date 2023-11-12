using Runtime.Common;
using UnityEngine;

namespace Runtime.CH1.Main
{
    public class TopDownInteraction
    {
        private int _interactionLayerMask;
        private Transform _transform;
        private float _interactionDistance;
        
        public TopDownInteraction(Transform transform, int interactionLayerMask, float interactionDistance = 1.0f)
        {
            _transform = transform;
            _interactionLayerMask = interactionLayerMask;
            _interactionDistance = interactionDistance;
        }
        
        public bool Interact(Vector2 direction)
        {
            RaycastHit2D hit = Physics2D.Raycast(_transform.position, direction, _interactionDistance, _interactionLayerMask);
            
            if (hit.collider != null)
            {
                hit.collider.GetComponent<Interactive>()?.Interact();
                return true;
            }
            
            return false;
        }
    }
}