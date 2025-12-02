using UnityEngine;
using UnityEditor;

namespace Editor
{
    public class DebugPlayerSetup : EditorWindow
    {
        [MenuItem("Tools/Debug Player Setup")]
        public static void DebugPlayer()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            
            if (player == null)
            {
                Debug.LogError("❌ No Player found in scene!");
                return;
            }
            
            GameObject playerObj = player.gameObject;
            Debug.Log($"=== PLAYER DEBUG INFO ===");
            Debug.Log($"Name: {playerObj.name}");
            Debug.Log($"Position: {playerObj.transform.position}");
            Debug.Log($"Active: {playerObj.activeSelf}");
            Debug.Log($"Layer: {LayerMask.LayerToName(playerObj.layer)}");
            
            // Check SpriteRenderer
            SpriteRenderer sr = playerObj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Debug.Log($"✅ SpriteRenderer found");
                Debug.Log($"  - Sprite: {(sr.sprite != null ? sr.sprite.name : "NULL")}");
                Debug.Log($"  - Color: {sr.color}");
                Debug.Log($"  - Sorting Layer: {sr.sortingLayerName}");
                Debug.Log($"  - Order in Layer: {sr.sortingOrder}");
                Debug.Log($"  - Enabled: {sr.enabled}");
            }
            else
            {
                Debug.LogWarning("⚠️ No SpriteRenderer found!");
            }
            
            // Check Animator
            Animator anim = playerObj.GetComponent<Animator>();
            if (anim != null)
            {
                Debug.Log($"✅ Animator found");
                Debug.Log($"  - Controller: {(anim.runtimeAnimatorController != null ? anim.runtimeAnimatorController.name : "NULL")}");
                Debug.Log($"  - Enabled: {anim.enabled}");
            }
            else
            {
                Debug.LogWarning("⚠️ No Animator found!");
            }
            
            // Check Rigidbody2D
            Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Debug.Log($"✅ Rigidbody2D found");
                Debug.Log($"  - Gravity Scale: {rb.gravityScale}");
            }
            else
            {
                Debug.LogWarning("⚠️ No Rigidbody2D found!");
            }
            
            // Check PlayerController
            Debug.Log($"✅ PlayerController found");
            Debug.Log($"  - Move Speed: {player.moveSpeed}");
            
            // Select player
            Selection.activeGameObject = playerObj;
            SceneView.lastActiveSceneView.FrameSelected();
            
            Debug.Log($"\n💡 Player selected and camera focused on it!");
        }
        
        [MenuItem("Tools/Fix Player Visibility")]
        public static void FixPlayerVisibility()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            
            if (player == null)
            {
                Debug.LogError("❌ No Player found in scene!");
                return;
            }
            
            GameObject playerObj = player.gameObject;
            
            // Ensure active
            playerObj.SetActive(true);
            
            // Fix SpriteRenderer
            SpriteRenderer sr = playerObj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.enabled = true;
                sr.color = Color.white;
                
                // If no sprite, try to load one
                if (sr.sprite == null)
                {
                    Sprite idleSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Characters/Player/Idle/stand_pose1.png");
                    if (idleSprite != null)
                    {
                        sr.sprite = idleSprite;
                        Debug.Log("✅ Assigned idle sprite to Player");
                    }
                }
            }
            else
            {
                // Add SpriteRenderer if missing
                sr = playerObj.AddComponent<SpriteRenderer>();
                Sprite idleSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/Characters/Player/Idle/stand_pose1.png");
                if (idleSprite != null)
                {
                    sr.sprite = idleSprite;
                }
                Debug.Log("✅ Added SpriteRenderer to Player");
            }
            
            // Set sorting
            sr.sortingLayerName = "Default";
            sr.sortingOrder = 10;
            
            // Mark dirty
            EditorUtility.SetDirty(playerObj);
            
            // Focus camera on player
            Selection.activeGameObject = playerObj;
            SceneView.lastActiveSceneView.FrameSelected();
            
            Debug.Log("✅ Player visibility fixed! Check Scene view now.");
        }
    }
}
