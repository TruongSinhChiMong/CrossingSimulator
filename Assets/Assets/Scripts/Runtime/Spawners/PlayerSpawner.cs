using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Prefab Player sẽ spawn khi vào Map.")]
    public GameObject playerPrefab;

    [Header("Runtime Parent (optional)")]
    [Tooltip("Nếu gán, Player sẽ được đặt làm con của object này (ví dụ: Runtime).")]
    public Transform runtimeParent;

    GameObject spawnedPlayer;

    void Start()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("[PlayerSpawner] Chưa gán Player Prefab trên " + name);
            return;
        }

        Vector3 spawnPos = transform.position;
        Quaternion spawnRot = Quaternion.identity;

        spawnedPlayer = Instantiate(playerPrefab, spawnPos, spawnRot);

        if (runtimeParent != null)
            spawnedPlayer.transform.SetParent(runtimeParent);

        // đảm bảo tag và layer ổn
        spawnedPlayer.tag = "Player";
        spawnedPlayer.transform.localScale = Vector3.one;

        Debug.Log($"[PlayerSpawner] Spawned Player tại {spawnPos}", spawnedPlayer);
    }
}
