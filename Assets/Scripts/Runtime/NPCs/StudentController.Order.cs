using UnityEngine;

public partial class StudentController : MonoBehaviour
{
    private void OnEnable()
    {
        OrdersManager.OnCross += HandleCrossOrder;
        OrdersManager.OnStop += HandleStopOrder;
    }

    private void OnDisable()
    {
        OrdersManager.OnCross -= HandleCrossOrder;
        OrdersManager.OnStop -= HandleStopOrder;
    }

    private void HandleCrossOrder()
    {
        // chỉ học sinh chưa chết, chưa báo kết quả mới nghe lệnh
        if (isDead || hasReportedResult)
            return;

        // chưa tới điểm chờ A thì cũng chưa băng qua đường
        if (!reachedWaitPoint)
            return;

        // Chỉ nghe lệnh nếu đang trong PlayerCommandZone
        if (!IsInPlayerCommandZone())
            return;

        isCrossing = true;
        isStopped = false;

        ResetYellState();

        // rời hàng chờ → báo spawner dồn hàng (nếu chưa rời)
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
            // Speed/Idle/Walk được điều khiển bởi currentVelocity
            animator.ResetTrigger(Animator.StringToHash("Yell"));
        }

        Debug.Log($"[StudentController] {gameObject.name} received CROSS order");
    }

    private void HandleStopOrder()
    {
        // chỉ học sinh chưa chết, chưa báo kết quả mới nghe lệnh
        if (isDead || hasReportedResult)
            return;

        // chưa tới điểm chờ A thì thôi
        if (!reachedWaitPoint)
            return;

        // Chỉ dừng nếu đang trong CrossingZone
        if (!isInCrossingZone)
            return;

        // Chỉ nghe lệnh nếu đang trong PlayerCommandZone
        if (!IsInPlayerCommandZone())
            return;

        isStopped = true;
        isCrossing = false;
        currentVelocity = Vector2.zero;

        ResetYellState();

        // Animation đứng yên (không Yell)
        UpdateAnimatorByVelocity();

        Debug.Log($"[StudentController] {gameObject.name} received STOP order");
    }

    /// <summary>
    /// Kiểm tra student có đang trong vùng ảnh hưởng của Player không.
    /// </summary>
    private bool IsInPlayerCommandZone()
    {
        if (PlayerCommandZone.Instance == null)
        {
            // Nếu không có PlayerCommandZone, cho phép nghe lệnh (backward compatible)
            return true;
        }
        return PlayerCommandZone.Instance.IsStudentInZone(this);
    }
}
