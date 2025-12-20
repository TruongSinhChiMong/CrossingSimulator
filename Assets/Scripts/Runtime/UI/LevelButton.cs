using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Runtime.UI;

public class LevelButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private Image bgImage;        // Kéo object con BG (Image) vào đây
    [SerializeField] private Image numberImage;    // Kéo object con Number (Image) vào đây

    [Header("Background Sprites")]
    [SerializeField] private Sprite lockedBG;      // block_star
    [SerializeField] private Sprite bg0Star;       // (tuỳ chọn) nền mở khóa nhưng chưa có sao, có thể để null
    [SerializeField] private Sprite bg1Star;       // 1_star
    [SerializeField] private Sprite bg2Star;       // 2_star
    [SerializeField] private Sprite bg3Star;       // 3_star

    [Header("Number Sprites (index = levelNumber)")]
    [Tooltip("Gán sprite số theo index: [1]=01, [2]=02, ... [5]=05. [0]=00 (optional).")]
    [SerializeField] private Sprite[] numberSprites;

    private LevelData data;

    /// <summary>
    /// LevelSelectionManager sẽ gọi hàm này để cập nhật UI theo trạng thái mở khóa + số sao.
    /// </summary>
    public void Setup(LevelData levelData)
    {
        data = levelData;

        // --- Safety checks ---
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (bgImage == null || numberImage == null)
        {
            Debug.LogWarning($"[LevelButton] Missing BG/Number Image references on {name}.", this);
        }

        // --- Set number sprite ---
        if (numberImage != null && numberSprites != null)
        {
            int idx = data.levelNumber;
            if (idx >= 0 && idx < numberSprites.Length && numberSprites[idx] != null)
            {
                numberImage.sprite = numberSprites[idx];
                numberImage.preserveAspect = true;
            }
            else
            {
                // Không bắt buộc, nhưng log để bạn dễ biết thiếu sprite số nào
                Debug.LogWarning($"[LevelButton] numberSprites missing index {idx} for {name}.", this);
            }
        }

        // --- Set BG + interactable ---
        if (!data.isUnlocked)
        {
            if (bgImage != null)
            {
                bgImage.sprite = lockedBG;
                bgImage.preserveAspect = true;
            }

            if (button != null) button.interactable = false;
        }
        else
        {
            if (button != null) button.interactable = true;

            int s = Mathf.Clamp(data.stars, 0, 3);

            if (bgImage != null)
            {
                switch (s)
                {
                    case 0:
                        // Nếu bạn chưa có bg0Star, tạm fallback về bg1Star để vẫn có nền
                        bgImage.sprite = (bg0Star != null) ? bg0Star : bg1Star;
                        break;
                    case 1:
                        bgImage.sprite = bg1Star;
                        break;
                    case 2:
                        bgImage.sprite = bg2Star;
                        break;
                    case 3:
                        bgImage.sprite = bg3Star;
                        break;
                }

                bgImage.preserveAspect = true;
            }
        }

        // --- Click handler ---
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        Debug.Log($"[LevelButton] CLICK {name}");

        if (data == null)
        {
            Debug.LogWarning($"[LevelButton] data NULL on {name} (Setup chưa chạy)");
            return;
        }

        Debug.Log($"[LevelButton] level={data.levelNumber} unlocked={data.isUnlocked} scene='{data.sceneName}'");

        if (!data.isUnlocked) return;

        UnityEngine.SceneManagement.SceneManager.LoadScene(data.sceneName);
        Debug.Log("[LevelButton] CLICK " + name);
    }




#if UNITY_EDITOR
    // Tự động cố gắng lấy Button nếu bạn quên kéo thả
    private void OnValidate()
    {
        if (button == null) button = GetComponent<Button>();
    }
#endif
}
