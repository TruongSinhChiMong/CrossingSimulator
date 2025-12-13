using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Xử lý di chuyển trái/phải, flip sprite, cập nhật Speed.
    /// </summary>
    private void FixedUpdate()
    {
        // Nếu đang bị choáng do xe tông thì không cho điều khiển
        if (isStunned)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat(AnimSpeed, 0f);
            return;
        }

        float inputX = moveAction.ReadValue<float>(); // -1..1

        // Nếu đang bật Stop thì khóa chuyển động ngang
        if (animator.GetBool(AnimIsStopping))
            inputX = 0f;

        float targetVx = inputX * moveSpeed;
        float currentVx = rb.linearVelocity.x;
        float accel = (Mathf.Abs(targetVx) > 0.01f) ? acceleration : deceleration;

        float newVx = Mathf.MoveTowards(
            currentVx,
            targetVx,
            accel * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(newVx, rb.linearVelocity.y);

        // Flip sprite theo hướng di chuyển
        if (Mathf.Abs(newVx) > 0.001f && sr != null)
        {
            sr.flipX = newVx < 0f;
        }

        // Cập nhật Animator (Speed)
        animator.SetFloat(AnimSpeed, Mathf.Abs(newVx));
    }
}
