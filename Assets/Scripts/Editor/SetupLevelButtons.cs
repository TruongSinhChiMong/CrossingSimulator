using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

namespace CrossingSimulator.Editor
{
    public class SetupLevelButtons
    {
        [MenuItem("Tools/Setup Level Selection Buttons")]
        public static void CreateLevelButtons()
        {
            // Tìm Canvas
            Canvas canvas = GameObject.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No Canvas found in scene! Please open LevelSelection scene first.");
                return;
            }
            
            // Tìm button template (LevelButton_1)
            GameObject template = GameObject.Find("LevelButton_1");
            if (template == null)
            {
                Debug.LogError("LevelButton_1 not found! Please create it first.");
                return;
            }
            
            // Tạo 4 button còn lại
            for (int i = 2; i <= 5; i++)
            {
                string buttonName = "LevelButton_" + i;
                
                // Check nếu đã tồn tại thì skip
                if (GameObject.Find(buttonName) != null)
                {
                    Debug.Log(buttonName + " already exists, skipping...");
                    continue;
                }
                
                // Duplicate template
                GameObject newButton = Object.Instantiate(template, canvas.transform);
                newButton.name = buttonName;
                
                // Set position
                RectTransform rectTransform = newButton.GetComponent<RectTransform>();
                float xPos = -400f + ((i - 1) * 200f);
                rectTransform.anchoredPosition = new Vector2(xPos, 50f);
                
                // Update text
                TextMeshProUGUI textTMP = newButton.GetComponentInChildren<TextMeshProUGUI>();
                if (textTMP != null)
                {
                    textTMP.text = i.ToString();
                }
                
                Text text = newButton.GetComponentInChildren<Text>();
                if (text != null)
                {
                    text.text = i.ToString();
                }
                
                Debug.Log("Created " + buttonName + " at position x=" + xPos);
            }
            
            // Mark scene as dirty
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            
            Debug.Log("<color=green>Successfully created 4 level buttons! Don't forget to assign them to LevelSelectionManager.</color>");
        }
    }
}
