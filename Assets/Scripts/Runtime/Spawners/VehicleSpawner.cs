using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cấu hình cho 1 lane xe.
/// </summary>
[System.Serializable]
public class VehicleLane
{
    [Header("Spawn point")]
    public Transform spawnPoint;

    [Header("Direction")]
    public bool moveDown = true;

    [Header("Speed")]
    public float minSpeed = 1f;
    public float maxSpeed = 3f;

    [Header("ZigZag / Lạng lách (optional)")]
    public bool useSwerve = false;
    public float swerveAmplitude = 0.3f;
    public float swerveFrequency = 1.5f;

    // Runtime: xe hiện tại đang chạy trên lane này
    [HideInInspector] public GameObject currentVehicle;
}

/// <summary>
/// Sinh xe trên nhiều lane với các quy tắc:
/// 1. Mỗi lane chỉ spawn xe mới khi xe trước đã ra khỏi camera
/// 2. Không spawn nhiều lane cùng lúc (tránh xe chạy song song)
/// </summary>
public class VehicleSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] vehiclePrefabs;

    [Header("Lanes")]
    public VehicleLane[] lanes;

    [Header("Spawn Timing")]
    [Tooltip("Thời gian chờ tối thiểu giữa 2 lần spawn (bất kỳ lane nào).")]
    [SerializeField] private float minGlobalSpawnDelay = 1.5f;
    
    [Tooltip("Thời gian chờ tối đa giữa 2 lần spawn.")]
    [SerializeField] private float maxGlobalSpawnDelay = 3f;

    [Header("Camera Check")]
    [Tooltip("Camera dùng để kiểm tra xe đã ra khỏi view chưa. Nếu null sẽ dùng Camera.main.")]
    [SerializeField] private Camera gameCamera;
    
    [Tooltip("Khoảng cách thêm ngoài camera bounds để coi như xe đã ra khỏi view.")]
    [SerializeField] private float cameraMargin = 1f;

    [Header("Optional Limit")]
    [Tooltip("Nếu bật, giới hạn tổng số xe tồn tại cùng lúc.")]
    public bool limitTotalVehicles = false;
    public int maxVehicles = 30;
    private int currentVehicles = 0;

    [Header("Optional Parent")]
    [Tooltip("Nếu gán, tất cả xe spawn ra sẽ được làm con của Transform này.")]
    public Transform runtimeParent;

    private float lastSpawnTime = -999f;
    private List<int> availableLaneIndices = new List<int>();

    private void Start()
    {
        if (gameCamera == null)
            gameCamera = Camera.main;

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        // Chờ 1 frame để camera setup xong
        yield return null;

        while (true)
        {
            // Chờ delay ngẫu nhiên
            float delay = Random.Range(minGlobalSpawnDelay, maxGlobalSpawnDelay);
            yield return new WaitForSeconds(delay);

            if (limitTotalVehicles && currentVehicles >= maxVehicles)
                continue;

            // Tìm các lane có thể spawn (xe trước đã ra khỏi camera hoặc chưa có xe)
            availableLaneIndices.Clear();
            for (int i = 0; i < lanes.Length; i++)
            {
                if (CanSpawnOnLane(lanes[i]))
                {
                    availableLaneIndices.Add(i);
                }
            }

            // Nếu có lane available, chọn ngẫu nhiên 1 lane để spawn
            if (availableLaneIndices.Count > 0)
            {
                int randomIndex = availableLaneIndices[Random.Range(0, availableLaneIndices.Count)];
                SpawnVehicleOnLane(lanes[randomIndex]);
            }
        }
    }

    /// <summary>
    /// Kiểm tra xem lane có thể spawn xe mới không.
    /// Điều kiện: không có xe hoặc xe hiện tại đã ra khỏi camera.
    /// </summary>
    private bool CanSpawnOnLane(VehicleLane lane)
    {
        if (lane == null || lane.spawnPoint == null)
            return false;

        // Chưa có xe trên lane -> OK
        if (lane.currentVehicle == null)
            return true;

        // Có xe nhưng đã bị destroy -> OK
        if (lane.currentVehicle == null)
            return true;

        // Kiểm tra xe đã ra khỏi camera chưa
        return IsOutOfCamera(lane.currentVehicle);
    }

    /// <summary>
    /// Kiểm tra object đã ra khỏi camera view chưa.
    /// </summary>
    private bool IsOutOfCamera(GameObject obj)
    {
        if (obj == null || gameCamera == null)
            return true;

        Vector3 viewportPos = gameCamera.WorldToViewportPoint(obj.transform.position);
        
        // Viewport: (0,0) = bottom-left, (1,1) = top-right
        // Thêm margin để chắc chắn xe đã ra hẳn
        float margin = cameraMargin / gameCamera.orthographicSize;
        
        bool outOfView = viewportPos.x < -margin || viewportPos.x > 1 + margin ||
                         viewportPos.y < -margin || viewportPos.y > 1 + margin;
        
        return outOfView;
    }

    private void SpawnVehicleOnLane(VehicleLane lane)
    {
        if (vehiclePrefabs == null || vehiclePrefabs.Length == 0) return;

        // Chọn prefab ngẫu nhiên
        var prefab = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Length)];
        var car = Instantiate(prefab, lane.spawnPoint.position, Quaternion.identity);

        if (runtimeParent != null)
            car.transform.SetParent(runtimeParent, true);

        currentVehicles++;

        // Lưu reference xe hiện tại của lane
        lane.currentVehicle = car;

        // Gán cấu hình lane cho VehicleController
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

        // Gắn notifier để khi xe bị Destroy thì trừ biến đếm
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
