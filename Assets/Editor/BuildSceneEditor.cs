using Engine.Configuration;
using System.Collections.Generic;
using UnityEditor;

namespace Editor
{
    /// <summary>
    /// This script runs on editor load to set the build settings scenes
    /// </summary>
    [InitializeOnLoad]
    public static class BuildSceneEditor
    {
        /// <summary>
        /// Called on load/recompile
        /// </summary>
        static BuildSceneEditor()
        {
            UpdateBuildSettings();
        }

        /// <summary>
        /// Updates the scene list in the build settings
        /// </summary>
        private static void UpdateBuildSettings()
        {
            List<EditorBuildSettingsScene> scenesToBuild = new List<EditorBuildSettingsScene>();

            //Add Main and Login scenes
            scenesToBuild.Add(new EditorBuildSettingsScene(SharedConfig.MAIN_SCENE_PATH, true));
            scenesToBuild.Add(new EditorBuildSettingsScene(SharedConfig.LOGIN_SCENE_PATH, true));

            //Add world scenes
            string[] sceneAssets = AssetDatabase.FindAssets("t:Scene", new[] { SharedConfig.WORLD_SCENES_FOLDER });
            foreach (string sceneAsset in sceneAssets)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneAsset);
                scenesToBuild.Add(new EditorBuildSettingsScene(scenePath, true));
            }

            //Apply settings
            EditorBuildSettings.scenes = scenesToBuild.ToArray();
        }
    }
}
