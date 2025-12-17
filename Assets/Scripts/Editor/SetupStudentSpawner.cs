using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Editor tool để tự động thêm StudentSpawner vào scene hiện tại.
/// Menu: Tools > Setup > Add Student Spawner
/// </summary>
public static class SetupStudentSpawner
{
    private const string StudentTypeAPath = "Assets/Prefabs/NPCs/Student/Student_TypeA.prefab";
    private const string StudentTypeBPath = "Assets/Prefabs/NPCs/Student/Student_TypeB.prefab";

    [MenuItem("Tools/Setup/Add Student Spawner")]
    public static void AddStudentSpawner()
    {
        // 1) Tìm hoặc tạo parent "Spawners"
        var spawnersGO = GameObject.Find("Spawners");
        if (spawnersGO == null)
        {
            spawnersGO = new GameObject("Spawners");
            Undo.RegisterCreatedObjectUndo(spawnersGO, "Create Spawners");
        }

        // 2) Kiểm tra đã có StudentSpawner chưa
        var existingSpawner = Object.FindObjectOfType<StudentSpawner>();
        if (existingSpawner != null)
        {
            Debug.LogWarning("[SetupStudentSpawner] StudentSpawner đã tồn tại trong scene!");
            Selection.activeGameObject = existingSpawner.gameObject;
            return;
        }

        // 3) Tạo StudentSpawner GameObject
        var spawnerGO = new GameObject("StudentSpawner");
        spawnerGO.transform.SetParent(spawnersGO.transform, false);
        Undo.RegisterCreatedObjectUndo(spawnerGO, "Create StudentSpawner");

        var spawner = spawnerGO.AddComponent<StudentSpawner>();

        // 4) Load prefabs
        var prefabA = AssetDatabase.LoadAssetAtPath<StudentController>(StudentTypeAPath);
        var prefabB = AssetDatabase.LoadAssetAtPath<StudentController>(StudentTypeBPath);

        if (prefabA == null)
            Debug.LogWarning($"[SetupStudentSpawner] Không tìm thấy prefab: {StudentTypeAPath}");
        if (prefabB == null)
            Debug.LogWarning($"[SetupStudentSpawner] Không tìm thấy prefab: {StudentTypeBPath}");

        // 5) Assign prefabs qua SerializedObject
        var so = new SerializedObject(spawner);
        so.FindProperty("prefabTypeA").objectReferenceValue = prefabA;
        so.FindProperty("prefabTypeB").objectReferenceValue = prefabB;
        so.FindProperty("totalStudents").intValue = 6;
        so.FindProperty("spawnPosition").vector2Value = new Vector2(5f, 0f);
        so.FindProperty("leftGateX").floatValue = -6f;
        so.FindProperty("minSpawnDelay").floatValue = 0.5f;
        so.FindProperty("maxSpawnDelay").floatValue = 1.5f;
        so.FindProperty("queueSpacing").floatValue = 0.6f;
        so.ApplyModifiedProperties();

        // 6) Tạo WaitPointRight (điểm chờ bên phải)
        var waitPointGO = new GameObject("WaitPointRight");
        waitPointGO.transform.SetParent(spawnerGO.transform, false);
        waitPointGO.transform.localPosition = new Vector3(3.5f, 0f, 0f);
        Undo.RegisterCreatedObjectUndo(waitPointGO, "Create WaitPointRight");

        // Assign waitPointRight
        so = new SerializedObject(spawner);
        so.FindProperty("waitPointRight").objectReferenceValue = waitPointGO.transform;
        so.ApplyModifiedProperties();

        // 7) Tạo RuntimeParent để chứa students spawn ra
        var runtimeParentGO = new GameObject("Students_Runtime");
        runtimeParentGO.transform.SetParent(spawnerGO.transform, false);
        Undo.RegisterCreatedObjectUndo(runtimeParentGO, "Create Students_Runtime");

        so = new SerializedObject(spawner);
        so.FindProperty("runtimeParent").objectReferenceValue = runtimeParentGO.transform;
        so.ApplyModifiedProperties();

        // 8) Đánh dấu scene dirty
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        // 9) Select object mới tạo
        Selection.activeGameObject = spawnerGO;

        Debug.Log("[SetupStudentSpawner] Đã thêm StudentSpawner vào scene! Nhớ điều chỉnh vị trí WaitPointRight và spawnPosition cho phù hợp với map.");
    }
}
