using UnityEngine;
using UnityEditor;
using Runtime.UI;

namespace CrossingSimulator.Editor
{
    public class AssignSpritesToButtons
    {
        [MenuItem("Tools/Auto Assign Sprites to Level Buttons")]
        public static void AssignSprites()
        {
            // Load sprites
            Sprite blockStar = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/block_star.png");
            Sprite oneStar = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/1_star.png");
            Sprite twoStar = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/2_star.png");
            Sprite threeStar = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Sprites/UI/3_star.png");
            
            if (blockStar == null || oneStar == null || twoStar == null || threeStar == null)
            {
                Debug.LogError("Could not load star sprites! Check if they exist in Assets/Art/Sprites/UI/");
                return;
            }
            
            // Find all LevelButton components in scene
            LevelButton[] buttons = Object.FindObjectsOfType<LevelButton>();
            
            if (buttons.Length == 0)
            {
                Debug.LogWarning("No LevelButton components found in scene!");
                return;
            }
            
            foreach (LevelButton button in buttons)
            {
                button.blockStarSprite = blockStar;
                button.oneStarSprite = oneStar;
                button.twoStarSprite = twoStar;
                button.threeStarSprite = threeStar;
                
                EditorUtility.SetDirty(button);
                
                Debug.Log("Assigned sprites to " + button.gameObject.name);
            }
            
            Debug.Log("<color=green>Successfully assigned sprites to " + buttons.Length + " buttons!</color>");
        }
    }
}
