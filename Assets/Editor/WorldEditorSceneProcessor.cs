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
    /// This script runs on editor load to aid in designing the world regions
    /// </summary>
    [InitializeOnLoad]
    public static class WorldEditorSceneProcessor
    {
        private static bool enteringPlayMode;

        /// <summary>
        /// Called on scene change
        /// </summary>
        static WorldEditorSceneProcessor()
        {
            EditorApplication.hierarchyChanged += checkScene;
            EditorApplication.playModeStateChanged += PlayModeChanged;
        }

        /// <summary>
        /// Called on play mode changed event
        /// </summary>
        /// <param name="obj"></param>
        private static void PlayModeChanged(PlayModeStateChange stateChange)
        {
            if (stateChange == PlayModeStateChange.EnteredEditMode)
            {
                enteringPlayMode = false;
                checkScene();
            }
            else if (stateChange == PlayModeStateChange.ExitingEditMode)
            {
                enteringPlayMode = true;
                UnloadWorldScenes();                
            }
        }

        /// <summary>
        /// Check if scene is the world editor scene
        /// </summary>
        private static void checkScene()
        {
            if (enteringPlayMode == false && Application.isPlaying == false && EditorSceneManager.GetActiveScene().path.Equals(SharedConfig.MAIN_SCENE_PATH))
            {                
                LoadWorld();
                SetDefaultSkybox();
            }
        }

        /// <summary>
        /// Unloads all additive scenes except active
        /// </summary>
        private static void UnloadWorldScenes()
        {
            if (Application.isPlaying == false && EditorSceneManager.GetActiveScene().path.Equals(SharedConfig.MAIN_SCENE_PATH))
            {
                int sceneCount = SceneManager.sceneCount;
                for (int i = 0; i < sceneCount; i++)
                {
                    Scene scene = EditorSceneManager.GetSceneAt(i);
                    if (scene.path.StartsWith(SharedConfig.WORLD_SCENES_FOLDER))
                    {
                        AsyncOperation sceneOp = EditorSceneManager.UnloadSceneAsync(scene);

                    }
                }
            }
        }

        /// <summary>
        /// Sets the scene skybox back to default for editing
        /// </summary>
        private static void SetDefaultSkybox()
        {
            RenderSettings.skybox.SetTexture("_Tex1", AssetDatabase.LoadAssetAtPath<Cubemap>("Assets/Settings/Editor Skybox.png"));
            RenderSettings.skybox.SetFloat("_Opacity1", 1);
            RenderSettings.skybox.SetFloat("_Opacity2", 0);
            RenderSettings.skybox.SetFloat("_Opacity3", 0);
            RenderSettings.skybox.SetFloat("_Opacity4", 0);
        }

        /// <summary>
        /// Loads the world regions into the scene
        /// </summary>
        private static void LoadWorld()
        {

            string[] sceneAssets = AssetDatabase.FindAssets("t:Scene", new[] { SharedConfig.WORLD_SCENES_FOLDER });
            foreach (string sceneAsset in sceneAssets)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneAsset);

                //Load scene
                LoadScene(scenePath);

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

            GameObject rootObject = rootObjects[0];
            rootObject.transform.position = new Vector3(chunk.x * SharedConfig.WORLD_CHUNK_SIZE, 0, chunk.y * SharedConfig.WORLD_CHUNK_SIZE);

        }

    }

}