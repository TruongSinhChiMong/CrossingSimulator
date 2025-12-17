using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

namespace Editor
{
    public class SetupPlayerInput : EditorWindow
    {
        [MenuItem("Tools/Setup Player Input System")]
        public static void SetupInput()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            
            if (player == null)
            {
                Debug.LogError("❌ No Player found in scene! Use 'Tools > Setup Player in Scene' first.");
                return;
            }
            
            GameObject playerObj = player.gameObject;
            
            // Check if PlayerInput already exists
            PlayerInput playerInput = playerObj.GetComponent<PlayerInput>();
            
            if (playerInput == null)
            {
                // Add PlayerInput component
                playerInput = playerObj.AddComponent<PlayerInput>();
                Debug.Log("✅ Added PlayerInput component");
            }
            else
            {
                Debug.Log("PlayerInput component already exists");
            }
            
            // Load Input Actions asset
            var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
            
            if (inputActions == null)
            {
                Debug.LogError("❌ Could not find InputSystem_Actions.inputactions!");
                return;
            }
            
            // Configure PlayerInput
            playerInput.actions = inputActions;
            playerInput.defaultActionMap = "Player";
            playerInput.notificationBehavior = PlayerNotifications.SendMessages;
            
            Debug.Log("✅ PlayerInput configured:");
            Debug.Log($"  - Actions: {inputActions.name}");
            Debug.Log($"  - Action Map: Player");
            Debug.Log($"  - Notification: SendMessages");
            
            // Mark dirty
            EditorUtility.SetDirty(playerObj);
            
            Debug.Log("\n🎮 Player Input setup complete!");
            Debug.Log("Now you can use WASD or Arrow Keys to move the player.");
            Debug.Log("Press Play to test!");
        }
        
        [MenuItem("Tools/Test Player Input")]
        public static void TestInput()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            
            if (player == null)
            {
                Debug.LogError("❌ No Player found in scene!");
                return;
            }
            
            PlayerInput playerInput = player.GetComponent<PlayerInput>();
            
            if (playerInput == null)
            {
                Debug.LogError("❌ Player doesn't have PlayerInput component!");
                Debug.Log("Run 'Tools > Setup Player Input System' first.");
                return;
            }
            
            Debug.Log("=== PLAYER INPUT TEST ===");
            Debug.Log($"✅ PlayerInput found");
            Debug.Log($"  - Actions: {(playerInput.actions != null ? playerInput.actions.name : "NULL")}");
            Debug.Log($"  - Current Action Map: {playerInput.currentActionMap?.name ?? "NULL"}");
            Debug.Log($"  - Notification Behavior: {playerInput.notificationBehavior}");
            Debug.Log($"  - Enabled: {playerInput.enabled}");
            
            if (playerInput.actions != null)
            {
                var moveAction = playerInput.actions.FindAction("Move");
                if (moveAction != null)
                {
                    Debug.Log($"✅ Move action found");
                    Debug.Log($"  - Enabled: {moveAction.enabled}");
                    Debug.Log($"  - Bindings: {moveAction.bindings.Count}");
                }
                else
                {
                    Debug.LogError("❌ Move action not found!");
                }
            }
            
            Debug.Log("\n💡 If everything looks good, press Play and try WASD or Arrow Keys!");
        }
    }
}
