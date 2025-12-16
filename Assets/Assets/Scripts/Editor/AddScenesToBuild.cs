using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CrossingSimulator.Editor
{
    public class AddScenesToBuild
    {
        [MenuItem("Tools/Add All Scenes To Build Settings")]
        public static void AddAllScenesToBuildSettings()
        {
            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
            
            // Thêm các scene theo thứ tự ưu tiên
            string[] sceneOrder = new string[] 
            { 
                "Assets/Scenes/Login.unity",
                "Assets/Scenes/LevelSelection.unity",
                "Assets/Scenes/Map1.unity",
                "Assets/Scenes/Map2.unity",
                "Assets/Scenes/Map3.unity",
                "Assets/Scenes/Map4.unity",
                "Assets/Scenes/Map5.unity",
                "Assets/Scenes/SampleScene.unity"
            };
            
            foreach (string scenePath in sceneOrder)
            {
                // Check if file exists in file system
                string fullPath = Path.Combine(Application.dataPath, "..", scenePath);
                if (File.Exists(fullPath))
                {
                    editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                    Debug.Log("Added scene to build: " + scenePath);
                }
                else
                {
                    Debug.LogWarning("Scene not found: " + scenePath);
                }
            }
            
            if (editorBuildSettingsScenes.Count > 0)
            {
                EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
                
                Debug.Log("<color=green>Successfully added " + editorBuildSettingsScenes.Count + " scenes to Build Settings!</color>");
                Debug.Log("Scene order:");
                for (int i = 0; i < editorBuildSettingsScenes.Count; i++)
                {
                    Debug.Log("  [" + i + "] " + editorBuildSettingsScenes[i].path);
                }
            }
            else
            {
                Debug.LogError("No scenes found to add!");
            }
            
            AssetDatabase.Refresh();
        }
    }
}
