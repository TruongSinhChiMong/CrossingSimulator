using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Animator")]
    public string pSpeed = "Speed";
    public string pIdle = "IsIdle";
    public string pWalk = "IsWalking";

    [Header("Sorting")]
    public int baseSortingOrder = 10; // Order cơ bản

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private bool isWalking;
    private bool isBlockedByVehicle = false;

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
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

    // Called by PlayerInput component (Send Messages / Broadcast Messages behavior)
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log($"OnMove called: {moveInput}");
    }

    // Alternative: Called when using Invoke Unity Events behavior
    public void OnMoveCallback(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        Debug.Log($"OnMoveCallback called: {moveInput}");
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Dừng lại nếu đang bị xe chặn
        if (isBlockedByVehicle)
        {
            rb.linearVelocity = Vector2.zero;
            UpdateAnimator(false);
            return;
        }

        // Apply movement
        rb.linearVelocity = moveInput * moveSpeed;

        // Check if walking
        isWalking = rb.linearVelocity.sqrMagnitude > 0.001f;

        UpdateAnimator(isWalking);

        // Flip sprite
        if (moveInput.x != 0f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(moveInput.x);
            transform.localScale = scale;
        }
    }

    void UpdateAnimator(bool walking)
    {
        if (anim == null) return;

        float speedValue = walking ? rb.linearVelocity.magnitude : 0f;
        anim.SetFloat(pSpeed, speedValue);
        anim.SetBool(pIdle, !walking);
        anim.SetBool(pWalk, walking);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Player chạm xe → dừng lại
        if (col.collider.CompareTag("Vehicle"))
        {
            isBlockedByVehicle = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        // Xe đi qua → player có thể di chuyển lại
        if (col.collider.CompareTag("Vehicle"))
        {
            isBlockedByVehicle = false;
        }
    }
}
