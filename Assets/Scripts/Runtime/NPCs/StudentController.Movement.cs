using UnityEngine;

public partial class StudentController : MonoBehaviour
{
    private void Update()
    {
        // Debug mỗi 2 giây
        if (Time.frameCount % 120 == 0)
        {
            Debug.Log($"[StudentController] Update: {gameObject.name}, isDead={isDead}, hasLeftQueue={hasLeftQueue}, isCrossing={isCrossing}, pos={transform.position}");
        }
        
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
        if (rb == null)
        {
            Debug.LogWarning($"[StudentController] FixedUpdate: rb is null on {gameObject.name}");
            return;
        }
        
        if (isDead)
            return;

        if (currentVelocity.sqrMagnitude > 0.001f)
        {
            rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
        }
    }

    /// <summary>
    /// Di chuyển từ vị trí hiện tại tới vị trí chờ (slot) ở bên phải.
    /// </summary>
    private void MoveToWaitPoint()
    {
        Vector2 currentPos = transform.position;
        Vector2 targetPos = waitPointRight;

        float distance = Vector2.Distance(currentPos, targetPos);
        
        // Debug log mỗi giây
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"[StudentController] MoveToWaitPoint: pos={currentPos}, target={targetPos}, dist={distance}, rb={rb != null}");
        }
        
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
