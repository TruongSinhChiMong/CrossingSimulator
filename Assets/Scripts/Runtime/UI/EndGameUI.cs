using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndGameUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

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

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);

        // Setup button listeners
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);

        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuClicked);
    }

    public void Show(bool isWin, int stars, int safeCount, int totalCount)
    {
        if (panel == null) return;

        panel.SetActive(true);

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
