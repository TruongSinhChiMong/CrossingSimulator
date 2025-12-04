using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class VehicleController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public bool moveDown = true; // true = đi xuống (Y giảm), false = đi lên (Y tăng)

    [Header("Auto Despawn")]
    public float despawnDistance = 15f; // Khoảng cách từ spawn point để tự hủy

    [Header("Collider")]
    [Range(0.3f, 1f)]
    public float colliderScale = 0.6f; // Thu nhỏ collider (0.6 = 60% kích thước sprite)

    [Header("Sorting")]
    public int baseSortingOrder = 10; // Order cơ bản

    private Rigidbody2D rb;
    private Vector3 startPos;
    private bool isBlockedByPlayer = false;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Kinematic; // Xe không bị đẩy bởi player
        startPos = transform.position;

        // Đảm bảo có tag Vehicle
        if (!gameObject.CompareTag("Vehicle"))
        {
            Debug.LogWarning($"[Vehicle] {gameObject.name} chưa có tag 'Vehicle'!");
        }

        // Thu nhỏ collider
        AdjustCollider();

        // Lấy sprite renderer
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Cập nhật sorting order dựa trên vị trí Y
        // Object có Y thấp hơn (ở dưới) sẽ có order cao hơn (hiển thị trước)
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = baseSortingOrder - Mathf.RoundToInt(transform.position.y * 10);
        }
    }

    void AdjustCollider()
    {
        // Box Collider 2D
        BoxCollider2D boxCol = GetComponent<BoxCollider2D>();
        if (boxCol != null)
        {
            boxCol.size *= colliderScale;
            return;
        }

        // Circle Collider 2D
        CircleCollider2D circleCol = GetComponent<CircleCollider2D>();
        if (circleCol != null)
        {
            circleCol.radius *= colliderScale;
            return;
        }

        // Capsule Collider 2D
        CapsuleCollider2D capsuleCol = GetComponent<CapsuleCollider2D>();
        if (capsuleCol != null)
        {
            capsuleCol.size *= colliderScale;
        }
    }

    void FixedUpdate()
    {
        // Dừng lại nếu bị player chặn
        if (isBlockedByPlayer)
        {
            return;
        }

        // Di chuyển theo trục Y (dùng MovePosition cho Kinematic)
        float direction = moveDown ? -1f : 1f;
        Vector2 movement = new Vector2(0f, moveSpeed * direction) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Flip sprite theo hướng đi (rotate 180 nếu đi lên)
        if (!moveDown)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        }

        // Tự hủy khi đi quá xa
        if (Vector3.Distance(transform.position, startPos) > despawnDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Xe gặp player → dừng lại
        if (col.CompareTag("Player"))
        {
            isBlockedByPlayer = true;
            Debug.Log("[Vehicle] Stopped - Player in front!");
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        // Player đi khỏi → xe tiếp tục chạy
        if (col.CompareTag("Player"))
        {
            isBlockedByPlayer = false;
            Debug.Log("[Vehicle] Resuming movement");
        }
    }
}
