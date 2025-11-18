using UnityEngine;

/// <summary>
/// Sinh học sinh ở mép phải, A/B xen kẽ, có giãn cách.
/// Gắn script này vào GameObject "Spawners/StudentSpawner".
/// </summary>
public class StudentSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefabTypeA;
    [SerializeField] private GameObject prefabTypeB;

    [SerializeField] private int count = 6;
    [SerializeField] private float startX = 7.5f;   // mép phải (tuỳ map)
    [SerializeField] private float startY = -1.6f;  // cao độ lề
    [SerializeField] private float spacing = 1.0f;  // giãn cách
    [SerializeField] private bool startsWaiting = true;

    [SerializeField] private Transform player; // nếu StudentController dùng khoảng cách

    void Start()
    {
        float x = startX;
        for (int i = 0; i < count; i++)
        {
            var prefab = (i % 2 == 0) ? prefabTypeA : prefabTypeB;
            var go = Instantiate(prefab, new Vector3(x, startY, 0f), Quaternion.identity, transform);

            // truyền cấu hình ban đầu (không bắt buộc, chỉ để đồng bộ nhanh)
            var sc = go.GetComponent<StudentController>();
            if (sc != null)
            {
                // set private field startsWaiting qua reflection (đỡ phải public)
                var fld = typeof(StudentController).GetField("startsWaiting", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (fld != null) fld.SetValue(sc, startsWaiting);

                var pfld = typeof(StudentController).GetField("player", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (pfld != null && player) pfld.SetValue(sc, player);
            }

            x += spacing;
        }
    }
}
