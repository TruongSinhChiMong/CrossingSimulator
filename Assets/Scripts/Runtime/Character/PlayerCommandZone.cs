using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Zone xung quanh Player. Chỉ những Student trong zone này mới nghe lệnh Stop/Cross.
/// Attach vào Player hoặc child object có CircleCollider2D (trigger).
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class PlayerCommandZone : MonoBehaviour
{
    public static PlayerCommandZone Instance { get; private set; }

    [Header("Zone Settings")]
    [Tooltip("Bán kính vùng ảnh hưởng của lệnh")]
    [SerializeField] private float zoneRadius = 3f;

    [Header("Tags")]
    [SerializeField] private string studentTag = "Student";

    private CircleCollider2D zoneCollider;
    private readonly HashSet<StudentController> studentsInZone = new HashSet<StudentController>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        zoneCollider = GetComponent<CircleCollider2D>();
        zoneCollider.isTrigger = true;
        zoneCollider.radius = zoneRadius;
    }

    private void OnValidate()
    {
        // Cập nhật radius khi thay đổi trong Inspector
        if (zoneCollider == null)
            zoneCollider = GetComponent<CircleCollider2D>();
        
        if (zoneCollider != null)
            zoneCollider.radius = zoneRadius;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!string.IsNullOrEmpty(studentTag) && other.CompareTag(studentTag))
        {
            var student = other.GetComponent<StudentController>();
            if (student != null)
            {
                studentsInZone.Add(student);
                Debug.Log($"[PlayerCommandZone] Student entered zone: {student.name}");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!string.IsNullOrEmpty(studentTag) && other.CompareTag(studentTag))
        {
            var student = other.GetComponent<StudentController>();
            if (student != null)
            {
                studentsInZone.Remove(student);
                Debug.Log($"[PlayerCommandZone] Student left zone: {student.name}");
            }
        }
    }

    /// <summary>
    /// Kiểm tra student có đang trong zone không.
    /// </summary>
    public bool IsStudentInZone(StudentController student)
    {
        return student != null && studentsInZone.Contains(student);
    }

    /// <summary>
    /// Lấy danh sách tất cả student đang trong zone.
    /// </summary>
    public IReadOnlyCollection<StudentController> GetStudentsInZone()
    {
        // Cleanup null references (student bị destroy)
        studentsInZone.RemoveWhere(s => s == null);
        return studentsInZone;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    // Vẽ zone trong Scene view để dễ debug
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, zoneRadius);
    }
}
