using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.CH4
{
    public class SimpleFollowMouse : MonoBehaviour
    {
        void Update()
        {
            Vector3 mousePos = UnityEngine.Input.mousePosition;
            mousePos.z = 10f; // 카메라에서의 거리
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = worldPos;
        }
    }
}