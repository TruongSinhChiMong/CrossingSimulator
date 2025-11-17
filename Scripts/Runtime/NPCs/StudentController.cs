using System.Collections;
using UnityEngine;

/// <summary>
/// Student/NPC logic: đi sang trái, dừng theo lệnh, nhìn lên khi gần player,
/// chờ 15s thì la hét 15s; nếu hết 15s vẫn chưa được lệnh qua đường thì tự qua.
/// Không còn field isYelling thừa – trạng thái đọc/ghi trực tiếp từ Animator.
/// </summary>
[RequireComponent(typeof(Animator))]
public class StudentController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.2f;      // tốc độ đi bộ (m/s)
    [SerializeField] private bool startMoving = true;     // spawn ra là đi luôn

    [Header("Auto Despawn / Biến mất khi ra khỏi màn")]
    [SerializeField] private float despawnX = -12f;       // x nhỏ hơn giá trị này thì hủy object
    [SerializeField] private bool destroyOnWallHit = true;// nếu đụng tường (MapCollision) thì hủy

    [Header("Yell (la hét)")]
    [SerializeField] private float waitBeforeYell = 15f;  // đứng chờ bao lâu thì bắt đầu hét
    [SerializeField] private float yellDuration = 15f;  // hét trong bao lâu

    [Header("Animator parameter names (điền ĐÚNG như trong Controller)")]
    [SerializeField] private string pSpeed = "Speed";
    [SerializeField] private string pNearPlayer = "NearPlayer"; // nếu bạn đang để "NearPlaye" thì gõ đúng tên đó
    [SerializeField] private string pIsYelling = "IsYelling";
    [SerializeField] private string pStopOrder = "StopOrder";
    [SerializeField] private string pIsHit = "IsHit";
    [SerializeField] private string pCrossOrder = "CrossOrder";

    // Cached
    private Animator animator;
    private Rigidbody2D rb;

    // Animator hashes (tính từ string để nhanh và tránh sai chính tả ở runtime)
    private int hSpeed, hNearPlayer, hIsYelling, hStopOrder, hIsHit, hCrossOrder;

    // State
    private bool nearPlayer = false;
    private bool stopOrder = false;     // đang bị giữ lại
    private bool crossOrder = false;    // được lệnh băng qua (hoặc auto sau khi hét)
    private bool isHit = false;         // bị xe tông
    private Coroutine yellCoro;

    // Property proxy vào Animator: không còn field isYelling thừa
    private bool IsYelling
    {
        get => animator.GetBool(hIsYelling);
        set => animator.SetBool(hIsYelling, value);
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); // có thể null nếu bạn không dùng RB

        hSpeed = Animator.StringToHash(pSpeed);
        hNearPlayer = Animator.StringToHash(pNearPlayer);
        hIsYelling = Animator.StringToHash(pIsYelling);
        hStopOrder = Animator.StringToHash(pStopOrder);
        hIsHit = Animator.StringToHash(pIsHit);
        hCrossOrder = Animator.StringToHash(pCrossOrder);
    }

    private void OnEnable()
    {
        // Trạng thái khởi tạo
        stopOrder = !startMoving;
        crossOrder = false;
        isHit = false;
        IsYelling = false;

        animator.SetFloat(hSpeed, startMoving ? moveSpeed : 0f);
        animator.SetBool(hStopOrder, stopOrder);
        animator.SetBool(hCrossOrder, false);
        animator.SetBool(hIsHit, false);
        animator.SetBool(hNearPlayer, false);

        TryStartYellRoutine(); // nếu spawn ra ở trạng thái dừng
    }

    private void Update()
    {
        // Rời màn hình -> hủy
        if (transform.position.x < despawnX)
        {
            Destroy(gameObject);
            return;
        }

        if (isHit)
        {
            animator.SetFloat(hSpeed, 0f);
            return;
        }

        // Di chuyển sang trái nếu không bị giữ
        float currentSpeed = stopOrder ? 0f : moveSpeed;

        if (currentSpeed > 0f)
        {
            if (rb != null)
                rb.MovePosition(rb.position + Vector2.left * currentSpeed * Time.deltaTime);
            else
                transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);
        }

        animator.SetFloat(hSpeed, currentSpeed);

        // Cập nhật nhìn lên khi ở gần player
        animator.SetBool(hNearPlayer, nearPlayer);

        // Nếu đang dừng mà chưa chạy coroutine thì khởi động
        TryStartYellRoutine();
    }

    private void TryStartYellRoutine()
    {
        if (stopOrder && yellCoro == null && !IsYelling)
            yellCoro = StartCoroutine(YellFlow());
    }

    private IEnumerator YellFlow()
    {
        // Chờ trước khi hét
        float t = 0f;
        while (t < waitBeforeYell && stopOrder && !crossOrder)
        {
            t += Time.deltaTime;
            yield return null;
        }
        if (!stopOrder || crossOrder) { yellCoro = null; yield break; }

        // Bắt đầu hét
        IsYelling = true;
        t = 0f;
        while (t < yellDuration && stopOrder && !crossOrder)
        {
            t += Time.deltaTime;
            yield return null;
        }
        IsYelling = false;

        // Hết thời gian mà vẫn chưa được lệnh -> tự băng qua
        if (stopOrder && !crossOrder)
            OrderCross();

        yellCoro = null;
    }

    #region Public API (gọi từ Player / Trigger)
    /// <summary> Player ra lệnh dừng. </summary>
    public void OrderStop(bool value = true)
    {
        stopOrder = value;
        animator.SetBool(hStopOrder, value);

        if (!value) // bỏ dừng
        {
            crossOrder = false; // chờ lệnh mới
            IsYelling = false;
            if (yellCoro != null) { StopCoroutine(yellCoro); yellCoro = null; }
        }
        else
        {
            TryStartYellRoutine();
        }
    }

    /// <summary> Player ra lệnh băng qua (hoặc bị auto sau khi hét). </summary>
    public void OrderCross()
    {
        crossOrder = true;
        stopOrder = false;

        animator.SetBool(hCrossOrder, true);
        animator.SetBool(hStopOrder, false);

        IsYelling = false;
        if (yellCoro != null) { StopCoroutine(yellCoro); yellCoro = null; }
    }

    /// <summary> Gần/xa player (set bởi trigger ở học sinh hoặc vùng xung quanh player). </summary>
    public void SetNearPlayer(bool value)
    {
        nearPlayer = value;
        animator.SetBool(hNearPlayer, value);
    }

    /// <summary> Khi bị xe tông. Có thể gắn vào OnCollisionEnter2D ở xe và gọi qua. </summary>
    public void SetHitByVehicle(Transform vehicle, bool attachToVehicle = true)
    {
        isHit = true;
        animator.SetBool(hIsHit, true);
        animator.SetFloat(hSpeed, 0f);
        IsYelling = false;

        if (attachToVehicle && vehicle != null)
        {
            transform.SetParent(vehicle, true);
        }
    }
    #endregion

    #region Simple triggers (tùy dùng)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) SetNearPlayer(true);

        if (destroyOnWallHit && other.gameObject.layer == LayerMask.NameToLayer("MapCollision"))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) SetNearPlayer(false);
    }
    #endregion
}
