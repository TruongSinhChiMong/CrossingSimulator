using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor tool để fix Student prefabs - đảm bảo có StudentController component
/// Menu: Tools > Fix > Student Prefabs
/// </summary>
public static class FixStudentPrefabs
{
    [MenuItem("Tools/Fix/Student Prefabs")]
    public static void Fix()
    {
        string[] prefabPaths = new string[]
        {
            "Assets/Prefabs/NPCs/Student/Student_TypeA.prefab",
            "Assets/Prefabs/NPCs/Student/Student_TypeB.prefab"
        };

        foreach (var path in prefabPaths)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogWarning($"[FixStudentPrefabs] Prefab not found: {path}");
                continue;
            }

            var controller = prefab.GetComponent<StudentController>();
            if (controller != null)
            {
                Debug.Log($"[FixStudentPrefabs] {prefab.name} already has StudentController ✓");
                continue;
            }

            // Mở prefab để edit
            var prefabRoot = PrefabUtility.LoadPrefabContents(path);
            prefabRoot.AddComponent<StudentController>();
            PrefabUtility.SaveAsPrefabAsset(prefabRoot, path);
            PrefabUtility.UnloadPrefabContents(prefabRoot);

            Debug.Log($"[FixStudentPrefabs] Added StudentController to {prefab.name} ✓");
        }

        AssetDatabase.Refresh();
        Debug.Log("[FixStudentPrefabs] Done!");
    }

    [MenuItem("Tools/Fix/Check Student Prefabs")]
    public static void Check()
    {
        string[] prefabPaths = new string[]
        {
            "Assets/Prefabs/NPCs/Student/Student_TypeA.prefab",
            "Assets/Prefabs/NPCs/Student/Student_TypeB.prefab"
        };

        foreach (var path in prefabPaths)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogError($"[CheckStudentPrefabs] Prefab NOT FOUND: {path}");
                continue;
            }

            var controller = prefab.GetComponent<StudentController>();
            if (controller != null)
            {
                Debug.Log($"[CheckStudentPrefabs] {prefab.name} has StudentController ✓");
            }
            else
            {
                Debug.LogError($"[CheckStudentPrefabs] {prefab.name} MISSING StudentController!");
            }
        }
    }
}
