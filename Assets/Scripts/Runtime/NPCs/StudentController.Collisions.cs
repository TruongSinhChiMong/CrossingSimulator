using UnityEngine;

public partial class StudentController : MonoBehaviour
{
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
    }
}
