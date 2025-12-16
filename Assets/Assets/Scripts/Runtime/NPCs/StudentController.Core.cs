using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public partial class StudentController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [Tooltip("Tốc độ đi của học sinh.")]
    [SerializeField] private float moveSpeed = 2f;

    [Header("Tags")]
    [SerializeField] private string vehicleTag = "Vehicle";
    [SerializeField] private string safeZoneTag = "SafeZone";

    // --- STATE CHUNG ---
    protected bool isCrossing;          // đang băng qua
    protected bool isStopped;           // đang bị Stop
    protected bool isDead;              // bị xe tông

    protected Vector2 currentVelocity;  // vận tốc hiện tại

    // --- SPAWNER / MAP ---
    protected float leftGateX;          // mốc X bên trái (fallback nếu không dùng SafeZone)
    protected Vector2 waitPointRight;   // vị trí chờ bên phải (slot hiện tại trong hàng)
    protected StudentSpawner spawner;   // spawner đã tạo ra mình
    protected bool reachedWaitPoint;    // đã tới đúng vị trí chờ chưa
    protected bool hasReportedResult;   // đã báo sống / chết cho spawner chưa
    protected bool hasLeftQueue;        // đã rời hàng chờ (bắt đầu băng qua / chết) chưa

    // --- YELL (la hét) ---
    [Header("Yell Settings")]
    [SerializeField] private bool enableAutoYell = true;
    [SerializeField] private float waitBeforeFirstYell = 1.5f;
    [SerializeField] private float yellInterval = 3f;

    protected bool hasYelledOnce;
    protected float stoppedTimer;
    protected float yellTimer;

    // Animator parameter IDs
    private static readonly int AnimSpeed = Animator.StringToHash("Speed");
    private static readonly int AnimYell = Animator.StringToHash("Yell");
    private static readonly int AnimDie = Animator.StringToHash("Die");

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();

        ResetCoreState();
        ResetYellState();
    }

    private void ResetCoreState()
    {
        isCrossing = false;
        isStopped = false;
        isDead = false;
        hasReportedResult = false;
        reachedWaitPoint = false;
        hasLeftQueue = false;
        currentVelocity = Vector2.zero;
    }

    private void ResetYellState()
    {
        hasYelledOnce = false;
        stoppedTimer = 0f;
        yellTimer = 0f;
    }

    private void UpdateAnimatorByVelocity()
    {
        if (animator == null) return;

        float speed = isDead ? 0f : currentVelocity.magnitude;
        animator.SetFloat(AnimSpeed, speed);
    }
}
