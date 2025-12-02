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

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveInput;
    private bool isWalking;

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    // IMPORTANT: This method is called by PlayerInput component
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        Debug.Log($"OnMove called: {moveInput}");
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Apply movement
        rb.linearVelocity = moveInput * moveSpeed;

        // Check if walking
        isWalking = rb.linearVelocity.sqrMagnitude > 0.001f;

        // Update animator
        if (anim != null)
        {
            float speedValue = isWalking ? rb.linearVelocity.magnitude : 0f;
            anim.SetFloat(pSpeed, speedValue);
            anim.SetBool(pIdle, !isWalking);
            anim.SetBool(pWalk, isWalking);
        }

        // Flip sprite
        if (moveInput.x != 0f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(moveInput.x);
            transform.localScale = scale;
        }
    }
}
