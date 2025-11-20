using UnityEngine;

namespace Runtime.CH3.Main
{
	public abstract class InteractableGridObject : GridObject, IInteractable
	{
		[SerializeField] protected float interactionRange = 2f;
		protected bool canInteract = true;

		public float InteractionRange => interactionRange;
		public bool CanInteract => canInteract;

		/// <summary>
		/// CH3_LevelData로부터 데이터를 초기화합니다.
		/// </summary>
		public override void InitializeFromData(CH3_LevelData data)
		{
			base.InitializeFromData(data);
			if (data != null)
			{
				interactionRange = data.interactionRange;
			}
		}

		public abstract void OnInteract(GameObject interactor);
	}
}

