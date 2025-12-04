using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class StudentController : MonoBehaviour
{
    [Header("Movement")]
    public float MoveSpeed = 1.2f;
    public bool StartMoving = true;

    [Header("Auto Despawn")]
    public float DespawnX = -12f;          // đi tới trái màn hình thì biến mất
    public bool DestroyOnWallHit = true;   // chạm tường trái/phải thì huỷ

    [Header("Safe Zone")]
    public string safeZoneTag = "SafeZone"; // Tag của vùng an toàn

    [Header("Collider")]
    [Range(0.3f, 1f)]
    public float colliderScale = 0.5f; // Thu nhỏ collider để va chạm gần hơn

    [Header("Yell (la hét)")]
    public float WaitBeforeYell = 15f;
    public float YellDuration = 15f;

    [Header("Animator parameters (đúng tên trong Controller)")]
    public string P_Speed = "Speed";
    public string P_NearPlayer = "NearPlayer";
    public string P_IsYelling = "IsYelling";
    public string P_StopOrder = "StopOrder";
    public string P_IsHit = "IsHit";
    public string P_CrossOrder = "CrossOrder";

    Rigidbody2D rb;
    Animator anim;
    Transform player;
    bool isMoving;
    bool hasReachedSafety = false;
    bool isHit = false;

    public void SetPlayer(Transform p) => player = p;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        isMoving = StartMoving;
        AdjustCollider();
    }

    void AdjustCollider()
    {
        BoxCollider2D boxCol = GetComponent<BoxCollider2D>();
        if (boxCol != null)
        {
            boxCol.size *= colliderScale;
            return;
        }

        CircleCollider2D circleCol = GetComponent<CircleCollider2D>();
        if (circleCol != null)
        {
            circleCol.radius *= colliderScale;
            return;
        }

        CapsuleCollider2D capsuleCol = GetComponent<CapsuleCollider2D>();
        if (capsuleCol != null)
        {
            capsuleCol.size *= colliderScale;
        }
    }

    void Update()
    {
        // di chuyển cơ bản sang trái khi được phép
        if (isMoving)
        {
            rb.linearVelocity = new Vector2(-MoveSpeed, 0f);
            anim.SetFloat(P_Speed, Mathf.Abs(rb.linearVelocity.x));
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetFloat(P_Speed, 0f);
        }

        // tự huỷ khi ra khỏi màn hình trái
        if (transform.position.x <= DespawnX)
            Destroy(gameObject);
    }

    // Player gọi: ra lệnh dừng/đi
    public void OrderStop(bool stop)
    {
        isMoving = !stop;
        anim.SetBool(P_StopOrder, stop);
    }

    // Player gọi: cho phép băng qua
    public void OrderCross()
    {
        isMoving = true;
        anim.SetTrigger(P_CrossOrder);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!DestroyOnWallHit) return;

        if (col.collider.CompareTag("MapCollision") || col.collider.name.Contains("Wall"))
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Học sinh đến vùng an toàn
        if (col.CompareTag(safeZoneTag) && !hasReachedSafety && !isHit)
        {
            hasReachedSafety = true;
            GameManager.Instance?.OnStudentSafe();
            Debug.Log("[Student] Reached safety!");
        }

        // Học sinh bị xe đâm
        if (col.CompareTag("Vehicle") && !isHit)
        {
            isHit = true;
            isMoving = false;
            anim.SetBool(P_IsHit, true);
            GameManager.Instance?.OnStudentHit();
            Debug.Log("[Student] Got hit by vehicle!");

            // Dính theo xe
            AttachToVehicle(col.transform);
        }
    }

    void AttachToVehicle(Transform vehicle)
    {
        // Tắt physics của học sinh
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Gắn học sinh làm con của xe
        transform.SetParent(vehicle);

        // Giữ vị trí tương đối (dính trên đầu xe)
        // Có thể điều chỉnh offset nếu cần
        Vector3 localPos = transform.localPosition;
        localPos.z = 0;
        transform.localPosition = localPos;
    }
}
