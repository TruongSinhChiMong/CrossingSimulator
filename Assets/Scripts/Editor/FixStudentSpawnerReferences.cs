using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Editor tool để fix references cho StudentSpawner trong scene hiện tại.
/// Menu: Tools > Setup > Fix StudentSpawner References
/// </summary>
public static class FixStudentSpawnerReferences
{
    private const string StudentTypeAPath = "Assets/Prefabs/NPCs/Student/Student_TypeA.prefab";
    private const string StudentTypeBPath = "Assets/Prefabs/NPCs/Student/Student_TypeB.prefab";

    [MenuItem("Tools/Setup/Fix StudentSpawner References")]
    public static void FixReferences()
    {
        var spawner = Object.FindObjectOfType<StudentSpawner>();
        if (spawner == null)
        {
            Debug.LogError("[FixStudentSpawnerReferences] Không tìm thấy StudentSpawner trong scene!");
            return;
        }

        var so = new SerializedObject(spawner);
        bool changed = false;

        // 1) Fix prefabs
        var prefabA = AssetDatabase.LoadAssetAtPath<StudentController>(StudentTypeAPath);
        var prefabB = AssetDatabase.LoadAssetAtPath<StudentController>(StudentTypeBPath);

        if (prefabA != null && so.FindProperty("prefabTypeA").objectReferenceValue == null)
        {
            so.FindProperty("prefabTypeA").objectReferenceValue = prefabA;
            Debug.Log("[FixStudentSpawnerReferences] Assigned prefabTypeA");
            changed = true;
        }

        if (prefabB != null && so.FindProperty("prefabTypeB").objectReferenceValue == null)
        {
            so.FindProperty("prefabTypeB").objectReferenceValue = prefabB;
            Debug.Log("[FixStudentSpawnerReferences] Assigned prefabTypeB");
            changed = true;
        }

        // 2) Fix rightSign - tìm SignNumberSpriteFixed trong scene
        if (so.FindProperty("rightSign").objectReferenceValue == null)
        {
            var signs = Object.FindObjectsOfType<SignNumberSpriteFixed>();
            foreach (var sign in signs)
            {
                // Tìm sign bên phải (có thể dựa vào tên hoặc vị trí)
                if (sign.gameObject.name.ToLower().Contains("right"))
                {
                    so.FindProperty("rightSign").objectReferenceValue = sign;
                    Debug.Log($"[FixStudentSpawnerReferences] Assigned rightSign: {sign.gameObject.name}");
                    changed = true;
                    break;
                }
            }
        }

        // 3) Fix leftSign
        if (so.FindProperty("leftSign").objectReferenceValue == null)
        {
            var signs = Object.FindObjectsOfType<SignNumberSpriteFixed>();
            foreach (var sign in signs)
            {
                if (sign.gameObject.name.ToLower().Contains("left"))
                {
                    so.FindProperty("leftSign").objectReferenceValue = sign;
                    Debug.Log($"[FixStudentSpawnerReferences] Assigned leftSign: {sign.gameObject.name}");
                    changed = true;
                    break;
                }
            }
        }

        // 4) Fix progressBar
        if (so.FindProperty("progressBar").objectReferenceValue == null)
        {
            var progressBar = Object.FindObjectOfType<ProgressBarWorld>();
            if (progressBar != null)
            {
                so.FindProperty("progressBar").objectReferenceValue = progressBar;
                Debug.Log("[FixStudentSpawnerReferences] Assigned progressBar");
                changed = true;
            }
        }

        if (changed)
        {
            so.ApplyModifiedProperties();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("<color=green>[FixStudentSpawnerReferences] Đã fix references! Nhớ Save scene.</color>");
        }
        else
        {
            Debug.Log("[FixStudentSpawnerReferences] Không có gì cần fix hoặc không tìm thấy objects phù hợp.");
        }

        Selection.activeGameObject = spawner.gameObject;
    }
}
