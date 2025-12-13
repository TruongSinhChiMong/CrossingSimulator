using TMPro;
using UnityEngine;

public class SignCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;

    /// <summary>
    /// Hàm chung để set bất kỳ con số nào lên biển báo.
    /// </summary>
    public void SetNumber(int value)
    {
        if (counterText != null)
            counterText.text = value.ToString();
    }

    /// <summary>
    /// Tên cũ, giữ để tương thích với StudentSpawner.
    /// </summary>
    public void SetRemaining(int remaining)
    {
        SetNumber(remaining);
    }
}
