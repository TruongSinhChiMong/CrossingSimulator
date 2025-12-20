using UnityEngine;

namespace Runtime.UI
{
    [System.Serializable]
    public class LevelData
    {
        public int levelNumber;
        public bool isUnlocked;
        public int stars;        // 0..3
        public string sceneName; // "Map1".."Map5"
    }

    public static class LevelDataManager
    {
        // === CONFIG ===
        public const int TOTAL_LEVELS = 5; // bạn đang có Map1..Map5

        // === KEYS ===
        private const string KEY_PREFIX = "LevelData_";
        private const string KEY_UNLOCKED_SUFFIX = "_Unlocked";
        private const string KEY_STARS_SUFFIX = "_Stars";

        /// <summary>
        /// Lưu kết quả của 1 level.
        /// - Lưu sao theo best (không bị giảm sao nếu chơi lại tệ hơn)
        /// - Nếu thắng (stars > 0) thì mở khóa level kế tiếp (nếu còn trong TOTAL_LEVELS)
        /// </summary>
        public static void SaveLevelData(int levelNumber, int stars)
        {
            if (levelNumber < 1 || levelNumber > TOTAL_LEVELS)
            {
                Debug.LogWarning($"[LevelDataManager] Invalid levelNumber={levelNumber}. TOTAL_LEVELS={TOTAL_LEVELS}");
                return;
            }

            int clampedStars = Mathf.Clamp(stars, 0, 3);

            // best-stars
            int oldStars = PlayerPrefs.GetInt(GetStarsKey(levelNumber), 0);
            int bestStars = Mathf.Max(oldStars, clampedStars);

            PlayerPrefs.SetInt(GetStarsKey(levelNumber), bestStars);

            // level này chắc chắn đã mở (vì đã chơi tới)
            PlayerPrefs.SetInt(GetUnlockedKey(levelNumber), 1);

            // chỉ mở level tiếp theo khi THẮNG thật sự (stars > 0)
            if (clampedStars > 0 && levelNumber < TOTAL_LEVELS)
            {
                PlayerPrefs.SetInt(GetUnlockedKey(levelNumber + 1), 1);
            }

            PlayerPrefs.Save();
        }

        /// <summary>
        /// Load dữ liệu 1 level. Mặc định:
        /// - Level 1: mở khóa
        /// - Level 2..TOTAL_LEVELS: khóa
        /// </summary>
        public static LevelData LoadLevelData(int levelNumber)
        {
            // clamp để không phát sinh level ngoài phạm vi
            int lv = Mathf.Clamp(levelNumber, 1, TOTAL_LEVELS);

            bool defaultUnlocked = (lv == 1);

            bool isUnlocked = PlayerPrefs.GetInt(GetUnlockedKey(lv), defaultUnlocked ? 1 : 0) == 1;
            int stars = Mathf.Clamp(PlayerPrefs.GetInt(GetStarsKey(lv), 0), 0, 3);

            return new LevelData
            {
                levelNumber = lv,
                isUnlocked = isUnlocked,
                stars = stars,
                sceneName = "Map" + lv
            };
        }

        /// <summary>
        /// Reset tiến độ chỉ liên quan tới level (không xóa toàn bộ PlayerPrefs của game).
        /// </summary>
        public static void ResetAllLevels()
        {
            for (int lv = 1; lv <= TOTAL_LEVELS; lv++)
            {
                PlayerPrefs.DeleteKey(GetUnlockedKey(lv));
                PlayerPrefs.DeleteKey(GetStarsKey(lv));
            }

            PlayerPrefs.Save();
        }

        /// <summary>
        /// (Tuỳ chọn) Mở khóa tất cả level để test.
        /// </summary>
        public static void UnlockAllLevelsForDebug()
        {
            for (int lv = 1; lv <= TOTAL_LEVELS; lv++)
            {
                PlayerPrefs.SetInt(GetUnlockedKey(lv), 1);
            }

            PlayerPrefs.Save();
        }

        private static string GetUnlockedKey(int levelNumber)
            => KEY_PREFIX + levelNumber + KEY_UNLOCKED_SUFFIX;

        private static string GetStarsKey(int levelNumber)
            => KEY_PREFIX + levelNumber + KEY_STARS_SUFFIX;
    }
}
