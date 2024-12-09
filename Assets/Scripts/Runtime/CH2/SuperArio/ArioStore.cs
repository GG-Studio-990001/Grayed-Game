using Runtime.CH2.SuperArio;
using UnityEngine;
using UnityEngine.InputSystem;

public class ArioStore : MonoBehaviour
{
    [SerializeField] private SurfaceEffector2D _surface;
    [SerializeField] private StoreWall[] _storeWalls;
    [SerializeField] private GameObject[] _boxes;
    
    private Vector2 _initPos;
    private Rigidbody2D _rb;
    
    private void Start()
    {
        _initPos = transform.position;
        _rb = GetComponent<Rigidbody2D>();
        _rb.isKinematic = true;
        ArioManager.instance.OnEnterStore += EnterStore;
    }

    private void OnDestroy()
    {
        ArioManager.instance.OnEnterStore -= EnterStore;
    }

    private void EnterStore(bool isTrue)
    {
        _rb.isKinematic = false;
        gameObject.SetActive(true);
        foreach (var box in _boxes)
        {
            if(box.TryGetComponent(out IStoreBox storeBox))
                storeBox.Check();
        }
    }

    private void ExitStore()
    {
        _rb.isKinematic = true;
        transform.position = _initPos;
        gameObject.SetActive(false);
        ArioManager.instance.RestartSuperArio();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.TryGetComponent(out StoreWall wall))
            return;
        
        if (wall.IsLeft)
        {
            // 컨베이어 오른쪽으로 변경
            _surface.speed = 3.5f;
        }
        else
        {
            // 컨베이어 왼쪽으로 변경
            _surface.speed = -3.5f;
        }
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!ArioManager.instance.IsPlay || ArioManager.instance.IsPause)
            return;

        Vector2 moveInput = context.ReadValue<Vector2>();

        // if (context.performed)
        // {
        //     // 점프 중일 때 아래 방향키를 눌렀을 때 앉기 상태로 전환
        //     if (moveInput.y < 0) // 아래쪽
        //     {
        //         if (_col.offset.y == 0)
        //             _col.offset = new Vector2(0, -0.1f);
        //         // 점프 중에도 앉기 상태로 변경
        //         _animator.enabled = false;
        //         _spr.sprite = sitSprite;
        //     }
        //     else if (moveInput.y > 0 && transform.position.y <= _startPos.y && !_isJump) // 위쪽 (점프)
        //     {
        //         _jumpBufferTimeRemaining = _jumpBufferTime; // 점프 입력을 버퍼에 저장
        //     }
        // }
        // else if (context.canceled) // 아래 방향키를 뗐을 때
        // {
        //     _animator.enabled = true;
        //     _spr.sprite = _initSprite;
        //
        //     // 아래 방향키를 뗀 후, 앉기 상태를 원래 상태로 되돌림
        //     if (_col.offset.y != 0)
        //         _col.offset = new Vector2(0, 0);
        // }
    }
    
    // private void FixedUpdate()
    // {
    //     if (!ArioManager.instance.IsPlay) return;
    //
    //     // 점프 대기 시간 동안 점프를 실행할지 여부 결정
    //     if (_jumpBufferTimeRemaining > 0)
    //     {
    //         _jumpBufferTimeRemaining -= Time.fixedDeltaTime;
    //     }
    //
    //     // 점프 처리
    //     TryJump();
    //
    //     if (_isJump)
    //     {
    //         if (transform.position.y <= _jumpHeight - 0.1f && !_isTop)
    //         {
    //             transform.position = Vector2.Lerp(transform.position,
    //                 new Vector2(transform.position.x, _jumpHeight), _jumpSpeed * Time.fixedDeltaTime);
    //         }
    //         else
    //         {
    //             _isTop = true;
    //         }
    //
    //         if (transform.position.y > _startPos.y && _isTop)
    //         {
    //             transform.position = Vector2.MoveTowards(transform.position, _startPos, _jumpSpeed * Time.fixedDeltaTime);
    //         }
    //     }
    //
    //     // 땅에 닿았을 때 점프 관련 상태만 초기화
    //     if (transform.position.y <= _startPos.y)
    //     {
    //         _isJump = false;
    //         _isTop = false;
    //         transform.position = _startPos;
    //     }
    // }
}