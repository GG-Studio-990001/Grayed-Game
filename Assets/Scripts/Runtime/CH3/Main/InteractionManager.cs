using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

namespace Runtime.CH3.Main
{
    public class InteractionManager : MonoBehaviour
    {
        [SerializeField] private float checkRadius = 3f;
        [SerializeField] private LayerMask interactableLayer;
        
        private Transform playerTransform;

        private void Start()
        {
            playerTransform = transform;
        }

        public void TryInteract()
        {
            Collider[] colliders = Physics.OverlapSphere(playerTransform.position, checkRadius, interactableLayer);
    
            var interactable = colliders
                .Select(c => c.GetComponent<IInteractable>())
                .Where(i => i != null && i.CanInteract)
                .OrderBy(i =>
                {
                    var mb = i as MonoBehaviour;
                    return mb != null 
                        ? Vector3.Distance(playerTransform.position, mb.transform.position) 
                        : float.MaxValue;
                })
                .FirstOrDefault();

            if (interactable != null)
            {
                var mb = interactable as MonoBehaviour;
                if (mb != null)
                {
                    float distance = Vector3.Distance(playerTransform.position, mb.transform.position);
                    if (distance <= interactable.InteractionRange)
                    {
                        interactable.OnInteract(gameObject);
                    }
                }
            }
        }

    }
}