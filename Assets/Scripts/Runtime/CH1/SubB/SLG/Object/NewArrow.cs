using UnityEngine;

public class NewArrow : MonoBehaviour
{
    public Transform player;  // 플레이어 Transform
    public Transform target;  // 목표 (건물)
    public Rigidbody2D rb;    // Rigidbody2D (화살표 이동을 부드럽게 처리)

    public float arrowDistance = 1.15f;  // 거리 1.15로 고정
    public float rotationSmoothTime = 0.05f; // 회전 보간 속도
    private float currentVelocity; // SmoothDampAngle 속도 저장용

    private void FixedUpdate() // 물리 업데이트 활용
    {
        if (rb.gameObject.activeSelf) // 활성화 상태일 때만 실행
        {
            ShowArrowTo(); // 부드럽게 위치 및 회전 조정
        }
    }

    public void SetArrowActive(bool active)
    {
        if (active)
        {
            if (target == null) return;

            // ✅ 물리 엔진 비활성화 (즉시 값 반영하기 위해)
            rb.simulated = false;

            // ✅ 즉시 방향 계산
            Vector3 direction = (target.position - player.position).normalized;
            Vector3 desiredPosition = player.position + direction * arrowDistance;

            // ✅ 즉시 위치 & 회전값 적용 (딜레이 없음)
            rb.position = desiredPosition;

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = targetAngle;

            // ✅ 이전 회전 속도 영향 제거
            currentVelocity = 0;

            // ✅ 물리 엔진 다시 활성화 (FixedUpdate에서 정상 작동하도록)
            rb.simulated = true;
        }

        rb.gameObject.SetActive(active);
    }

    private void ShowArrowTo()
    {
        if (target == null) return;

        Vector3 direction = (target.position - player.position).normalized;
        Vector3 desiredPosition = player.position + direction * arrowDistance;

        // ✅ FixedUpdate에서 부드러운 위치 보정
        rb.MovePosition(desiredPosition);

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float currentAngle = rb.rotation;
        float smoothedAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref currentVelocity, rotationSmoothTime);

        rb.MoveRotation(smoothedAngle);
    }
}
