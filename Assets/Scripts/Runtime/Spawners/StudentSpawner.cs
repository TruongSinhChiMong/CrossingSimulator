using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawn học sinh, cho chúng xếp hàng ngang bên phải,
/// dồn hàng khi 1 học sinh rời hàng,
/// cập nhật biển số & ProgressBar.
/// </summary>
public class StudentSpawner : MonoBehaviour
{
    [Header("Student Prefabs")]
    [SerializeField] private StudentController prefabTypeA;
    [SerializeField] private StudentController prefabTypeB;

    [Header("Spawn Settings")]
    [SerializeField] private int totalStudents = 6;
    [SerializeField] private Vector2 spawnPosition = new Vector2(4.5f, -2f);
    [SerializeField] private float minSpawnDelay = 0.5f;
    [SerializeField] private float maxSpawnDelay = 1.5f;

    [Header("Cross Settings")]
    [Tooltip("Mốc X bên trái được coi là đã qua đường (backup nếu SafeZone không dùng).")]
    [SerializeField] private float leftGateX = -6.0f;

    [Tooltip("Điểm chờ bên phải, ngay sát vạch qua đường (học sinh đầu hàng).")]
    [SerializeField] private Transform waitPointRight;

    [Header("Queue Settings")]
    [Tooltip("Khoảng cách giữa 2 học sinh trong hàng ngang (trục X).")]
    [SerializeField] private float queueSpacing = 0.6f;

    [Header("Hierarchy")]
    [SerializeField] private Transform runtimeParent;

    [Header("UI / World UI")]
    [SerializeField] private SignCounter signCounter;        // biển bên phải: số còn lại
    [SerializeField] private ProgressBarWorld progressBar;   // thanh tiến độ sprite

    private int spawnedCount;
    private int succeededCount;
    private int deadCount;

    // danh sách học sinh còn đang xếp hàng chờ bên phải
    private readonly List<StudentController> waitingStudents = new List<StudentController>();

    private void Start()
    {
        spawnedCount = 0;
        succeededCount = 0;
        deadCount = 0;

        UpdateUI();
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (spawnedCount < totalStudents)
        {
            SpawnOneStudent();
            spawnedCount++;

            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
        }
    }

    private void SpawnOneStudent()
    {
        StudentController prefab = ChooseRandomPrefab();
        if (prefab == null)
        {
            Debug.LogWarning("[StudentSpawner] No student prefab assigned.");
            return;
        }

        Transform parent = runtimeParent != null ? runtimeParent : transform;
        Vector3 pos = spawnPosition; // trong tường phải

        StudentController stu = Instantiate(prefab, pos, Quaternion.identity, parent);

        // base A (slot đầu hàng)
        Vector2 baseWait = waitPointRight != null
            ? (Vector2)waitPointRight.position
            : spawnPosition;

        // index = vị trí trong hàng, 0 = đầu hàng
        int index = waitingStudents.Count;
        Vector2 waitPoint = baseWait + Vector2.right * queueSpacing * index;

        stu.SetupFromSpawner(leftGateX, waitPoint, this);

        waitingStudents.Add(stu);

        UpdateUI();
    }

    private StudentController ChooseRandomPrefab()
    {
        if (prefabTypeA != null && prefabTypeB != null)
        {
            return Random.value < 0.5f ? prefabTypeA : prefabTypeB;
        }

        if (prefabTypeA != null) return prefabTypeA;
        if (prefabTypeB != null) return prefabTypeB;
        return null;
    }

    /// <summary>
    /// Học sinh rời hàng (bắt đầu băng qua hoặc chết khi đang chờ).
    /// Dồn hàng: thằng sau tiến lên chỗ thằng trước.
    /// </summary>
    internal void OnStudentLeftQueue(StudentController student)
    {
        int index = waitingStudents.IndexOf(student);
        if (index < 0)
            return;

        waitingStudents.RemoveAt(index);

        Vector2 baseWait = waitPointRight != null
            ? (Vector2)waitPointRight.position
            : spawnPosition;

        // dồn lại slot 0,1,2,... từ trái sang phải
        for (int i = 0; i < waitingStudents.Count; i++)
        {
            Vector2 newWait = baseWait + Vector2.right * queueSpacing * i;
            waitingStudents[i].UpdateQueueWaitPoint(newWait);
        }
    }

    /// <summary>
    /// Kiểm tra 1 học sinh có đang đứng đầu hàng hay không.
    /// Dùng cho auto-yell.
    /// </summary>
    internal bool IsFrontStudent(StudentController student)
    {
        return waitingStudents.Count > 0 && waitingStudents[0] == student;
    }

    internal void NotifyStudentSucceeded(StudentController student)
    {
        succeededCount++;
        UpdateUI();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStudentSafe();
        }
    }

    internal void NotifyStudentDied(StudentController student)
    {
        deadCount++;
        UpdateUI();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStudentHit();
        }
    }

    private void UpdateUI()
    {
        int resolved = succeededCount + deadCount;
        int remaining = Mathf.Max(0, totalStudents - resolved);

        if (signCounter != null)
        {
            signCounter.SetRemaining(remaining);
        }

        if (progressBar != null)
        {
            // chỉ các em qua AN TOÀN mới làm tiến độ tăng
            progressBar.SetProgress(succeededCount, totalStudents);
        }
    }

    public int TotalStudents => totalStudents;
    public int SucceededStudents => succeededCount;
}
