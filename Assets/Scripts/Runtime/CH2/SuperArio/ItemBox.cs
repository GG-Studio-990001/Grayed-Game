using Runtime.CH2.SuperArio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemBox : MonoBehaviour
{
    [field:SerializeField] public Sprite closeSprite { get; private set; }
    protected int _cost;

    protected bool CanBuy()
    {
        if(ArioManager.instance.CoinCnt >= _cost)
        {
            return true;
        }

        return false;
    }
}
