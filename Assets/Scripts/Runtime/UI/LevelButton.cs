using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Runtime.UI
{
    public class LevelButton : MonoBehaviour
    {
        [Header("References")]
        public Button button;
        public Image starImage;
        
        [Header("Text (Use one of these)")]
        public Text levelNumberText;
        public TextMeshProUGUI levelNumberTextTMP;
        
        [Header("Star Sprites")]
        public Sprite blockStarSprite;
        public Sprite oneStarSprite;
        public Sprite twoStarSprite;
        public Sprite threeStarSprite;
        
        private LevelData levelData;
        
        private void Reset()
        {
            // Tự động tìm và gắn references khi add component
            if (button == null)
                button = GetComponent<Button>();
            
            if (starImage == null)
            {
                Transform starTransform = transform.Find("StarImage");
                if (starTransform != null)
                    starImage = starTransform.GetComponent<Image>();
            }
            
            // Tìm Text hoặc TextMeshPro
            Transform textTransform = transform.Find("LevelNumberText");
            if (textTransform != null)
            {
                if (levelNumberText == null)
                    levelNumberText = textTransform.GetComponent<Text>();
                
                if (levelNumberTextTMP == null)
                    levelNumberTextTMP = textTransform.GetComponent<TextMeshProUGUI>();
            }
        }
        
        public void Initialize(LevelData data)
        {
            levelData = data;
            
            Debug.Log($"[LevelButton] Initializing {gameObject.name} - Level {data.levelNumber}, Unlocked: {data.isUnlocked}, Stars: {data.stars}");
            
            // Check sprites
            if (blockStarSprite == null)
                Debug.LogWarning($"[LevelButton] {gameObject.name} is missing blockStarSprite!");
            if (oneStarSprite == null)
                Debug.LogWarning($"[LevelButton] {gameObject.name} is missing oneStarSprite!");
            if (twoStarSprite == null)
                Debug.LogWarning($"[LevelButton] {gameObject.name} is missing twoStarSprite!");
            if (threeStarSprite == null)
                Debug.LogWarning($"[LevelButton] {gameObject.name} is missing threeStarSprite!");
            
            // Set text cho cả 2 loại
            string levelText = data.levelNumber.ToString();
            if (levelNumberText != null)
            {
                levelNumberText.text = levelText;
            }
            if (levelNumberTextTMP != null)
            {
                levelNumberTextTMP.text = levelText;
            }
            
            UpdateStarDisplay();
            
            if (button != null)
            {
                button.interactable = data.isUnlocked;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(OnLevelButtonClicked);
            }
        }
        
        private void UpdateStarDisplay()
        {
            if (starImage == null)
            {
                Debug.LogWarning("StarImage is null on " + gameObject.name);
                return;
            }
            
            // Reset color về trắng đầy đủ
            starImage.color = Color.white;
            
            if (!levelData.isUnlocked)
            {
                // Level bị khóa
                if (blockStarSprite != null)
                {
                    starImage.sprite = blockStarSprite;
                }
                else
                {
                    Debug.LogWarning("Block star sprite is missing on " + gameObject.name);
                    starImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Màu xám nếu thiếu sprite
                }
            }
            else
            {
                // Level đã mở khóa
                switch (levelData.stars)
                {
                    case 1:
                        starImage.sprite = oneStarSprite != null ? oneStarSprite : blockStarSprite;
                        break;
                    case 2:
                        starImage.sprite = twoStarSprite != null ? twoStarSprite : blockStarSprite;
                        break;
                    case 3:
                        starImage.sprite = threeStarSprite != null ? threeStarSprite : blockStarSprite;
                        break;
                    default:
                        // Chưa có sao (chưa chơi)
                        starImage.sprite = oneStarSprite != null ? oneStarSprite : blockStarSprite;
                        starImage.color = new Color(1f, 1f, 1f, 0.3f); // Làm mờ nếu chưa có sao
                        break;
                }
            }
        }
        
        private void OnLevelButtonClicked()
        {
            if (levelData.isUnlocked)
            {
                SceneManager.LoadScene(levelData.sceneName);
            }
        }
    }
}
