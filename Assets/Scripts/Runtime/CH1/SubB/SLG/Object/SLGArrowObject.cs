using Runtime.CH1.Main.Player;
using UnityEngine;

public class SLGArrowObject : MonoBehaviour
{
    Vector2 _TargetPos;
    float _deltaTime = 0;

    const float ArrowDisplayTime = 2.0f;
    GameObject _player;
    ObjectFadeInOutComponent _fadeInOutComp;

    const float FadeInOutTime = 0.5f;
    bool bFadeIn = false;
    bool bFadeOut = false;

    void Awake()
    {
        _deltaTime = 0;
        _player = FindAnyObjectByType<TopDownPlayer>().gameObject;
    }

    private void Start()
    {
        _fadeInOutComp = this.gameObject.GetComponent<ObjectFadeInOutComponent>();
    }
    void Update()
    {
        if (_deltaTime < FadeInOutTime)
        {
            if (bFadeOut == false)
            {
                if (_fadeInOutComp != null)
                {
                    _fadeInOutComp.PlayFadeOut(FadeInOutTime);
                    bFadeOut = true;
                }
            }
        }
        else if(ArrowDisplayTime - FadeInOutTime <= _deltaTime  && _deltaTime < ArrowDisplayTime)
        {
            if (bFadeIn == false)
            {
                if (_fadeInOutComp != null)
                {
                    _fadeInOutComp.PlayFadeIn(FadeInOutTime);
                    bFadeIn = true;
                }
            }
        }
        if(_deltaTime > ArrowDisplayTime)
        {
            _deltaTime = 0;
            bFadeIn = false;
            bFadeOut = false;

            Destroy(this.gameObject);
        }
        _deltaTime += Time.deltaTime;
    }
    public void SetTargetPos(Vector3 TargetPos)
    {
        _deltaTime = 0;
        _TargetPos = TargetPos;

        float angle = _player.transform.position.y
            > _TargetPos.y ? 0.0F : 180.0F;
        this.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        this.transform.localPosition = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
    }
}