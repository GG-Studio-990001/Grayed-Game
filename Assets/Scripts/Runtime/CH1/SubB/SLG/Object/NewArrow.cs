using UnityEngine;

public class NewArrow : MonoBehaviour
{
    public Transform player;  // 플레이어 Transform
    public Transform target;  // 목표 (건물)
    public Rigidbody2D rb;    // Rigidbody2D (화살표 이동을 부드럽게 처리)

    public float arrowDistance = 1.15f;  // 거리 1.15로 고정
    public float rotationSmoothTime = 0.05f; // 회전 보간 속도
    private float currentVelocity; // SmoothDampAngle 속도 저장용

    void FixedUpdate() // 물리 업데이트 활용
    {
        ShowArrowTo();
    }

    void ShowArrowTo()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position;
        Vector3 direction = (targetPosition - player.position).normalized;
        Vector3 desiredPosition = player.position + direction * arrowDistance;

        rb.MovePosition(desiredPosition);

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float currentAngle = rb.rotation;
        float smoothedAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref currentVelocity, rotationSmoothTime);

        rb.MoveRotation(smoothedAngle);
    }
}
