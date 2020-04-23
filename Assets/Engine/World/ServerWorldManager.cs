using System.Collections.Generic;
using UnityEngine;
using Engine.Configuration;
using Engine.Factory;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Engine.Logging;
using System;
using System.IO;
using UnityEngine.AI;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Engine.World
{
    /// <summary>
    /// Handles loading and managing the game world for the server
    /// </summary>
    public class ServerWorldManager : Manager
    {

        /*
         * Constructor
         */

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="worldLoadedCallback">The callback action called when the world is finished loading</param>
        public ServerWorldManager(Action worldLoadedCallback)
        {
            this.worldLoadedCallback = worldLoadedCallback;
        }

        /*
         * Override Methods
         */

        /// <summary>
        /// Called on manager initialization
        /// </summary>
        public override void Init()
        {
            LoadWorld().ConfigureAwait(true);
        }

        /// <summary>
        /// Called on shutdown
        /// </summary>
        public override void Shutdown()
        {

        }

        /// <summary>
        /// Called every frame
        /// </summary>
        public override void Process()
        {
           
        }


        /*
         * Internal Variables
         */

        /// <summary>
        /// Called when the world is finished loading
        /// </summary>
        private Action worldLoadedCallback;

        /// <summary>
        /// The list of chunks
        /// </summary>
        private List<Vector2Int> chunks = new List<Vector2Int>();
             

        /*
         * Internal Methods
         */              

        /// <summary>
        /// Load the game world
        /// </summary>
        private async Task LoadWorld()
        {
            //Iterate scenes
            for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);

                //Only support world scenes, skip Main and Login scenes
                if (scenePath.Equals(SharedConfig.MAIN_SCENE_PATH) || scenePath.Equals(SharedConfig.LOGIN_SCENE_PATH))
                    continue;

                try
                {
                    //Get chunk from scene name
                    Vector2Int chunk = new Vector2Int(Int32.Parse(sceneName.Split(',')[0]), Int32.Parse(sceneName.Split(',')[1]));

                    //Load
                    await LoadChunk(chunk);

                } catch (Exception e)
                {
                    Log.LogError("Unable to load world scene: " + sceneName);
                    Log.LogError(e.ToString());
                }

            }

            //Finished, do callback
            worldLoadedCallback();

            await Task.CompletedTask;
        }

        /// <summary>
        /// Load chunk scene
        /// </summary>
        /// <param name="chunk">The chunk to load</param>
        private async Task LoadChunk(Vector2Int chunk)
        {
            //Load scene
            AsyncOperation sceneOp = SceneManager.LoadSceneAsync(chunk.x + "," + chunk.y, LoadSceneMode.Additive);

            while (sceneOp.isDone == false)
                await Task.Delay(1);

            chunks.Add(chunk);

            await Task.CompletedTask;

        }

        private void BuildNavmeshSurface(Vector2Int chunk)
        {
            Scene scene = SceneManager.GetSceneByName(chunk.x + "," + chunk.y);
            GameObject[] rootObjects = scene.GetRootGameObjects();

            if(rootObjects.Length != 1)
            {
                Log.LogError("Scene " + scene.name + " has more than one root object.");
                return;
            }

            GameObject root = rootObjects[0];
        }

        /*
         * Utility Methods
         */

        /// <summary>
        /// Returns the chunk location for a position
        /// </summary>
        /// <param name="position">Target position</param>
        /// <returns></returns>
        private Vector2Int getChunkForPosition(Vector2 position)
        {
            int x = (int) position.x;
            int y = (int) position.y;

            return new Vector2Int(Mathf.FloorToInt(x / SharedConfig.WORLD_CHUNK_SIZE), Mathf.FloorToInt(y / SharedConfig.WORLD_CHUNK_SIZE));
        }

        /// <summary>
        /// Overload method for getChunkPosition
        /// </summary>
        /// <param name="position">Target position</param>
        /// <returns></returns>
        private Vector2Int getChunkForPosition(Vector3 position)
        {
            return getChunkForPosition(new Vector2(position.x, position.z));
        }

        /// <summary>
        /// Returns the scene name for a chunk vector
        /// </summary>
        /// <param name="chunk">The chunk</param>
        /// <returns></returns>
        private string getSceneNameForChunk(Vector2Int chunk)
        {
            return chunk.x + "," + chunk.y;
        }
    }
}
