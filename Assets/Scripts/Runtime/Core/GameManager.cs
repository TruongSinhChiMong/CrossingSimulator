using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Info")]
    [Tooltip("Số level hiện tại (Map1 = 1, Map2 = 2, ...)")]
    public int currentLevel = 1;

    [Header("Game State")]
    [Tooltip("Tổng số học sinh trong level này.")]
    public int totalStudents = 6;

    [Tooltip("Số học sinh đã qua đường an toàn.")]
    public int safeStudents = 0;

    [Tooltip("Số học sinh bị tai nạn.")]
    public int hitStudents = 0;

    [Header("Win Condition")]
    [Tooltip("Tỉ lệ học sinh qua an toàn tối thiểu để thắng (0–1).")]
    [Range(0f, 1f)]
    public float winThreshold = 0.5f;

    [Header("References")]
    [Tooltip("Spawner hiện tại, dùng để lấy totalStudents nếu cần.")]
    public StudentSpawner studentSpawner;

    [Tooltip("UI hiển thị màn hình thắng/thua, sao, thống kê.")]
    public EndGameUI endGameUI;

    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        isGameOver = false;
        safeStudents = 0;
        hitStudents = 0;

        // Lấy số học sinh từ StudentSpawner (nếu có), để khỏi nhập tay 2 nơi
        if (studentSpawner != null)
        {
            totalStudents = studentSpawner.TotalStudents;
            Debug.Log($"[GameManager] Got totalStudents from spawner: {totalStudents}");
        }
        else
        {
            Debug.LogWarning($"[GameManager] studentSpawner is NULL! Using default totalStudents: {totalStudents}");
        }

        // Log endGameUI status
        if (endGameUI == null)
        {
            Debug.LogError("[GameManager] endGameUI is NOT assigned in Inspector!");
        }
        else
        {
            Debug.Log($"[GameManager] endGameUI is assigned: {endGameUI.gameObject.name}");
        }

        // Parse level number từ tên scene (Map1, Map2, ...)
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.StartsWith("Map") &&
            int.TryParse(sceneName.Substring(3), out int level))
        {
            currentLevel = level;
        }
        
        Debug.Log($"[GameManager] Started - Level: {currentLevel}, TotalStudents: {totalStudents}");
    }

    /// <summary>
    /// Gọi khi học sinh qua đường an toàn.
    /// </summary>
    public void OnStudentSafe()
    {
        if (isGameOver) return;

        safeStudents++;

        Debug.Log($"[GameManager] Student safe: {safeStudents}/{totalStudents}");
        CheckEndGame();
    }

    /// <summary>
    /// Gọi khi học sinh bị tai nạn.
    /// </summary>
    public void OnStudentHit()
    {
        if (isGameOver) return;

        hitStudents++;

        Debug.Log($"[GameManager] Student hit: {hitStudents}/{totalStudents}");
        CheckEndGame();
    }

    /// <summary>
    /// Kiểm tra xem đã xử lý xong tất cả học sinh chưa và quyết định thắng/thua.
    /// </summary>
    private void CheckEndGame()
    {
        int processedStudents = safeStudents + hitStudents;
        Debug.Log($"[GameManager] CheckEndGame: processed={processedStudents}, total={totalStudents}, safe={safeStudents}, hit={hitStudents}");
        
        if (processedStudents < totalStudents) return;

        isGameOver = true;

        int minSafeToWin = Mathf.CeilToInt(totalStudents * winThreshold);
        bool isWin = safeStudents >= minSafeToWin;
        int stars = CalculateStars();

        Debug.Log($"[GameManager] Game Over! Win: {isWin}, Stars: {stars}, minSafeToWin: {minSafeToWin}");

        // Lưu tiến độ nếu thắng
        if (isWin && stars > 0)
        {
            Runtime.UI.LevelDataManager.SaveLevelData(currentLevel, stars);
        }

        // Hiển thị UI kết thúc
        if (endGameUI != null)
        {
            Debug.Log($"[GameManager] Showing EndGameUI...");
            endGameUI.Show(isWin, stars, safeStudents, totalStudents);
        }
        else
        {
            Debug.LogError("[GameManager] endGameUI is NULL! Cannot show end game screen.");
        }
    }

    /// <summary>
    /// Tính số sao dựa trên tỉ lệ học sinh an toàn.
    /// </summary>
    private int CalculateStars()
    {
        if (totalStudents <= 0) return 0;

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

        // Nếu scene tồn tại thì load, không thì quay về LevelSelection
        if (Application.CanStreamedLevelBeLoaded(nextScene))
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            SceneManager.LoadScene("LevelSelection");
        }
    }

    public void GoToLevelSelection()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LevelSelection");
    }
}
