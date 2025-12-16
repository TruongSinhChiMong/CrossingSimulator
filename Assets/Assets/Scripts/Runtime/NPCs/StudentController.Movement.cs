using UnityEngine;

public partial class StudentController : MonoBehaviour
{
    private void Update()
    {
        // cập nhật logic la hét / auto đi (nếu bật)
        UpdateYell(Time.deltaTime);

        if (isDead)
        {
            currentVelocity = Vector2.zero;
            UpdateAnimatorByVelocity();
            return;
        }

        // 1) Còn đang trong hàng chờ (chưa rời hàng)
        if (!hasLeftQueue)
        {
            MoveToWaitPoint();
            return;
        }

        // 2) Đã rời hàng nhưng đang bị Stop hoặc đã qua SafeZone → đứng yên
        if (!isCrossing || isStopped)
        {
            currentVelocity = Vector2.zero;
            UpdateAnimatorByVelocity();
            return;
        }

        // 3) Đang băng qua đường → đi sang bên trái
        currentVelocity = Vector2.left * moveSpeed;

        // Backup: nếu không dùng SafeZone, qua mốc X bên trái thì coi như thành công
        if (!hasReportedResult && transform.position.x <= leftGateX)
        {
            HandleReachedSafeZone(null);
        }

        UpdateAnimatorByVelocity();
    }

    private void FixedUpdate()
    {
        if (rb == null || isDead)
            return;

        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Di chuyển từ vị trí hiện tại tới vị trí chờ (slot) ở bên phải.
    /// </summary>
    private void MoveToWaitPoint()
    {
        Vector2 currentPos = transform.position;
        Vector2 targetPos = waitPointRight;

        float distance = Vector2.Distance(currentPos, targetPos);
        if (distance <= 0.02f)
        {
            // tới đúng slot chờ
            transform.position = targetPos;
            currentVelocity = Vector2.zero;
            reachedWaitPoint = true;

            UpdateAnimatorByVelocity();
            return;
        }

        Vector2 dir = (targetPos - currentPos).normalized;
        currentVelocity = dir * moveSpeed;

        UpdateAnimatorByVelocity();
    }
}
