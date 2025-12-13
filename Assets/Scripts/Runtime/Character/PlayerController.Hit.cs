using System.Collections;
using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Coroutine choáng – khóa điều khiển trong thời gian stunDuration.
    /// </summary>
    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    // ======= API cho VehicleController gọi nếu cần =======

    public void HitByVehicle()
    {
        HitByVehicle(Vector2.left, knockbackForce, stunDuration);
    }

    public void HitByVehicle(Vector2 direction)
    {
        HitByVehicle(direction, knockbackForce, stunDuration);
    }

    public void HitByVehicle(Vector2 direction, float force)
    {
        HitByVehicle(direction, force, stunDuration);
    }

    public void HitByVehicle(Vector2 direction, float force, float stunTime)
    {
        if (direction == Vector2.zero)
            direction = Vector2.left;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);

        if (gameObject.activeInHierarchy)
            StartCoroutine(StunCoroutine(stunTime));
    }

    /// <summary>
    /// Va chạm trực tiếp với collider tag "Vehicle".
    /// </summary>
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Vehicle"))
        {
            Vector2 dir = (transform.position - col.transform.position).normalized;
            HitByVehicle(dir, knockbackForce, stunDuration);
        }
    }
}
