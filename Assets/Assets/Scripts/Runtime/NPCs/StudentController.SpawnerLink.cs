using UnityEngine;

public partial class StudentController : MonoBehaviour
{
    /// <summary>
    /// Gọi ngay sau khi Instantiate từ StudentSpawner.
    /// Spawner đặt vị trí spawn bên trong tường phải.
    /// </summary>
    public void SetupFromSpawner(float leftGateX, Vector2 waitPointRight, StudentSpawner spawner)
    {
        this.leftGateX = leftGateX;
        this.waitPointRight = waitPointRight;
        this.spawner = spawner;

        ResetCoreState();
        ResetYellState();
    }

    /// <summary>
    /// Spawner gọi khi dồn hàng: cập nhật lại vị trí chờ (slot) mới.
    /// </summary>
    public void UpdateQueueWaitPoint(Vector2 newWaitPoint)
    {
        waitPointRight = newWaitPoint;
        reachedWaitPoint = false; // để MoveToWaitPoint đưa về chỗ mới
    }

    /// <summary>
    /// Được gọi khi va chạm với SafeZone hoặc đi qua mốc X bên trái.
    /// Không chặn học sinh: nó vẫn tiếp tục đi sang trái cho tới khi ra khỏi màn hình.
    /// </summary>
    private void HandleReachedSafeZone(Collider2D trigger)
    {
        if (hasReportedResult)
            return;

        hasReportedResult = true;

        if (spawner != null)
        {
            spawner.NotifyStudentSucceeded(this);
        }
    }

    /// <summary>
    /// Được gọi khi va chạm với Vehicle.
    /// Học sinh được tính là chết và "dính" vào xe trôi ra khỏi map.
    /// </summary>
    private void HandleHitByVehicle(Collider2D vehicleCollider)
    {
        if (hasReportedResult)
            return;

        hasReportedResult = true;
        isDead = true;
        isCrossing = false;
        isStopped = false;
        currentVelocity = Vector2.zero;

        // nếu còn trong hàng thì cho rời hàng & dồn hàng luôn
        if (!hasLeftQueue)
        {
            hasLeftQueue = true;
            if (spawner != null)
            {
                spawner.OnStudentLeftQueue(this);
            }
        }

        if (animator != null)
        {
            animator.SetTrigger(AnimDie);
        }

        // bỏ điều khiển vật lý của học sinh
        if (rb != null)
        {
            rb.simulated = false;
        }

        // tắt collider để không va đụng thêm lần nữa
        foreach (var col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }

        // "dính" vào xe: set parent
        Transform vehicleTransform = vehicleCollider.GetComponentInParent<Transform>();
        if (vehicleTransform != null)
        {
            transform.SetParent(vehicleTransform, true);
        }

        if (spawner != null)
        {
            spawner.NotifyStudentDied(this);
        }
    }
}
