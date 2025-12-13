using System.Collections;
using UnityEngine;

/// <summary>
/// Cấu hình cho 1 lane xe.
/// Mỗi lane có:
/// - spawnPoint: vị trí sinh xe
/// - moveDown  : hướng đi (true = từ trên xuống)
/// - min/maxSpawnDelay: thời gian chờ giữa 2 xe
/// - min/maxSpeed     : khoảng tốc độ của xe
/// - useSwerve        : có lạng lách hay không
/// </summary>
[System.Serializable]
public class VehicleLane
{
    [Header("Spawn point")]
    public Transform spawnPoint;

    [Header("Direction")]
    public bool moveDown = true;

    [Header("Spawn Interval (seconds)")]
    public float minSpawnDelay = 2f;
    public float maxSpawnDelay = 5f;

    [Header("Speed")]
    public float minSpeed = 1f;
    public float maxSpeed = 3f;

    [Header("ZigZag / Lạng lách (optional)")]
    public bool useSwerve = false;
    public float swerveAmplitude = 0.3f;
    public float swerveFrequency = 1.5f;
}

/// <summary>
/// Sinh xe trên nhiều lane.
/// </summary>
public class VehicleSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] vehiclePrefabs;

    [Header("Lanes")]
    public VehicleLane[] lanes;

    [Header("Optional Limit")]
    [Tooltip("Nếu bật, giới hạn tổng số xe tồn tại cùng lúc.")]
    public bool limitTotalVehicles = false;

    public int maxVehicles = 30;
    private int currentVehicles = 0;

    [Header("Optional Parent")]
    [Tooltip("Nếu gán, tất cả xe spawn ra sẽ được làm con của Transform này (ví dụ: Runtime).")]
    public Transform runtimeParent;

    private void Start()
    {
        // mỗi lane có 1 coroutine spawn riêng
        foreach (var lane in lanes)
        {
            if (lane != null && lane.spawnPoint != null)
            {
                StartCoroutine(SpawnLaneRoutine(lane));
            }
        }
    }

    private IEnumerator SpawnLaneRoutine(VehicleLane lane)
    {
        while (true)
        {
            float delay = Random.Range(lane.minSpawnDelay, lane.maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            if (limitTotalVehicles && currentVehicles >= maxVehicles)
                continue;

            SpawnVehicleOnLane(lane);
        }
    }

    private void SpawnVehicleOnLane(VehicleLane lane)
    {
        if (vehiclePrefabs == null || vehiclePrefabs.Length == 0) return;

        // chọn prefab ngẫu nhiên (sau này bạn có thể thêm nhiều loại xe)
        var prefab = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Length)];
        var car = Instantiate(prefab, lane.spawnPoint.position, Quaternion.identity);

        if (runtimeParent != null)
            car.transform.SetParent(runtimeParent, true);

        currentVehicles++;

        // gán cấu hình lane cho VehicleController
        var vc = car.GetComponent<VehicleController>();
        if (vc != null)
        {
            vc.moveDown = lane.moveDown;
            vc.moveSpeed = Random.Range(lane.minSpeed, lane.maxSpeed);

            vc.enableSwerve = lane.useSwerve;
            vc.swerveAmplitude = lane.swerveAmplitude;
            vc.swerveFrequency = lane.swerveFrequency;
            vc.swervePhase = Random.Range(0f, Mathf.PI * 2f);
        }

        // gắn notifier để khi xe bị Destroy thì trừ biến đếm
        var notifier = car.AddComponent<VehicleDestroyNotifier>();
        notifier.spawner = this;
    }

    /// <summary>
    /// Được gọi bởi VehicleDestroyNotifier.OnDestroy().
    /// </summary>
    public void OnVehicleDestroyed()
    {
        currentVehicles = Mathf.Max(0, currentVehicles - 1);
    }
}
