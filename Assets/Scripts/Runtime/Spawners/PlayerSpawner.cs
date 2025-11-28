using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public PlayerController playerPrefab;

    void Start()
    {
        if (Object.FindFirstObjectByType<PlayerController>() != null) return;
        var player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        player.name = "Player";
    }
}
