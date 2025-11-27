using UnityEngine;
using System.Collections;

public class StudentSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject prefabTypeA;
    public GameObject prefabTypeB;

    [Header("Spawn Settings")]
    [Tooltip("Tổng số học sinh spawn trong màn")]
    public int count = 6;

    [Tooltip("Vị trí spawn (world). Chỉnh Y để dời học sinh lên/xuống.")]
    public Vector2 spawnPos = new Vector2(3f, -1.8f);

    [Tooltip("Thời gian trễ ngẫu nhiên giữa mỗi học sinh (giây)")]
    public float minDelay = 6f;
    public float maxDelay = 15f;

    [Header("UI (optional)")]
    public SignCounter rightSignCounter;   // biển phải
    public ProgressManager progress;       // thanh tiến độ

    [Header("Optional")]
    public PlayerController player;        // để Student biết Player (SetPlayer)

    private void Start()
    {
        if (!prefabTypeA || !prefabTypeB)
        {
            Debug.LogError("[StudentSpawner] Chưa gán Prefab Type A/B.");
            enabled = false;
            return;
        }

        if (rightSignCounter)
            rightSignCounter.SetTotal(count);

        if (progress)
            progress.totalStudents = count;

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < count; i++)
        {
            var prefab = (i % 2 == 0) ? prefabTypeA : prefabTypeB;
            var go = Instantiate(prefab, spawnPos, Quaternion.identity);

            var sc = go.GetComponent<StudentController>();
            if (sc != null && player != null)
            {
                sc.SetPlayer(player.transform);
            }

            if (rightSignCounter)
                rightSignCounter.SetRemaining(count - i - 1);

            float delay = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(delay);
        }
    }
}
