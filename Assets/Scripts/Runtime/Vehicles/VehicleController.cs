using UnityEngine;

/// <summary>
/// Điều khiển chuyển động & hiệu ứng cho 1 chiếc xe.
/// Gắn script này lên prefab xe (Car_01).
/// </summary>
[DisallowMultipleComponent]
public class VehicleController : MonoBehaviour
{
    [Header("Movement (dọc)")]
    [Tooltip("Tốc độ di chuyển cơ bản của xe.")]
    public float moveSpeed = 5f;

    [Tooltip("True = xe đi từ trên xuống, False = từ dưới lên.")]
    public bool moveDown = true;

    [Header("Despawn")]
    [Tooltip("Khoảng cách di chuyển tối đa trước khi xe tự hủy.")]
    public float despawnDistance = 15f;

    [Header("Scaling (giả 3D)")]
    [Tooltip("Bật/tắt hiệu ứng xa nhỏ – gần to.")]
    public bool enableScaling = true;

    [Tooltip("Tỉ lệ scale của xe khi ở trên cao (xa người chơi).")]
    [Range(0.1f, 3f)] public float startScale = 0.66f;   // 2/3

    [Tooltip("Tỉ lệ scale của xe khi gần người chơi.")]
    [Range(0.1f, 3f)] public float endScale = 1.0f;      // 3/3

    [Tooltip("Giá trị Y ở mép trên đường (vùng spawn).")]
    public float topY = 1.9f;

    [Tooltip("Giá trị Y ở gần vạch băng qua đường.")]
    public float bottomY = -2.0f;

    [Header("ZigZag / Lạng lách (tùy chọn)")]
    [Tooltip("Nếu bật, xe sẽ lạng lách qua lại theo trục X.")]
    public bool enableSwerve = false;

    [Tooltip("Biên độ lạng lách (độ lệch ngang tối đa).")]
    public float swerveAmplitude = 0.3f;

    [Tooltip("Tần số lạng lách (tốc độ lắc qua lại).")]
    public float swerveFrequency = 1.5f;

    // phase được gán từ Spawner để mỗi xe lắc lệch pha một chút
    [HideInInspector] public float swervePhase = 0f;

    // --------- private state ----------
    private Vector3 baseScale;
    private Vector3 startPos;
    private float baseX;
    private float timeAlive;

    private void Start()
    {
        baseScale = transform.localScale;  // scale gốc (coi như 3/3)
        startPos = transform.position;
        baseX = transform.position.x;
        timeAlive = 0f;

        // nếu xe có Rigidbody2D thì cho nó kinematic để không bị gravity
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
            rb.gravityScale = 0f;
        }

        // lúc spawn xe nhỏ = startScale
        if (enableScaling)
        {
            transform.localScale = baseScale * startScale;
        }
    }

    private void Update()
    {
        // -------- DI CHUYỂN DỌC ----------
        float dirY = moveDown ? -1f : 1f;

        Vector3 pos = transform.position;
        pos.y += dirY * moveSpeed * Time.deltaTime;

        // -------- LẠNG LÁCH NGANG ----------
        if (enableSwerve && swerveAmplitude > 0f && swerveFrequency > 0f)
        {
            timeAlive += Time.deltaTime;
            float offsetX = Mathf.Sin(timeAlive * swerveFrequency + swervePhase) * swerveAmplitude;
            pos.x = baseX + offsetX;
        }

        transform.position = pos;

        // -------- SCALING THEO Y ----------
        if (enableScaling)
        {
            // t = 0 khi ở topY, t = 1 khi ở bottomY
            float t = Mathf.InverseLerp(topY, bottomY, pos.y);
            float s = Mathf.Lerp(startScale, endScale, t);
            transform.localScale = baseScale * s;
        }

        // -------- DESPAWN ----------
        if (Vector3.Distance(startPos, transform.position) >= despawnDistance)
        {
            Destroy(gameObject);
        }
    }
}
