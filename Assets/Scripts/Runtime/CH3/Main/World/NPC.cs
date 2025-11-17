using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

namespace Runtime.CH3.Main
{
    public class NPC : InteractableGridObject, IGridPassable
    {
        [SerializeField] private string npcName;
        [TextArea]
        [SerializeField] private string dialogueText;

        [Header("Grid Position Settings")]
        [SerializeField] private bool occupyGridCell = true;
        [SerializeField] private bool passableWhileOccupying = true;

        [Header("Dialogue Settings")]
        [SerializeField] private bool autoFindDialogueRunner = true;
        [SerializeField] private DialogueRunner dialogueRunner;
        [SerializeField] private string startNode;
        [SerializeField] private bool lockPlayerInputDuringDialogue = true;
        [SerializeField] private UnityEvent onDialogueStarted;
        [SerializeField] private UnityEvent onDialogueCompleted;

        private DialogueRunner subscribedRunner;
        private bool isInteractionInProgress;

        public bool IsPassable => !occupyGridCell || passableWhileOccupying;

        public override void Initialize(Vector2Int gridPos)
        {
            // base.Initialize에서 gridPositionMode에 따라 위치 결정
            base.Initialize(gridPos);

            // occupyGridCell은 base.Initialize의 OccupyTiles에서 이미 처리됨
            // 필요시 추가 로직을 여기에 작성
        }

        private void OnEnable()
        {
            EnsureDialogueRunnerReference();
            SubscribeToDialogueRunner();
            PreloadDialogueRunner();
        }

        private void PreloadDialogueRunner()
        {
            // DialogueRunner를 미리 준비시켜 첫 실행 시 지연을 줄임
            if (dialogueRunner != null && dialogueRunner.yarnProject != null)
            {
                StartCoroutine(PrepareDialogueRunner());
            }
        }

        private IEnumerator PrepareDialogueRunner()
        {
            // DialogueRunner가 준비될 때까지 기다림
            if (dialogueRunner != null && dialogueRunner.yarnProject != null)
            {
                // Yarn 프로젝트가 로드되었는지 확인
                yield return null;
            }
        }

        private void OnDisable()
        {
            UnsubscribeFromDialogueRunner();
        }

        public override void OnInteract(GameObject interactor)
        {
            if (!canInteract || isInteractionInProgress)
            {
                return;
            }

            DialogueRunner runner = EnsureDialogueRunnerReference();

            if (runner == null)
            {
                return;
            }

            if (runner.yarnProject == null)
            {
                Debug.LogWarning($"NPC '{name}'의 DialogueRunner에 YarnProject가 설정되어 있지 않습니다.");
                return;
            }

            if (runner.IsDialogueRunning)
            {
                return;
            }

            string nodeToStart = string.IsNullOrEmpty(startNode) ? runner.startNode : startNode;
            if (!string.IsNullOrEmpty(nodeToStart) && !runner.NodeExists(nodeToStart))
            {
                Debug.LogWarning($"NPC '{name}'가 요청한 노드 '{nodeToStart}'를 찾을 수 없습니다.");
                return;
            }

            canInteract = false;
            isInteractionInProgress = true;

            if (lockPlayerInputDuringDialogue)
            {
                Managers.Data?.InGameKeyBinder?.PlayerInputDisable();
            }

            // 즉시 이벤트 호출하여 반응성 향상
            onDialogueStarted?.Invoke();

            // 코루틴으로 대화 시작하여 지연 최소화
            StartCoroutine(StartDialogueAsync(runner, nodeToStart));
        }

        private IEnumerator StartDialogueAsync(DialogueRunner runner, string node)
        {
            // 한 프레임 대기하여 UI 업데이트가 먼저 되도록 함
            yield return null;

            if (!TryStartDialogue(runner, node))
            {
                isInteractionInProgress = false;
                canInteract = true;
                
                if (lockPlayerInputDuringDialogue)
                {
                    Managers.Data?.InGameKeyBinder?.PlayerInputEnable();
                }
            }
        }

        private bool TryStartDialogue(DialogueRunner runner, string node)
        {
            try
            {
                runner.StartDialogue(node);
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"NPC '{name}'가 대화를 시작하지 못했습니다: {ex.Message}", this);
                return false;
            }
        }

        private DialogueRunner EnsureDialogueRunnerReference()
        {
            if (dialogueRunner == null && autoFindDialogueRunner)
            {
                dialogueRunner = FindObjectOfType<DialogueRunner>();
            }

            SubscribeToDialogueRunner();
            return dialogueRunner;
        }

        private void SubscribeToDialogueRunner()
        {
            if (dialogueRunner == null)
            {
                return;
            }

            if (subscribedRunner == dialogueRunner)
            {
                return;
            }

            UnsubscribeFromDialogueRunner();
            dialogueRunner.onDialogueComplete.AddListener(HandleDialogueComplete);
            subscribedRunner = dialogueRunner;
        }

        private void UnsubscribeFromDialogueRunner()
        {
            if (subscribedRunner != null)
            {
                subscribedRunner.onDialogueComplete.RemoveListener(HandleDialogueComplete);
                subscribedRunner = null;
            }
        }

        private void HandleDialogueComplete()
        {
            if (!isInteractionInProgress)
            {
                return;
            }

            if (lockPlayerInputDuringDialogue)
            {
                Managers.Data?.InGameKeyBinder?.PlayerInputEnable();
            }

            onDialogueCompleted?.Invoke();

            isInteractionInProgress = false;
            canInteract = true;
        }
    }
}