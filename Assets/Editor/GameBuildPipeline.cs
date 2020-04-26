using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using Engine.Configuration;
using System.Collections.Generic;

namespace Editor
{
    public class GameBuildPipeline : MonoBehaviour
    {
        /*
         * Pipeline Options
         */

        [MenuItem("Elavad/Build/Windows64/Build Client")]
        public static void BuildWindows64Client()
        {
            Build(BuildTarget.StandaloneWindows64, true);
        }

        [MenuItem("Elavad/Build/Windows64/Build Server")]
        public static void BuildWindows64Server()
        {
            Build(BuildTarget.StandaloneWindows64, false);
        }

        [MenuItem("Elavad/Build/Windows32/Build Client")]
        public static void BuildWindows32Client()
        {
            Build(BuildTarget.StandaloneWindows, true);
        }

        [MenuItem("Elavad/Build/Windows32/Build Server")]
        public static void BuildWindows32Server()
        {
            Build(BuildTarget.StandaloneWindows, false);
        }

        /*
         * Internal Functions
         */

        /// <summary>
        /// Build based on target
        /// </summary>
        /// <param name="target">The build target</param>
        /// <param name="client">Client or server</param>
        private static void Build(BuildTarget target, bool client)
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            List<string> scenesToBuild = new List<string>();

            if (client)
            {
                //Add Client Scene
                scenesToBuild.Add(SharedConfig.CLIENT_SCENE_PATH);

                //Add Login Scene
                scenesToBuild.Add(SharedConfig.LOGIN_SCENE_PATH);
            }
            else
            {
                //Add Server Scene
                scenesToBuild.Add(SharedConfig.SERVER_SCENE_PATH);

                //Development build for server
                buildPlayerOptions.options = BuildOptions.Development | BuildOptions.BuildScriptsOnly;
            }

            //Add world scenes
            string[] sceneAssets = AssetDatabase.FindAssets("t:Scene", new[] { SharedConfig.WORLD_SCENES_FOLDER });
            foreach (string sceneAsset in sceneAssets)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneAsset);
                scenesToBuild.Add(scenePath);
            }

            buildPlayerOptions.scenes = scenesToBuild.ToArray();
            buildPlayerOptions.locationPathName = "Builds/" + target.ToString() + "/" + (client ? "Client/Angels of Elavad.exe" : "Server/Server.exe");
            buildPlayerOptions.target = target;


            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed.");
            }
        }
    }
}