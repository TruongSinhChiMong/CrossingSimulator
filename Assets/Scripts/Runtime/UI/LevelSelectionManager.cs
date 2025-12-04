using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.UI
{
    public class LevelSelectionManager : MonoBehaviour
    {
        [Header("Level Buttons")]
        [SerializeField] private LevelButton[] levelButtons;
        
        [Header("Settings")]
        [SerializeField] private int totalLevels = 5;
        
        private void Start()
        {
            InitializeLevels();
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
                    LevelData data = LevelDataManager.LoadLevelData(i + 1);
                    levelButtons[i].Initialize(data);
                }
            }
        }
        
        public void BackToLogin()
        {
            SceneManager.LoadScene("Login");
        }
        
        public void UnlockAllLevels()
        {
            for (int i = 1; i <= totalLevels; i++)
            {
                LevelDataManager.SaveLevelData(i, 0);
            }
            InitializeLevels();
        }
        
        public void ResetAllLevels()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            InitializeLevels();
        }

        public void OpenSettings()
        {
            SceneManager.LoadScene("Settings");
        }
    }
}
