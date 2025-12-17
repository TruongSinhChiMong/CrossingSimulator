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
            return;
        }
    }
}
