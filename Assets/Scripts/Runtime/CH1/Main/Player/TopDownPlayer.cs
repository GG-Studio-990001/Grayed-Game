using Runtime.ETC;
using Runtime.Interface;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.CH1.Main.Player
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class TopDownPlayer : MonoBehaviour
    {
        // TODO 이거 데이터로 빼야함
        [SerializeField] private float moveSpeed = 5.0f;
        [SerializeField] private float animSpeed = 0.5f;
        
        public Vector2 Direction => _movement.Direction;
        private Vector2 _lastInput;

        private IMovement _movement;
        public IAnimation Animation { get; private set; }
        private IInteraction _interaction;
        
        private PlayerState _state = PlayerState.Idle;
        private Vector2 _movementInput;

        public bool IsDirecting = false;
        
        private void Start()
        {
            _movement = new TopDownMovement(moveSpeed, transform);
            Animation = new TopDownAnimation(GetComponent<Animator>(), animSpeed);
            _interaction = new TopDownInteraction(transform, LayerMask.GetMask("NPC"), LayerMask.GetMask(GlobalConst.Interaction));
        }
        
        private void Update()
        {
            if (!IsDirecting)
                Animation.SetAnimation(_state.ToString(), _lastInput);
            // 연출 중 애니메이션 교체는 수동으로 SetAnimation 호출
        }

        private void FixedUpdate()
        {
            if (_state == PlayerState.Get)
                return;

            bool isMove = _movement.Move(_movementInput);
            _state = isMove ? PlayerState.Move : PlayerState.Idle;
        }
        
        public void OnInteraction()
        {
            _interaction.Interact(_movement.Direction);
            //bool isInteract = _interaction.Interact(_movement.Direction);
            //if (isInteract)
            //    _state = PlayerState.Idle;
            //_state = isInteract ? PlayerState.Interact : PlayerState.Idle;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _movementInput = context.ReadValue<Vector2>();
            if (context.ReadValue<Vector2>() != Vector2.zero)
                _lastInput = context.ReadValue<Vector2>();
        }

        public void PlayerIdle()
        {
            _state = PlayerState.Idle;
            Animation.SetAnimation(_state.ToString(), _lastInput);
        }

        public void SetLastInput(Vector2 direction)
        {
            _lastInput = direction;
        }
    }
}