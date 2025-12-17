using UnityEngine;

namespace Runtime.UI
{
    [System.Serializable]
    public class LevelData
    {
        public int levelNumber;
        public bool isUnlocked;
        public int stars; // 0 = chưa chơi, 1-3 = số sao đạt được
        public string sceneName;
    }

    public class LevelDataManager : MonoBehaviour
    {
        private const string LEVEL_DATA_KEY = "LevelData_";
        
        public static void SaveLevelData(int levelNumber, int stars)
        {
            PlayerPrefs.SetInt(LEVEL_DATA_KEY + levelNumber + "_Stars", stars);
            PlayerPrefs.SetInt(LEVEL_DATA_KEY + levelNumber + "_Unlocked", 1);
            
            // Mở khóa level tiếp theo nếu hoàn thành
            if (stars > 0)
            {
                PlayerPrefs.SetInt(LEVEL_DATA_KEY + (levelNumber + 1) + "_Unlocked", 1);
            }
            
            PlayerPrefs.Save();
        }
        
        public static LevelData LoadLevelData(int levelNumber)
        {
            LevelData data = new LevelData
            {
                levelNumber = levelNumber,
                isUnlocked = PlayerPrefs.GetInt(LEVEL_DATA_KEY + levelNumber + "_Unlocked", levelNumber == 1 ? 1 : 0) == 1,
                stars = PlayerPrefs.GetInt(LEVEL_DATA_KEY + levelNumber + "_Stars", 0),
                sceneName = "Map" + levelNumber
            };
            
            return data;
        }
    }
}
