using UnityEngine;

namespace Runtime.UI
{
    public class LevelSelectionManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private int totalLevels = LevelDataManager.TOTAL_LEVELS;

        [Header("Level Buttons (drag LevelButton_1..LevelButton_5 in order)")]
        [SerializeField] private LevelButton[] levelButtons;

        private void Awake()
        {
            // Nếu bạn quên gán hoặc gán sai, tự dùng TOTAL_LEVELS trong LevelDataManager
            if (totalLevels <= 0)
                totalLevels = LevelDataManager.TOTAL_LEVELS;
        }
        private void Start()
        {
            Debug.Log("TEST LOG: Scene LevelSelection is running");
        }

        private void OnEnable()
        {
            InitializeLevels();
        }

        /// <summary>
        /// Load dữ liệu từng level và cập nhật UI button.
        /// </summary>
        public void InitializeLevels()
        {
            if (levelButtons == null || levelButtons.Length == 0)
            {
                Debug.LogWarning("[LevelSelectionManager] levelButtons is not assigned.", this);
                return;
            }

            int count = Mathf.Min(totalLevels, levelButtons.Length);

            for (int i = 0; i < count; i++)
            {
                int levelNumber = i + 1; // level bắt đầu từ 1
                LevelData data = LevelDataManager.LoadLevelData(levelNumber);

                if (levelButtons[i] == null)
                {
                    Debug.LogWarning($"[LevelSelectionManager] levelButtons[{i}] is null (level {levelNumber}).", this);
                    continue;
                }

                levelButtons[i].Setup(data);
            }
        }

        // =======================
        // Debug / tiện cho test
        // =======================

        [ContextMenu("Reset All Levels")]
        public void ResetAllLevels()
        {
            LevelDataManager.ResetAllLevels();
            InitializeLevels();
        }

        [ContextMenu("Unlock All Levels (Debug)")]
        public void UnlockAllLevelsForDebug()
        {
            LevelDataManager.UnlockAllLevelsForDebug();
            InitializeLevels();
        }
    }
}
