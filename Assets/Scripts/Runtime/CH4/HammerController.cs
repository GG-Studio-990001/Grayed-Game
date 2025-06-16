using UnityEngine;
using InputOld = UnityEngine.Input;

namespace Runtime.CH4
{
    public class HammerController : MonoBehaviour
    {
        [Header("망치 손잡이 끝 (항아리 중심)")]
        [SerializeField] private Transform grip;

        [Header("망치 머리 Transform")]
        [SerializeField] private Transform head;

        [Header("플레이어(항아리) Transform")]
        [SerializeField] private Transform player;

        void Start()
        {
            // 로컬 위치 고정 설정 (길이 5.0 기준)
            grip.localPosition = new Vector3(0f, -2.5f, 0f);
            head.localPosition = new Vector3(0f, 2.5f, 0f);
        }

        void Update()
        {
            if (player == null || grip == null || head == null) return;

            transform.position = player.position;

            Vector3 mousePos = InputOld.mousePosition;
            mousePos.z = 10f;
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mousePos);
            mouseWorld.z = 0f;

            Vector3 direction = (mouseWorld - transform.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }
}