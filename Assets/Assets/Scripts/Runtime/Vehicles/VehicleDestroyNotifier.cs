using UnityEngine;

/// <summary>
/// Gắn tự động lên mỗi xe khi spawn ra.
/// Khi gameObject bị Destroy, script này báo lại cho VehicleSpawner.
/// </summary>
public class VehicleDestroyNotifier : MonoBehaviour
{
    [HideInInspector] public VehicleSpawner spawner;

    private void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.OnVehicleDestroyed();
        }
    }
}
