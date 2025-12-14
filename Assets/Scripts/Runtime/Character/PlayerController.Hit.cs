using UnityEngine;

public partial class PlayerController : MonoBehaviour
{
    [Header("Hit / Knockback Settings")]
    [SerializeField] private string vehicleTag = "Vehicle";
    [Tooltip("Khoảng đẩy player ra khỏi xe khi bị tông.")]
    [SerializeField] private float knockbackDistance = 0.7f;

    private static readonly int AnimHit = Animator.StringToHash("Hit");

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(vehicleTag))
            return;

        HandleHitByVehicle(other);
    }

    private void HandleHitByVehicle(Collider2D vehicleCollider)
    {
        // 1) Gọi animation Hit
        var animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger(AnimHit);
        }

        // 2) Đẩy player theo PHƯƠNG NGANG (X) thôi
        var rb = GetComponent<Rigidbody2D>();
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

        // 3) Sau này nếu cần báo GameManager thì gọi ở đây
        // GameManager.Instance?.OnPlayerHit();
    }
}
