using UnityEngine;
using UnityEngine.InputSystem; // NEW Input System

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 30f;

    [Header("Signals (Z/X)")]
    [SerializeField] private float crossDuration = 0.6f;   // thời gian bật state Cross
    [SerializeField] private float stopDuration = 0.6f;   // thời gian bật state Stop

    // Animator params (đặt đúng tên trong Animator)
    private static readonly int AnimSpeed = Animator.StringToHash("Speed");
    private static readonly int AnimIsCrossing = Animator.StringToHash("IsCrossing");
    private static readonly int AnimIsStopping = Animator.StringToHash("IsStopping");

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;

    // --- NEW INPUT SYSTEM actions ---
    private InputAction moveAction;   // trục -1..1
    private InputAction crossAction;  // Z / gamepad south
    private InputAction stopAction;   // X / gamepad east

    private Coroutine crossRoutine, stopRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        // ----------------- MOVE (1D Axis) -----------------
        moveAction = new InputAction(
            name: "Move",
            type: InputActionType.Value
        );

        // 1D Axis composite cho bàn phím
        var axis = moveAction.AddCompositeBinding("1DAxis");
        axis.With("Negative", "<Keyboard>/a");
        axis.With("Negative", "<Keyboard>/leftArrow");
        axis.With("Positive", "<Keyboard>/d");
        axis.With("Positive", "<Keyboard>/rightArrow");

        // Thêm gamepad (left stick X) – optional, sẽ “pick” tín hiệu mạnh hơn
        moveAction.AddBinding("<Gamepad>/leftStick/x");

        // ----------------- Z (Cross) -----------------
        crossAction = new InputAction("Cross", InputActionType.Button);
        crossAction.AddBinding("<Keyboard>/z");
        crossAction.AddBinding("<Gamepad>/buttonSouth"); // A (Xbox) / Cross (PS)

        // ----------------- X (Stop) ------------------
        stopAction = new InputAction("Stop", InputActionType.Button);
        stopAction.AddBinding("<Keyboard>/x");
        stopAction.AddBinding("<Gamepad>/buttonEast");   // B (Xbox) / Circle (PS)
    }

    private void OnEnable()
    {
        moveAction.Enable();

        crossAction.performed += OnCrossPerformed;
        crossAction.Enable();

        stopAction.performed += OnStopPerformed;
        stopAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();

        crossAction.performed -= OnCrossPerformed;
        crossAction.Disable();

        stopAction.performed -= OnStopPerformed;
        stopAction.Disable();
    }

    private void FixedUpdate()
    {
        float inputX = moveAction.ReadValue<float>(); // -1..1 (từ 1DAxis hoặc leftStick/x)

        // Nếu đang bật Stop thì khóa chuyển động ngang
        if (animator.GetBool(AnimIsStopping))
            inputX = 0f;

        float targetVx = inputX * moveSpeed;
        float currentVx = rb.linearVelocity.x;
        float accel = (Mathf.Abs(targetVx) > 0.01f) ? acceleration : deceleration;

        float newVx = Mathf.MoveTowards(currentVx, targetVx, accel * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newVx, rb.linearVelocity.y);

        // Flip sprite theo hướng di chuyển
        if (Mathf.Abs(newVx) > 0.001f)
        {
            if (sr != null) sr.flipX = newVx < 0f;
        }

        // Cập nhật Animator
        animator.SetFloat(AnimSpeed, Mathf.Abs(newVx));
    }

    // ===================== HANDLERS =====================

    private void OnCrossPerformed(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (crossRoutine != null) StopCoroutine(crossRoutine);
        crossRoutine = StartCoroutine(SetBoolForSeconds(AnimIsCrossing, crossDuration));
    }

    private void OnStopPerformed(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (stopRoutine != null) StopCoroutine(stopRoutine);
        stopRoutine = StartCoroutine(SetBoolForSeconds(AnimIsStopping, stopDuration));
        // dừng gấp tức thời (tùy thích)
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private System.Collections.IEnumerator SetBoolForSeconds(int boolHash, float seconds)
    {
        animator.SetBool(boolHash, true);
        yield return new WaitForSeconds(seconds);
        animator.SetBool(boolHash, false);
    }
}
