using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class EndGameUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("Canvas Settings")]
    [Tooltip("Canvas chứa EndGame UI - sẽ tự động set sorting order cao")]
    public Canvas endGameCanvas;
    
    [Tooltip("Sorting order cao để đảm bảo hiển thị trên cùng")]
    public int canvasSortingOrder = 1000;

    [Header("Title")]
    public TextMeshProUGUI titleText;
    public string winTitle = "HOÀN THÀNH!";
    public string loseTitle = "THẤT BẠI!";

    [Header("Stars")]
    public Image star1;
    public Image star2;
    public Image star3;
    public Sprite starOn;
    public Sprite starOff;

    [Header("Score")]
    public TextMeshProUGUI scoreText;

    [Header("Buttons")]
    public Button restartButton;
    public Button nextLevelButton;
    public Button menuButton;

    private CanvasGroup canvasGroup;
    private GraphicRaycaster raycaster;

    void Awake()
    {
        // Tự động tìm Canvas nếu chưa assign
        if (endGameCanvas == null)
        {
            endGameCanvas = GetComponentInParent<Canvas>();
            if (endGameCanvas == null)
                endGameCanvas = GetComponent<Canvas>();
        }

        // Setup Canvas để đảm bảo hiển thị trên cùng
        SetupCanvas();

        // Ẩn panel bằng CanvasGroup thay vì SetActive
        // Điều này đảm bảo panel vẫn active và có thể Show được
        if (panel != null)
        {
            // Đảm bảo panel LUÔN active
            panel.SetActive(true);
            
            // Ẩn bằng CanvasGroup
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

        // Setup button listeners
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);

        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuClicked);
    }

    void SetupCanvas()
    {
        if (endGameCanvas == null) return;

        // Đảm bảo Canvas có sorting order cao nhất
        endGameCanvas.overrideSorting = true;
        endGameCanvas.sortingOrder = canvasSortingOrder;

        // Đảm bảo có GraphicRaycaster để nhận input
        raycaster = endGameCanvas.GetComponent<GraphicRaycaster>();
        if (raycaster == null)
        {
            raycaster = endGameCanvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        // Thêm CanvasGroup để control interactivity
        if (panel != null)
        {
            canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = panel.AddComponent<CanvasGroup>();
            }
        }

        // Đảm bảo có EventSystem trong scene
        EnsureEventSystem();
    }

    void EnsureEventSystem()
    {
        var eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            var eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
            Debug.Log("[EndGameUI] Created EventSystem");
        }
    }
    
    /// <summary>
    /// Active tất cả parent từ root xuống để đảm bảo child có thể active được
    /// </summary>
    void ActivateAllParents(Transform child)
    {
        // Thu thập tất cả parent vào list
        var parents = new System.Collections.Generic.List<GameObject>();
        Transform current = child.parent;
        while (current != null)
        {
            parents.Add(current.gameObject);
            current = current.parent;
        }
        
        // Active từ root xuống (đảo ngược list)
        for (int i = parents.Count - 1; i >= 0; i--)
        {
            if (!parents[i].activeSelf)
            {
                Debug.LogWarning($"[EndGameUI] Activating parent: {parents[i].name}");
                parents[i].SetActive(true);
            }
        }
    }

    public void Show(bool isWin, int stars, int safeCount, int totalCount)
    {
        Debug.Log($"[EndGameUI] Show called - isWin: {isWin}, stars: {stars}");
        
        if (panel == null)
        {
            Debug.LogError("[EndGameUI] panel is NULL!");
            return;
        }

        // Đảm bảo panel và tất cả parent đều active
        ActivateAllParents(panel.transform);
        panel.SetActive(true);

        // Đảm bảo Canvas được enable và có sorting order cao
        if (endGameCanvas != null)
        {
            endGameCanvas.enabled = true;
            endGameCanvas.overrideSorting = true;
            endGameCanvas.sortingOrder = canvasSortingOrder;
        }

        // HIỆN PANEL bằng CanvasGroup (đây là cách chính để show)
        if (canvasGroup == null)
        {
            canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = panel.AddComponent<CanvasGroup>();
            }
        }
        
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        Debug.Log($"[EndGameUI] CanvasGroup set - alpha: {canvasGroup.alpha}, interactable: {canvasGroup.interactable}");

        // Pause game
        Time.timeScale = 0f;

        // Title
        if (titleText != null)
            titleText.text = isWin ? winTitle : loseTitle;

        // Stars
        SetStars(stars);

        // Score
        if (scoreText != null)
            scoreText.text = $"{safeCount}/{totalCount} học sinh an toàn";

        // Next level button chỉ hiện khi thắng
        if (nextLevelButton != null)
            nextLevelButton.gameObject.SetActive(isWin);

        Debug.Log($"[EndGameUI] Panel shown! activeInHierarchy: {panel.activeInHierarchy}, Canvas order: {endGameCanvas?.sortingOrder}");
    }

    void SetStars(int count)
    {
        if (star1 != null)
            star1.sprite = count >= 1 ? starOn : starOff;

        if (star2 != null)
            star2.sprite = count >= 2 ? starOn : starOff;

        if (star3 != null)
            star3.sprite = count >= 3 ? starOn : starOff;
    }

    void OnRestartClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartLevel();
    }

    void OnNextLevelClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.NextLevel();
    }

    void OnMenuClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.GoToLevelSelection();
    }

    void OnDestroy()
    {
        if (restartButton != null)
            restartButton.onClick.RemoveListener(OnRestartClicked);

        if (nextLevelButton != null)
            nextLevelButton.onClick.RemoveListener(OnNextLevelClicked);

        if (menuButton != null)
            menuButton.onClick.RemoveListener(OnMenuClicked);
    }
}
