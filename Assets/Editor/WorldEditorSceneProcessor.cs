using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using Engine.Configuration;

namespace Editor
{
    /// <summary>
    /// The world editor runs in editor to aid in designing the world regions
    /// </summary>
    [InitializeOnLoad]
    public static class WorldEditorSceneProcessor
    {

        /// <summary>
        /// Called on scene change
        /// </summary>
        static WorldEditorSceneProcessor()
        {
            EditorApplication.hierarchyChanged += checkScene;
        }

        /// <summary>
        /// Check if scene is the world editor scene
        /// </summary>
        private static void checkScene()
        {
            if (EditorSceneManager.GetActiveScene().name.Equals("World_Editor"))
            {
                LoadWorld();
            }
        }

        /// <summary>
        /// Loads the world regions into the scene
        /// </summary>
        private static void LoadWorld()
        {
            string[] sceneAssets = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes/World" });
            foreach (string sceneAsset in sceneAssets)
            {
                LoadScene(AssetDatabase.GUIDToAssetPath(sceneAsset));
            }
        }

        /// <summary>
        /// Load a specific chunk scene by name
        /// </summary>
        /// <param name="scene"></param>
        private static void LoadScene(string scenePath)
        {
            string sceneName = Path.GetFileName(scenePath).Replace(".unity", "");
            if (EditorSceneManager.GetSceneByName(sceneName).isLoaded)
                return;
             
            Vector2Int chunk = new Vector2Int(Int32.Parse(sceneName.Split(',')[0]), Int32.Parse(sceneName.Split(',')[1]));

            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            Scene scene = EditorSceneManager.GetSceneByName(sceneName);
            GameObject[] rootObjects = scene.GetRootGameObjects();

            if (rootObjects.Length == 0)
            {
                Debug.LogError("Scene " + sceneName + " is missing a root object.");
            }
            else
            if (rootObjects.Length > 1)
            {
                Debug.LogError("Scene " + sceneName + " has more than one root object.");
            }

            GameObject rootObject = rootObjects[0];
            rootObject.transform.position = new Vector3(chunk.x * SharedConfig.WORLD_CHUNK_SIZE, 0, chunk.y * SharedConfig.WORLD_CHUNK_SIZE);
        }
    }

}