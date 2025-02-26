using UnityEngine;
using SLGDefines;
using Runtime.CH1.Main.Object;

public class SLGInteractionObject : InteractionObject
{
    [SerializeField] private SLGObjectType type = SLGObjectType.WOOD;
    private SLGActionComponent _SLGAction;
    public bool _isActive = false;

    void Start()
    {
        _SLGAction = FindAnyObjectByType<SLGActionComponent>().GetComponent<SLGActionComponent>();
    }

    private void Update()
    {
        //따로 클릭 가능한 UI들을 빼야할까?
        if (_SLGAction != null && Input.GetMouseButtonDown(0))
        {
            if (_SLGAction.CanInteract() || (_SLGAction.IsInInputTutorial() && type < SLGObjectType.ASSETMAX))
            {
                Vector3 startPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z);
                Vector3 pos = Camera.main.ScreenToWorldPoint(startPoint);
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 10, LayerMask.GetMask("UI"));

                if (hit.collider != null)
                {
                    if (hit.transform.gameObject == this.gameObject)
                    {
                        Interact();
                    }
                }
            }
        }
    }

    public void InitInteractionData(SLGObjectType InType)
    {
        if(_SLGAction == null)
        {
            _SLGAction = FindAnyObjectByType<SLGActionComponent>().GetComponent<SLGActionComponent>();
        }
        type = InType;
        RefreshPopupIcon();
        _isActive = true;
    }
    private void RefreshPopupIcon()
    {
        if (_SLGAction != null)
        {
            Sprite currentSprite = _SLGAction.GetInteractionSprite(type);
            if (currentSprite != null)
            {
                SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = currentSprite;
            }
        }
    }

    public new bool Interact(Vector2 direction = default)
    {
        OnInteract?.Invoke();
        if (_SLGAction != null)
        {
            _SLGAction.ProcessObjectInteraction(type);
            _isActive = false;
            if(type <= SLGObjectType.ASSETMAX)
            {
                this.gameObject.SetActive(false);
            }
        }
        return true;
    }
}
