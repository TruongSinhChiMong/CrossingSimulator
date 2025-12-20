using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using CrossingSimulator.Networking;

namespace Runtime.UI
{
    public class LevelSelectionManager : MonoBehaviour
    {
        [Header("Level Buttons")]
        [SerializeField] private LevelButton[] levelButtons;
        
        [Header("Settings")]
        [SerializeField] private int totalLevels = 5;

        [Header("Settings Popup")]
        [SerializeField] private SettingsPopup settingsPopup;
        [SerializeField] private Button settingsButton;

        [Header("Loading")]
        [SerializeField] private GameObject loadingIndicator;

        private GameData cachedGameData;
        
        private void Start()
        {
            SetupSettingsButton();
            LoadGameDataFromApi();
        }

        private void SetupSettingsButton()
        {
            if (settingsButton != null)
                settingsButton.onClick.AddListener(OpenSettings);
        }

        private void LoadGameDataFromApi()
        {
            ShowLoading(true);

            GameDataService.Instance.GetGameData((success, gameData) =>
            {
                ShowLoading(false);
                cachedGameData = gameData ?? CreateDefaultGameData();
                InitializeLevels();
            });
        }

        private GameData CreateDefaultGameData()
        {
            // Default: level 1 mở, 0 sao
            var data = new GameData
            {
                unlockLevel = 1,
                levels = new System.Collections.Generic.List<LevelProgress>()
            };

            for (int i = 1; i <= totalLevels; i++)
            {
                data.levels.Add(new LevelProgress
                {
                    map = "Map" + i,
                    score = "0",
                    star = 0,
                    unlock = (i == 1) // Chỉ level 1 mở
                });
            }

            return data;
        }
        
        private void InitializeLevels()
        {
            if (levelButtons == null || levelButtons.Length == 0)
            {
                Debug.LogWarning("No level buttons assigned! Please assign level buttons in Inspector.");
                return;
            }
            
            for (int i = 0; i < levelButtons.Length && i < totalLevels; i++)
            {
                if (levelButtons[i] != null)
                {
                    LevelData data = GetLevelDataFromCache(i + 1);
                    levelButtons[i].Initialize(data);
                }
            }
        }

        private LevelData GetLevelDataFromCache(int levelNumber)
        {
            var levelData = new LevelData
            {
                levelNumber = levelNumber,
                sceneName = "Map" + levelNumber,
                isUnlocked = (levelNumber == 1), // Default: chỉ level 1 mở
                stars = 0
            };

            if (cachedGameData?.levels != null)
            {
                var progress = cachedGameData.levels.Find(l => l.map == "Map" + levelNumber);
                if (progress != null)
                {
                    levelData.isUnlocked = progress.unlock;
                    levelData.stars = progress.star;
                }
                else if (levelNumber <= cachedGameData.unlockLevel)
                {
                    // Fallback: dùng unlockLevel
                    levelData.isUnlocked = true;
                }
            }

            return levelData;
        }

        private void ShowLoading(bool show)
        {
            if (loadingIndicator != null)
                loadingIndicator.SetActive(show);
        }
        
        public void BackToLogin()
        {
            SceneManager.LoadScene("Login");
        }
        
        public void UnlockAllLevels()
        {
            if (cachedGameData == null)
                cachedGameData = CreateDefaultGameData();

            cachedGameData.unlockLevel = totalLevels;
            foreach (var level in cachedGameData.levels)
            {
                level.unlock = true;
            }

            // Save to API
            GameDataService.Instance.SaveGameData(cachedGameData, (success, msg) =>
            {
                Debug.Log($"[LevelSelection] UnlockAllLevels: {msg}");
            });

            InitializeLevels();
        }
        
        public void ResetAllLevels()
        {
            cachedGameData = CreateDefaultGameData();

            // Save to API
            GameDataService.Instance.SaveGameData(cachedGameData, (success, msg) =>
            {
                Debug.Log($"[LevelSelection] ResetAllLevels: {msg}");
            });

            InitializeLevels();
        }

        public void OpenSettings()
        {
            if (settingsPopup != null)
                settingsPopup.Show();
            else
                Debug.LogWarning("[LevelSelection] SettingsPopup not assigned!");
        }

        void OnDestroy()
        {
            if (settingsButton != null)
                settingsButton.onClick.RemoveListener(OpenSettings);
        }
    }
}
