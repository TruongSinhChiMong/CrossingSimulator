using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Zone xung quanh Player. Chỉ những Student trong zone này mới nghe lệnh Stop/Cross.
/// Attach vào Player hoặc child object có BoxCollider2D (trigger).
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerCommandZone : MonoBehaviour
{
    public static PlayerCommandZone Instance { get; private set; }

    [Header("Zone Settings")]
    [Tooltip("Kích thước vùng ảnh hưởng của lệnh (width x height)")]
    [SerializeField] private Vector2 zoneSize = new Vector2(6f, 6f);

    [Header("Tags")]
    [SerializeField] private string studentTag = "Student";

    private BoxCollider2D zoneCollider;
    private readonly HashSet<StudentController> studentsInZone = new HashSet<StudentController>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        zoneCollider = GetComponent<BoxCollider2D>();
        zoneCollider.isTrigger = true;
        zoneCollider.size = zoneSize;
    }

    private void OnValidate()
    {
        // Cập nhật size khi thay đổi trong Inspector
        var collider = GetComponent<BoxCollider2D>();
        if (collider != null)
            collider.size = zoneSize;
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
        Gizmos.DrawWireCube(transform.position, zoneSize);
    }
}
