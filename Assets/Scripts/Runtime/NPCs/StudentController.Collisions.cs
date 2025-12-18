using UnityEngine;

public partial class StudentController : MonoBehaviour
{
    [Header("Zone Tags")]
    [SerializeField] private string crossingZoneTag = "CrossingZone";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || hasReportedResult)
            return;

        // Bị xe tông
        if (!string.IsNullOrEmpty(vehicleTag) && other.CompareTag(vehicleTag))
        {
            HandleHitByVehicle(other);
            return;
        }

        // Đi vào vùng an toàn bên trái
        if (!string.IsNullOrEmpty(safeZoneTag) && other.CompareTag(safeZoneTag))
        {
            HandleReachedSafeZone(other);
            return;
        }

        // Đi vào CrossingZone (vùng giữa spawn và safe)
        if (!string.IsNullOrEmpty(crossingZoneTag) && other.CompareTag(crossingZoneTag))
        {
            // Chỉ báo spawner nếu chưa vào crossing zone trước đó
            if (!isInCrossingZone && spawner != null)
            {
                spawner.NotifyStudentEnteredCrossing(this);
            }
            isInCrossingZone = true;
            return;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isDead || hasReportedResult)
            return;

        // Rời khỏi CrossingZone
        if (!string.IsNullOrEmpty(crossingZoneTag) && other.CompareTag(crossingZoneTag))
        {
            isInCrossingZone = false;
            
            // Nếu student rời CrossingZone về phía bên trái (đã qua đường thành công)
            // và chưa báo kết quả, thì coi như đã safe
            if (isCrossing && transform.position.x < other.bounds.center.x)
            {
                Debug.Log($"[StudentController] {gameObject.name} exited CrossingZone to the left - marking as safe");
                HandleReachedSafeZone(null);
            }
            return;
        }
    }
}
