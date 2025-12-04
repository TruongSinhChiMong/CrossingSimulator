using UnityEngine;
using System.Collections;

public class VehicleSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] vehiclePrefabs; // Các loại xe khác nhau

    [Header("Spawn Settings")]
    public Transform[] spawnPoints; // Các điểm spawn (mỗi làn đường 1 điểm)
    public float minSpawnDelay = 2f;
    public float maxSpawnDelay = 5f;

    [Header("Vehicle Settings")]
    public float minSpeed = 3f;
    public float maxSpeed = 7f;

    [Header("Game Control")]
    public bool isSpawning = true;

    void Start()
    {
        if (vehiclePrefabs == null || vehiclePrefabs.Length == 0)
        {
            Debug.LogError("[VehicleSpawner] Chưa gán Vehicle Prefabs!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[VehicleSpawner] Chưa gán Spawn Points!");
            return;
        }

        // Bắt đầu spawn cho mỗi làn đường
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            StartCoroutine(SpawnRoutine(i));
        }
    }

    IEnumerator SpawnRoutine(int laneIndex)
    {
        while (isSpawning)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            if (!isSpawning) break;

            SpawnVehicle(laneIndex);
        }
    }

    void SpawnVehicle(int laneIndex)
    {
        if (laneIndex >= spawnPoints.Length) return;

        Transform spawnPoint = spawnPoints[laneIndex];
        GameObject prefab = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Length)];

        GameObject vehicle = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        // Cấu hình tốc độ ngẫu nhiên
        VehicleController vc = vehicle.GetComponent<VehicleController>();
        if (vc != null)
        {
            vc.moveSpeed = Random.Range(minSpeed, maxSpeed);
            // Hướng đi dựa vào vị trí spawn (trên màn hình = đi xuống, dưới màn hình = đi lên)
            vc.moveDown = spawnPoint.position.y > 0;
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    public void StartSpawning()
    {
        isSpawning = true;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            StartCoroutine(SpawnRoutine(i));
        }
    }
}
