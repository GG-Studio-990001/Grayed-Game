using UnityEngine;

namespace Runtime.CH3.Main
{
	public abstract class InteractableGridObject : GridObject, IInteractable
	{
		[SerializeField] protected float interactionRange = 2f;
		protected bool canInteract = true;

		public float InteractionRange => interactionRange;
		public bool CanInteract => canInteract;

		public abstract void OnInteract(GameObject interactor);
	}
}


