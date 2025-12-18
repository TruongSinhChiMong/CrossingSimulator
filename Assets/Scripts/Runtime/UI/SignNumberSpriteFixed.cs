using UnityEngine;

public class SignNumberSpriteFixed : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Mảng sprite theo thứ tự: 0 -> max (ví dụ 0..32)
    // index 0 = "00" hoặc "0", index 1 = "01", ...
    [SerializeField] private Sprite[] numberSprites;

    // Cache giá trị hiện tại để tránh update không cần thiết
    private int currentValue = -1;

    private void Reset()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Đổi số hiển thị trên bảng theo giá trị value.
    /// </summary>
    public void SetNumber(int value)
    {
        if (spriteRenderer == null || numberSprites == null || numberSprites.Length == 0)
        {
            Debug.LogWarning($"[SignNumberSpriteFixed] Cannot set number: spriteRenderer={spriteRenderer != null}, sprites={numberSprites?.Length ?? 0}");
            return;
        }

        int originalValue = value;
        value = Mathf.Clamp(value, 0, numberSprites.Length - 1);
        
        if (originalValue != value)
        {
            Debug.LogWarning($"[SignNumberSpriteFixed] Value {originalValue} clamped to {value} (sprites count: {numberSprites.Length})");
        }

        // Tránh update nếu giá trị không đổi
        if (value == currentValue)
            return;

        currentValue = value;

        Sprite sprite = numberSprites[value];
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
            Debug.Log($"[SignNumberSpriteFixed] {gameObject.name} set to {value}");
        }
    }

    /// <summary>
    /// Lấy giá trị hiện tại đang hiển thị.
    /// </summary>
    public int GetCurrentValue()
    {
        return currentValue;
    }
}
