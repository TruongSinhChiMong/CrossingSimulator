using UnityEngine;

public class SignNumberSpriteFixed : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Mảng sprite theo thứ tự: 0 -> max (ví dụ 0..32)
    // index 0 = "00" hoặc "0", index 1 = "01", ...
    [SerializeField] private Sprite[] numberSprites;

    private void Reset()
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
            return;

        value = Mathf.Clamp(value, 0, numberSprites.Length - 1);

        Sprite sprite = numberSprites[value];
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }
}
