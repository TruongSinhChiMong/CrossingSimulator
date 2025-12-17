using UnityEngine;

/// <summary>
/// Vùng crossing nằm giữa SpawnZone và SafeZone.
/// Student chỉ bị dừng khi ấn X nếu đang trong vùng này.
/// Setup: Tạo GameObject với BoxCollider2D (IsTrigger = true), gán tag "CrossingZone".
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class CrossingZone : MonoBehaviour
{
    private void Reset()
    {
        // Tự động setup collider khi add component
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Gán tag nếu chưa có
        if (string.IsNullOrEmpty(gameObject.tag) || gameObject.tag == "Untagged")
        {
            gameObject.tag = "CrossingZone";
        }
    }

    private void OnDrawGizmos()
    {
        // Vẽ vùng crossing trong Scene view để dễ nhìn
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // màu cam trong suốt
            Vector3 center = transform.position + (Vector3)col.offset;
            Vector3 size = col.size;
            Gizmos.DrawCube(center, size);

            Gizmos.color = new Color(1f, 0.5f, 0f, 1f); // viền cam
            Gizmos.DrawWireCube(center, size);
        }
    }
}
