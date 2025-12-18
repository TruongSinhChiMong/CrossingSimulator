using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Editor tool để thêm PlayerCommandZone vào Player trong scene.
/// Menu: Tools > Setup > Add Player Command Zone
/// </summary>
public static class SetupPlayerCommandZone
{
    [MenuItem("Tools/Setup/Add Player Command Zone")]
    public static void AddPlayerCommandZone()
    {
        // 1) Tìm Player trong scene - thử nhiều cách
        GameObject playerGO = FindPlayerGameObject();
        
        if (playerGO == null)
        {
            Debug.LogError("[SetupPlayerCommandZone] Không tìm thấy Player trong scene!");
            Debug.LogError("[SetupPlayerCommandZone] Hãy chạy 'Tools > Setup Player in Scene' trước, hoặc chọn Player object và chạy lại.");
            return;
        }

        // 2) Kiểm tra đã có PlayerCommandZone chưa
        var existingZone = playerGO.GetComponentInChildren<PlayerCommandZone>();
        if (existingZone != null)
        {
            Debug.LogWarning("[SetupPlayerCommandZone] PlayerCommandZone đã tồn tại!");
            Selection.activeGameObject = existingZone.gameObject;
            return;
        }

        // 3) Tạo child object cho zone
        var zoneGO = new GameObject("CommandZone");
        zoneGO.transform.SetParent(playerGO.transform, false);
        zoneGO.transform.localPosition = Vector3.zero;
        Undo.RegisterCreatedObjectUndo(zoneGO, "Create PlayerCommandZone");

        // 4) Thêm components
        var collider = zoneGO.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 3f;

        var zone = zoneGO.AddComponent<PlayerCommandZone>();

        // 5) Set layer (optional - để tránh va chạm vật lý không mong muốn)
        // zoneGO.layer = LayerMask.NameToLayer("Ignore Raycast");

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Selection.activeGameObject = zoneGO;

        Debug.Log("<color=green>[SetupPlayerCommandZone] Đã tạo PlayerCommandZone cho " + playerGO.name + "!</color>");
        Debug.Log("<color=yellow>[SetupPlayerCommandZone] Điều chỉnh Zone Radius trong Inspector nếu cần.</color>");
        Debug.Log("<color=yellow>[SetupPlayerCommandZone] Nhớ đảm bảo Student prefab có tag 'Student'!</color>");
    }

    /// <summary>
    /// Tìm Player GameObject trong scene bằng nhiều cách.
    /// </summary>
    private static GameObject FindPlayerGameObject()
    {
        // Cách 1: Tìm theo PlayerController component
        var playerController = Object.FindObjectOfType<PlayerController>();
        if (playerController != null)
            return playerController.gameObject;

        // Cách 2: Tìm theo tag "Player"
        var playerByTag = GameObject.FindGameObjectWithTag("Player");
        if (playerByTag != null)
            return playerByTag;

        // Cách 3: Tìm theo tên "Player"
        var playerByName = GameObject.Find("Player");
        if (playerByName != null)
            return playerByName;

        // Cách 4: Nếu user đang chọn một object, dùng object đó
        if (Selection.activeGameObject != null)
        {
            Debug.Log("[SetupPlayerCommandZone] Sử dụng object đang được chọn: " + Selection.activeGameObject.name);
            return Selection.activeGameObject;
        }

        return null;
    }
}
