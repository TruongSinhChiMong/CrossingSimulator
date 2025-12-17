using UnityEngine;
using UnityEditor;

namespace Editor
{
    public class SetupPlayerInScene : EditorWindow
    {
        [MenuItem("Tools/Setup Player in Scene")]
        public static void SetupPlayer()
        {
            // Kiểm tra xem đã có Player trong scene chưa
            PlayerController existingPlayer = FindObjectOfType<PlayerController>();
            if (existingPlayer != null)
            {
                Debug.LogWarning($"Player already exists in scene: {existingPlayer.gameObject.name}");
                Selection.activeGameObject = existingPlayer.gameObject;
                return;
            }
            
            // Load Player prefab
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Character/Player.prefab");
            
            if (playerPrefab == null)
            {
                Debug.LogError("Could not find Player prefab at Assets/Prefabs/Character/Player.prefab");
                return;
            }
            
            // Instantiate Player vào scene
            GameObject player = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
            
            if (player == null)
            {
                Debug.LogError("Failed to instantiate Player prefab!");
                return;
            }
            
            // Đặt tên và vị trí
            player.name = "Player";
            player.transform.position = new Vector3(0f, 2.5f, 0f); // Phía trên Student
            
            // Setup Sorting Layer nếu có Sprite Renderer
            SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingLayerName = "Default"; // Hoặc "Characters" nếu có
                sr.sortingOrder = 10;
            }
            
            // Đảm bảo có các components cần thiết
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }
            
            // Register Undo
            Undo.RegisterCreatedObjectUndo(player, "Setup Player");
            
            // Select Player để dễ chỉnh sửa
            Selection.activeGameObject = player;
            
            Debug.Log($"✅ Player created successfully at position {player.transform.position}");
            Debug.Log("You can now adjust the position in Inspector if needed.");
        }
        
        [MenuItem("Tools/Find Player in Scene")]
        public static void FindPlayer()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            
            if (player != null)
            {
                Selection.activeGameObject = player.gameObject;
                Debug.Log($"Found Player: {player.gameObject.name} at position {player.transform.position}");
            }
            else
            {
                Debug.LogWarning("No Player found in scene. Use 'Tools > Setup Player in Scene' to create one.");
            }
        }
    }
}
