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
        if (panel != null)
            panel.SetActive(false);

        // Tự động tìm Canvas nếu chưa assign
        if (endGameCanvas == null)
        {
            endGameCanvas = GetComponentInParent<Canvas>();
            if (endGameCanvas == null)
                endGameCanvas = GetComponent<Canvas>();
        }

        // Setup Canvas để đảm bảo hiển thị trên cùng
        SetupCanvas();

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

    public void Show(bool isWin, int stars, int safeCount, int totalCount)
    {
        if (panel == null) return;

        panel.SetActive(true);

        // Đảm bảo Canvas sorting order cao nhất khi show
        if (endGameCanvas != null)
        {
            endGameCanvas.overrideSorting = true;
            endGameCanvas.sortingOrder = canvasSortingOrder;
        }

        // Đảm bảo CanvasGroup cho phép tương tác
        if (canvasGroup != null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }

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

        Debug.Log($"[EndGameUI] Panel shown - Canvas sorting order: {endGameCanvas?.sortingOrder}");
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
