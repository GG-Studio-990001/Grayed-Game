using Runtime.CH2.SuperArio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders.Simulation;
using UnityEngine.Serialization;

public abstract class ItemBox : MonoBehaviour
{
    [field:SerializeField] public Sprite useSprite { get; private set; }
    protected SpriteRenderer _spr;
    protected Sprite _initSprite;
    protected int _cost;
    protected bool IsUsed { get; set; }
    
    private BoxCollider2D _col;
    protected bool CanBuy()
    {
        if(ArioManager.Instance.CoinCnt >= _cost)
        {
            return true;
        }

        return false;
    }
    
    protected virtual void Init()
    {
        _spr = GetComponent<SpriteRenderer>();
        _col = GetComponent<BoxCollider2D>();
        _initSprite = _spr.sprite;
    }
    
    protected IEnumerator Delay()
    {
        _col.enabled = false;
        yield return new WaitForSeconds(0.95f);
        _col.enabled = true;
    }
    
    protected virtual void ChangeSprite()
    {
        if (_spr != null)
        {
            _spr.sprite = useSprite;
        }
    }

    protected virtual void ResetSprite()
    {
        if (_spr != null)
        {
            _spr.sprite = _initSprite; // 원래 색상으로 복구
        }
    }

    public abstract void Check();
    protected abstract void Use();
    
    protected void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out ArioStore ario))
        {
            Use();
        }
    }
}
