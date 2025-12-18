using UnityEngine;

public partial class StudentController : MonoBehaviour
{
    // Cache hash để tránh tính lại mỗi frame
    private static readonly int AnimYellTrigger = Animator.StringToHash("Yell");
    
    private void UpdateYell(float deltaTime)
    {
        if (!enableAutoYell)
            return;

        // đã chết, đã báo kết quả hoặc đã rời hàng thì không auto yell nữa
        if (isDead || hasReportedResult || hasLeftQueue)
        {
            ResetYellState();
            return;
        }

        // chỉ auto yell + auto đi khi:
        // - đã đứng đúng slot chờ
        // - và đang là thằng đầu hàng
        if (!reachedWaitPoint || spawner == null || !spawner.IsFrontStudent(this))
        {
            ResetYellState();
            return;
        }

        stoppedTimer += deltaTime;

        if (!hasYelledOnce)
        {
            if (stoppedTimer >= waitBeforeFirstYell)
            {
                // phát animation la hét (bỏ qua nếu animator không có parameter này)
                TrySetYellAnimation();

                hasYelledOnce = true;

                // sau khi hét xong thì tự băng qua đường
                isCrossing = true;
                isStopped = false;

                if (!hasLeftQueue)
                {
                    hasLeftQueue = true;
                    if (spawner != null)
                    {
                        spawner.OnStudentLeftQueue(this);
                    }
                }
            }
        }
        else
        {
            // nếu sau này muốn hét nhiều lần thì dùng yellInterval ở đây
        }
    }
    
    /// <summary>
    /// Thử set animation Yell, bỏ qua nếu animator không có parameter này
    /// </summary>
    private void TrySetYellAnimation()
    {
        if (animator == null) return;
        
        // Kiểm tra xem animator có parameter "Yell" không
        foreach (var param in animator.parameters)
        {
            if (param.nameHash == AnimYellTrigger && param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.SetTrigger(AnimYellTrigger);
                return;
            }
        }
        // Không có parameter thì bỏ qua, không log warning để tránh spam
    }
}
