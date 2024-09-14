using Runtime.CH1.Main.Player;
using UnityEngine;

public class SLGArrowObject : MonoBehaviour
{
    Vector2 _TargetPos;
    float _deltaTime = 0;

    const float ArrowDisplayTime = 2.0f;
    GameObject _player;
    void Start()
    {
        _deltaTime = 0;
        _player = FindAnyObjectByType<TopDownPlayer>().gameObject;
    }

    void Update()
    {
        if (_deltaTime < ArrowDisplayTime)
        {
            float angle = Mathf.Atan2(_player.transform.position.y - _TargetPos.y, _player.transform.position.x - _TargetPos.x) * Mathf.Rad2Deg;
            this.transform.rotation = Quaternion.AngleAxis(angle + 180, Vector3.forward);
            this.transform.localPosition = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * (-1), Mathf.Sin(angle * Mathf.Deg2Rad) * (-1), 0);

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