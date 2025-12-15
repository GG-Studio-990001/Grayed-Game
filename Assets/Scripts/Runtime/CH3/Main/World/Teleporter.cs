using UnityEngine;
using System.Collections;
using Runtime.InGameSystem;

namespace Runtime.CH3.Main
{
    public class Teleporter : Structure, IInteractable
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRange = 2f;
        private bool canInteract = true;

        [Header("Teleporter Settings")]
        [SerializeField] private TeleportRegion region = TeleportRegion.BaseCamp;
        [SerializeField] private bool isMainTeleporter = false;
        [SerializeField] private float teleportDelay = 0.1f;
        [SerializeField] private float uiCloseDistance = 2f;

        private TeleporterState state = new TeleporterState();

        public TeleportRegion Region => region;
        public bool IsActivated => state.IsActivated;

        private class TeleporterState
        {
            public bool IsTeleporting = false;
            public bool IsActivated = false;
            public bool CanInteract = true;
            public GameObject CurrentInteractor;
            public Coroutine DistanceCheckCoroutine;
        }

        internal void ResetState()
        {
            state.IsTeleporting = false;
            state.CanInteract = true;
            state.CurrentInteractor = null;
            if (state.DistanceCheckCoroutine != null)
            {
                StopCoroutine(state.DistanceCheckCoroutine);
                state.DistanceCheckCoroutine = null;
            }
        }

        public override void Initialize(Vector2Int gridPos)
        {
            base.Initialize(gridPos);

            Collider collider = GetComponent<Collider>();
            if (collider == null)
            {
                collider = GetComponentInChildren<Collider>();
            }
            if (collider == null)
            {
                var spriteTransform = base.GetSpriteTransform();
                if (spriteTransform != null && spriteTransform != transform)
                {
                    collider = spriteTransform.GetComponent<Collider>();
                }
            }

            RegisterToManager();
        }

        protected override void Start()
        {
            base.Start();
            
            if (TeleporterManager.Instance != null)
            {
                Teleporter registered = TeleporterManager.Instance.GetTeleporter(region);
                if (registered != this)
                {
                    RegisterToManager();
                }
            }
        }

        private void RegisterToManager()
        {
            if (TeleporterManager.Instance == null) return;

            TeleporterManager.Instance.RegisterTeleporter(this);

            if (isMainTeleporter || region == TeleportRegion.BaseCamp)
            {
                state.IsActivated = true;
                TeleporterManager.Instance.ActivateTeleporter(region);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (TeleporterManager.Instance != null)
            {
                TeleporterManager.Instance.UnregisterTeleporter(this);
            }
        }

        public float InteractionRange => interactionRange;
        public bool CanInteract => state.CanInteract && !state.IsTeleporting;

        public void OnInteract(GameObject interactor)
        {
            if (!state.CanInteract || state.IsTeleporting) return;
            if (!interactor.CompareTag("Player")) return;

            if (!state.IsActivated)
            {
                state.IsActivated = true;
                TeleporterManager.Instance?.ActivateTeleporter(region);
            }

            if (isMainTeleporter && TeleporterManager.Instance != null)
            {
                if (!TeleporterManager.Instance.HasOtherActivatedTeleporters(region))
                {
                    return;
                }
            }

            if (TeleportUI.Instance != null && TeleportUI.Instance.IsShowing)
            {
                if (TeleportUI.Instance.CurrentTeleporter != this)
                {
                    TeleportUI.Instance.HideRegionList();
                }
                else
                {
                    return;
                }
            }

            state.CurrentInteractor = interactor;
            if (TeleportUI.Instance != null)
            {
                TeleportUI.Instance.ShowRegionList(region, this);
                if (state.DistanceCheckCoroutine != null)
                {
                    StopCoroutine(state.DistanceCheckCoroutine);
                }
                state.DistanceCheckCoroutine = StartCoroutine(CheckDistanceToInteractor());
            }
        }

        public void TeleportToRegion(TeleportRegion targetRegion, GameObject player)
        {
            if (state.IsTeleporting) return;

            Teleporter targetTeleporter = TeleporterManager.Instance?.GetTeleporter(targetRegion);
            if (targetTeleporter == null) return;

            StartCoroutine(TeleportPlayer(player, targetTeleporter));
        }

        private IEnumerator TeleportPlayer(GameObject player, Teleporter targetTeleporter)
        {
            state.IsTeleporting = true;
            state.CanInteract = false;

            TeleportUI.Instance?.HideRegionList();

            if (state.DistanceCheckCoroutine != null)
            {
                StopCoroutine(state.DistanceCheckCoroutine);
                state.DistanceCheckCoroutine = null;
            }

            FadeController fadeController = FindObjectOfType<FadeController>();
            if (fadeController != null)
            {
                fadeController.StartFadeOut();
                yield return new WaitForSeconds(fadeController.FadeDuration);
            }

            Vector3 targetPosition = CalculateTargetPosition(player, targetTeleporter);
            MovePlayerToPosition(player, targetPosition);

            if (fadeController != null)
            {
                fadeController.StartFadeIn();
                yield return new WaitForSeconds(fadeController.FadeDuration);
            }

            yield return new WaitForSeconds(teleportDelay);

            ResetState();
            targetTeleporter?.ResetState();
        }

        private Vector3 CalculateTargetPosition(GameObject player, Teleporter targetTeleporter)
        {
            if (GridSystem.Instance == null) return player.transform.position;

            Vector2Int destGrid = targetTeleporter.GridPosition;
            destGrid.y -= 1;
            Vector3 targetPosition = GridSystem.Instance.GridToWorldPosition(destGrid);
            targetPosition.y = player.transform.position.y;
            return targetPosition;
        }

        private void MovePlayerToPosition(GameObject player, Vector3 position)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.position = position;
            }
            else
            {
                player.transform.position = position;
            }
        }

        private IEnumerator CheckDistanceToInteractor()
        {
            const float checkInterval = 0.1f;
            float sqrCloseDistance = uiCloseDistance * uiCloseDistance;

            while (state.CurrentInteractor != null && TeleportUI.Instance != null && TeleportUI.Instance.IsShowing)
            {
                Vector3 offset = state.CurrentInteractor.transform.position - transform.position;
                if (offset.sqrMagnitude > sqrCloseDistance)
                {
                    TeleportUI.Instance.HideRegionList();
                    state.CurrentInteractor = null;
                    yield break;
                }
                yield return new WaitForSeconds(checkInterval);
            }
        }


    }
}