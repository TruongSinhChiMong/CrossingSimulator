using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Info")]
    public int currentLevel = 1;

    [Header("Game State")]
    public int totalStudents = 6;
    public int safeStudents = 0;
    public int hitStudents = 0;

    [Header("Win Condition")]
    [Tooltip("Số học sinh tối thiểu qua an toàn để thắng (% của total)")]
    [Range(0f, 1f)]
    public float winThreshold = 0.5f;

    [Header("References")]
    public ProgressManager progressManager;
    public EndGameUI endGameUI;

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        isGameOver = false;
        safeStudents = 0;
        hitStudents = 0;

        // Parse level number from scene name (Map1, Map2, etc.)
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.StartsWith("Map") && int.TryParse(sceneName.Substring(3), out int level))
        {
            currentLevel = level;
        }
    }

    /// <summary>
    /// Gọi khi học sinh qua đường an toàn
    /// </summary>
    public void OnStudentSafe()
    {
        if (isGameOver) return;

        safeStudents++;
        progressManager?.AddPoint(1);

        Debug.Log($"[GameManager] Student safe: {safeStudents}/{totalStudents}");
        CheckEndGame();
    }

    /// <summary>
    /// Gọi khi học sinh bị tai nạn
    /// </summary>
    public void OnStudentHit()
    {
        if (isGameOver) return;

        hitStudents++;
        Debug.Log($"[GameManager] Student hit: {hitStudents}/{totalStudents}");
        CheckEndGame();
    }

    void CheckEndGame()
    {
        int processedStudents = safeStudents + hitStudents;
        if (processedStudents < totalStudents) return;

        isGameOver = true;
        int minSafeToWin = Mathf.CeilToInt(totalStudents * winThreshold);
        bool isWin = safeStudents >= minSafeToWin;
        int stars = CalculateStars();

        Debug.Log($"[GameManager] Game Over! Win: {isWin}, Stars: {stars}");

        // Lưu tiến độ nếu thắng
        if (isWin && stars > 0)
        {
            Runtime.UI.LevelDataManager.SaveLevelData(currentLevel, stars);
        }

        // Hiển thị UI kết thúc
        if (endGameUI != null)
        {
            endGameUI.Show(isWin, stars, safeStudents, totalStudents);
        }
    }

    int CalculateStars()
    {
        float ratio = (float)safeStudents / totalStudents;
        if (ratio >= 1f) return 3;
        if (ratio >= 0.66f) return 2;
        if (ratio >= 0.33f) return 1;
        return 0;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        int nextLevel = currentLevel + 1;
        string nextScene = "Map" + nextLevel;

        // Kiểm tra scene có tồn tại không
        if (Application.CanStreamedLevelBeLoaded(nextScene))
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            // Quay về Level Selection nếu hết level
            SceneManager.LoadScene("LevelSelection");
        }
    }

    public void GoToLevelSelection()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelection");
    }
}
