using System.Collections;
using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    [Header("Hit / Knockback Settings")]
    [SerializeField] private string vehicleTag = "Vehicle";
    [Tooltip("Khoảng đẩy player ra khỏi xe khi bị tông.")]
    [SerializeField] private float knockbackDistance = 0.7f;

    private static readonly int AnimHit = Animator.StringToHash("Hit");
    private Coroutine stunCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(vehicleTag))
            return;

        HandleHitByVehicle(other);
    }

    private void HandleHitByVehicle(Collider2D vehicleCollider)
    {
        // 1) Gọi animation Hit
        if (animator != null)
        {
            animator.SetTrigger(AnimHit);
        }

        // Đảm bảo scale đúng khi bị hit
        transform.localScale = new Vector3(0.9f, 0.9f, 1f);

        // 2) Đẩy player theo PHƯƠNG NGANG (X) thôi
        if (rb != null)
        {
            float playerX = rb.position.x;
            float carX = vehicleCollider.bounds.center.x;

            // Nếu xe bên trái → đẩy player sang phải, và ngược lại
            float directionX = (playerX >= carX) ? 1f : -1f;

            Vector2 knockDir = new Vector2(directionX, 0f); // chỉ ngang
            Vector2 newPos = rb.position + knockDir * knockbackDistance;

            rb.position = newPos;
        }

        // 3) Bật stun để player không di chuyển được
        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);
        stunCoroutine = StartCoroutine(StunRoutine());

        // 4) Sau này nếu cần báo GameManager thì gọi ở đây
        // GameManager.Instance?.OnPlayerHit();
    }

    private IEnumerator StunRoutine()
    {
        isStunned = true;
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
        stunCoroutine = null;
    }
}
