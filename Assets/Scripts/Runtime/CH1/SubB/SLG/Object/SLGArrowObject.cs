using Codice.CM.SEIDInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SLGArrowObject : MonoBehaviour
{
    float _deltaTime = 0;
    Vector2 _TargetPos;

    const float ArrowDisplayTime = 2.0f;
    void Start()
    {
        _deltaTime = 0;
    }

    void Update()
    {
        if (_deltaTime < ArrowDisplayTime)
        {
            float angle = Mathf.Atan2(gameObject.transform.position.y - _TargetPos.y, gameObject.transform.position.x - _TargetPos.x) * Mathf.Rad2Deg;
            this.transform.rotation = Quaternion.AngleAxis(angle+180, Vector3.forward);

            _deltaTime += Time.deltaTime;
        }
        else
        {
            _deltaTime = 0;
            Destroy(this.gameObject);
        }
    }

    public void SetTargetPos(Vector3 TargetPos)
    {
        _deltaTime = 0;
        _TargetPos = TargetPos;
    }
}
