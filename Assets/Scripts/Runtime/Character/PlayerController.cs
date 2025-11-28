using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // Cho các script khác truy cập Player nếu cần
    public static PlayerController Instance;

    [Header("Movement")]
    [Tooltip("Tốc độ di chuyển của Joe")]
    public float moveSpeed = 3f;

    [Header("Animator")]
    [Tooltip("Tên parameter float trong Animator dùng cho tốc độ")]
    public string pSpeed = "Speed";

    Rigidbody2D rb;
    Animator anim;
    Vector2 move;

    void Awake()
    {
        // Gán instance, không phá huỷ gì thêm cho đỡ rắc rối
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Cấu hình Rigidbody2D cho game 2D, không bị rơi
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    // Được gọi bởi PlayerInput (action Move)
    public void OnMove(InputAction.CallbackContext ctx)
    {
        move = ctx.ReadValue<Vector2>();
        // Debug tạm, sau này có thể xoá:
        // if (ctx.performed || ctx.canceled) Debug.Log("Move: " + move);
    }

    // Được gọi bởi action Z
    public void OnActionZ(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Debug.Log("Z pressed");
            // TODO: sau này gọi OrdersManager.EmitCross();
        }
    }

    // Được gọi bởi action X
    public void OnActionX(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Debug.Log("X pressed");
            // TODO: sau này gọi OrdersManager.EmitStop();
        }
    }

    void FixedUpdate()
    {
        // Di chuyển bằng Rigidbody2D
        rb.linearVelocity = move * moveSpeed;

        // Cập nhật Animator (Speed)
        if (anim)
        {
            float speedValue = rb.linearVelocity.sqrMagnitude > 0.001f
                ? rb.linearVelocity.magnitude
                : 0f;

            anim.SetFloat(pSpeed, speedValue);
        }

        // Lật sprite theo hướng ngang
        if (move.x != 0f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(move.x);
            transform.localScale = scale;
        }
    }
}
