using System.Collections;
using UnityEngine;

/// <summary>
/// Điều khiển 1 học sinh:
/// - Đi về bên trái; giữ khoảng cách với học sinh trước mặt.
/// - Đợi 15s -> Yell 15s -> nếu chưa nhận lệnh thì tự qua đường.
/// - Gần Player thì ngước đầu + hiện Arrow (child).
/// - Nhận lệnh toàn cục từ OrdersManager: Z (Cross), X (Stop).
/// - Bị xe tông thì dính vào xe, rời màn.
/// - Đụng "Despawn" (trigger tường mép) thì Destroy.
/// </summary>
[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class StudentController : MonoBehaviour
{
    // --------- Inspector ----------
    [Header("References")]
    [SerializeField] private Transform arrow;               // kéo child "Arrow" vào
    [SerializeField] private CircleCollider2D nearTrigger;  // kéo collider "NearTrigger" (IsTrigger = true) vào
    [SerializeField] private Transform player;              // kéo Player nếu muốn check distance (không bắt buộc)

    [Header("Move")]
    [SerializeField] private float walkSpeed = 1.6f;
    [SerializeField] private float accel = 8f;
    [SerializeField] private float personalSpace = 0.6f;    // khoảng cách không dính nhau
    [SerializeField] private LayerMask studentMask;         // chọn Layer Student

    [Header("Wait / Yell")]
    [SerializeField] private bool startsWaiting = true;     // sinh ra ở cột: chờ lệnh?
    [SerializeField] private float waitBeforeYell = 15f;    // 15s chờ
    [SerializeField] private float yellDuration = 15f;    // 15s la -> tự qua

    [Header("Despawn")]
    [SerializeField] private string despawnTag = "Despawn"; // tag của trigger mép phải

    // ----- Animator params (đảm bảo trùng tên trong Animator) -----
    static readonly int P_Speed = Animator.StringToHash("Speed");
    static readonly int P_NearPlayer = Animator.StringToHash("NearPlayer"); // nếu Animator đang lỡ là "NearPlaye", sửa param trong Animator hoặc đổi hằng số này
    static readonly int P_IsYelling = Animator.StringToHash("IsYelling");
    static readonly int P_StopOrder = Animator.StringToHash("StopOrder");
    static readonly int P_IsHit = Animator.StringToHash("IsHit");
    static readonly int P_CrossOrder = Animator.StringToHash("CrossOrder");

    // --------- Runtime ----------
    Animator anim;
    Rigidbody2D rb;
    float currentSpeed;
    bool stopRequested;
    bool crossRequested;
    bool isYelling;
    bool isHit;
    bool nearPlayer; // trạng thái gần Player (để bật Arrow/LookUp)

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        OrdersManager.OnStopOrder += HandleStop;
        OrdersManager.OnCrossOrder += HandleCross;

        if (startsWaiting) StartCoroutine(WaitThenYellThenAutoCross());
    }

    void OnDisable()
    {
        OrdersManager.OnStopOrder -= HandleStop;
        OrdersManager.OnCrossOrder -= HandleCross;
    }

    void Start()
    {
        // Nếu không chờ thì cho phép qua luôn
        crossRequested = !startsWaiting;

        if (arrow) arrow.gameObject.SetActive(false);
        UpdateAnimatorImmediate();
    }

    void Update()
    {
        // Nếu muốn không dùng nearTrigger, có thể fallback theo khoảng cách:
        if (!nearTrigger && player)
            SetNear(Vector2.Distance(transform.position, player.position) < 1.3f);

        if (arrow) arrow.gameObject.SetActive(nearPlayer);

        // Tốc độ mục tiêu theo lệnh
        float target = (!stopRequested && crossRequested && !isHit) ? walkSpeed : 0f;

        // Chống dính nhau: check 1 vòng tròn nhỏ phía trước (hướng trái)
        if (target > 0f)
        {
            Vector2 probe = (Vector2)transform.position + Vector2.left * personalSpace;
            var hit = Physics2D.OverlapCircle(probe, 0.18f, studentMask);
            if (hit && hit.transform != transform) target = 0f;
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, target, accel * Time.deltaTime);
        anim.SetFloat(P_Speed, currentSpeed);
        anim.SetBool(P_NearPlayer, nearPlayer);
    }

    void FixedUpdate()
    {
        // Di chuyển sang trái theo currentSpeed
        rb.MovePosition(rb.position + Vector2.left * currentSpeed * Time.fixedDeltaTime);
    }

    // ---------- Orders ----------
    void HandleStop()
    {
        stopRequested = true;
        anim.SetBool(P_StopOrder, true);
    }

    void HandleCross()
    {
        crossRequested = true;
        stopRequested = false;
        anim.SetBool(P_StopOrder, false);
        anim.SetBool(P_CrossOrder, true);
    }

    // ---------- Wait -> Yell -> AutoCross ----------
    IEnumerator WaitThenYellThenAutoCross()
    {
        float t = 0f;
        while (t < waitBeforeYell && !crossRequested) { t += Time.deltaTime; yield return null; }

        if (!crossRequested)
        {
            isYelling = true;
            anim.SetBool(P_IsYelling, true);

            float y = 0f;
            while (y < yellDuration && !crossRequested) { y += Time.deltaTime; yield return null; }

            anim.SetBool(P_IsYelling, false);
            isYelling = false;

            if (!crossRequested) HandleCross(); // hết la vẫn chưa có lệnh -> tự qua
        }
    }

    // ---------- Collisions / Triggers ----------
    // nearTrigger gặp Player: bật/tắt NearPlayer
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) { SetNear(true); return; }

        if (other.CompareTag(despawnTag))
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) SetNear(false);
    }

    // Bị xe tông (va chạm cứng với Vehicle layer)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
        {
            isHit = true;
            anim.SetBool(P_IsHit, true);

            // dính vào xe, tắt physics mình
            rb.simulated = false;
            transform.SetParent(collision.transform, true);

            var caps = GetComponent<CapsuleCollider2D>(); if (caps) caps.enabled = false;
            if (nearTrigger) nearTrigger.enabled = false;
        }
    }

    void SetNear(bool value)
    {
        nearPlayer = value;
    }

    void UpdateAnimatorImmediate()
    {
        anim.SetFloat(P_Speed, 0f);
        anim.SetBool(P_StopOrder, startsWaiting);
        anim.SetBool(P_CrossOrder, !startsWaiting);
        anim.SetBool(P_IsYelling, false);
        anim.SetBool(P_IsHit, false);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Vector2 probe = (Vector2)transform.position + Vector2.left * personalSpace;
        Gizmos.DrawWireSphere(probe, 0.18f);
    }
#endif
}
