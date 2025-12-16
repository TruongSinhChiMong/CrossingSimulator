using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Gọi khi action Cross (Z) được nhấn.
    /// </summary>
    private void OnCrossPerformed(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (crossRoutine != null) StopCoroutine(crossRoutine);
        crossRoutine = StartCoroutine(SetBoolForSeconds(AnimIsCrossing, crossDuration));

        // Gửi lệnh cho học sinh
        OrdersManager.EmitCross();
        Debug.Log("[Player] Z -> Cross order");
    }

    /// <summary>
    /// Gọi khi action Stop (X) được nhấn.
    /// </summary>
    private void OnStopPerformed(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        if (stopRoutine != null) StopCoroutine(stopRoutine);
        stopRoutine = StartCoroutine(SetBoolForSeconds(AnimIsStopping, stopDuration));

        // Dừng gấp
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        // Gửi lệnh cho học sinh
        OrdersManager.EmitStop();
        Debug.Log("[Player] X -> Stop order");
    }
}
