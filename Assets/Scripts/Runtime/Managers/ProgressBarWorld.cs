using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Thanh tiến độ dùng SpriteRenderer trong world-space.
/// Chỉ việc đổi sprite theo tỉ lệ hoàn thành.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class ProgressBarWorld : MonoBehaviour
{
    [Header("Sprite Renderer")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Các sprite bậc thang (0/6, 1/6, ..., 6/6)")]
    [Tooltip("Sắp xếp từ ít nhất -> nhiều nhất. Ví dụ: [bar0per6, bar1per6, ..., bar6per6]")]
    [SerializeField] private List<Sprite> stepSprites = new List<Sprite>();

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Cập nhật hình theo số học sinh đã qua / tổng số.
    /// </summary>
    public void SetProgress(int completed, int total)
    {
        if (spriteRenderer == null || stepSprites == null || stepSprites.Count == 0)
            return;

        if (total <= 0)
        {
            spriteRenderer.sprite = stepSprites[0];
            return;
        }

        float t = Mathf.Clamp01((float)completed / total);
        int index = Mathf.Clamp(
            Mathf.RoundToInt(t * (stepSprites.Count - 1)),
            0,
            stepSprites.Count - 1);

        spriteRenderer.sprite = stepSprites[index];
    }
}
