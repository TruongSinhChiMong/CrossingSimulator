using UnityEngine;

public partial class StudentController : MonoBehaviour
{
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
                // phát animation la hét
                if (animator != null)
                {
                    animator.SetTrigger(Animator.StringToHash("Yell"));
                }

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
}
