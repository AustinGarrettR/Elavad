using System.Collections.Generic;
using UnityEngine;
using Engine.Configuration;
using Engine.Factory;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Engine.Logging;
using Engine.Player;

namespace Engine.World
{
    /// <summary>
    /// Handles loading and managing the game world for the client
    /// </summary>
    public class ClientWorldManager : Manager
    {
        /*
         * Constructor
         */

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientPlayerManager">The client player manager reference</param>
        public ClientWorldManager(ClientPlayerManager clientPlayerManager)
        {
            //Assign client player manager
            this.clientPlayerManager = clientPlayerManager;
        }

        /*
         * Override Methods
         */

        /// <summary>
        /// Called on manager initialization
        /// </summary>
        public override void Init()
        {

            //Create environment system
            environmentSystem = new EnvironmentSystem();

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
            if (gameLoaded)
            {
                ProcessLoaded();
            }
        }

        /// <summary>
        /// Called on game load
        /// </summary>
        /// <returns></returns>
        public override async Task LoadGameTask()
        {
            //Set focus object
            focusTarget = clientPlayerManager.GetMyPlayer().GetPlayerObject();

            //Set focus to the target
            environmentSystem.SetFocusTarget(focusTarget);

            //Process
            ProcessLoaded();

            //Ensure the world is loaded before continuing
            await WaitForAllChunksToFinishLoad();

            //Done loading this class
            gameLoaded = true;

            //Task completed
            await Task.CompletedTask;
        }

        /*
         * Internal Variables
         */

        /// <summary>
        /// The target focus for chunk loading and environment switching
        /// </summary>
        private GameObject focusTarget;

        /// <summary>
        /// The client player manager reference
        /// </summary>
        private ClientPlayerManager clientPlayerManager;

        /// <summary>
        /// Whether or not the game is loaded
        /// </summary>
        private bool gameLoaded = false;

        /// <summary>
        /// The list of chunks that are to be loaded
        /// </summary>
        private List<Vector2Int> chunks = new List<Vector2Int>();

        /// <summary>
        /// The list of chunks that are being loaded
        /// </summary>
        private List<Vector2Int> chunksInOperation = new List<Vector2Int>();

        /// <summary>
        /// The list of chunks that are loaded
        /// </summary>
        private List<Vector2Int> chunksLoaded = new List<Vector2Int>();

        /// <summary>
        /// The environment system instance
        /// </summary>
        private EnvironmentSystem environmentSystem;

        /*
         * Internal Methods
         */

        /// <summary>
        /// Waits for all chunks to finish loading
        /// </summary>
        /// <returns></returns>
        private async Task WaitForAllChunksToFinishLoad()
        {
            while (chunksInOperation.Count > 0)
                await Task.Delay(1);
        }

        /// <summary>
        /// Runs every frame when game is loaded
        /// </summary>
        private void ProcessLoaded()
        {
            UpdateChunks(focusTarget.transform.position);
            UpdateScenes();

            environmentSystem.Update();
        }

        /// <summary>
        /// Update chunks based around a target position
        /// </summary>
        /// <param name="focusPosition">Target position</param>
        private void UpdateChunks(Vector2 focusPosition)
        {
            //Figure out how far we need to go looking for chunks
            int maximumRelevantDistance = (SharedConfig.CHUNK_LOAD_VIEW_DISTANCE > SharedConfig.CHUNK_UNLOAD_VIEW_DISTANCE 
                ? SharedConfig.CHUNK_LOAD_VIEW_DISTANCE : SharedConfig.CHUNK_UNLOAD_VIEW_DISTANCE);

            int iterations = Mathf.CeilToInt(maximumRelevantDistance / SharedConfig.WORLD_CHUNK_SIZE);

            //Iterate over nearby chunk spots
            for (int x = -iterations; x < iterations; x++)
            {
                for (int y = -iterations; y < iterations; y++)
                {
                    int xPos = (int)focusPosition.x + (x * SharedConfig.WORLD_CHUNK_SIZE);
                    int yPos = (int)focusPosition.y + (y * SharedConfig.WORLD_CHUNK_SIZE);

                    //Get chunk positions
                    Vector2Int chunk = getChunkForPosition(new Vector2(xPos, yPos));

                    CheckChunk(focusPosition, chunk);
                }
            }

            //Iterate over current chunks as well
            foreach (Vector2Int chunk in chunks)
            {
                CheckChunk(focusPosition, chunk);
            }
        }

        /// <summary>
        /// Check the chunk for add/removal from the list
        /// </summary>
        /// <param name="chunk">The chunk</param>
        /// <param name="focusPosition">The target position</param>
        private void CheckChunk(Vector2 focusPosition, Vector2Int chunk)
        {
            int chunkX = chunk.x * SharedConfig.WORLD_CHUNK_SIZE;
            int chunkY = chunk.y * SharedConfig.WORLD_CHUNK_SIZE;

            Vector2Int chunkLocationCenter = new Vector2Int(chunkX + (SharedConfig.WORLD_CHUNK_SIZE / 2), chunkY + (SharedConfig.WORLD_CHUNK_SIZE / 2));

            //Check distance
            float distance = Vector2.Distance(focusPosition, chunkLocationCenter);

            if (distance < SharedConfig.CHUNK_LOAD_VIEW_DISTANCE)
            {
                //Chunk is close enough to add.
                //Now check if we already have it

                if (chunks.Contains(chunk) == false)
                {
                    //Add it!
                    AddChunk(chunk);
                }
            }
            else if (distance > SharedConfig.CHUNK_UNLOAD_VIEW_DISTANCE)
            {
                //Chunk is far enough to remove
                //Now check if we have the chunk and remove it

                if (chunks.Contains(chunk))
                {
                    RemoveChunk(chunk);
                }

            }
        }

        /// <summary>
        /// Overload method for UpdateChunks
        /// </summary>
        /// <param name="focusPosition">Target position</param>
        private void UpdateChunks(Vector3 focusPosition)
        {
            UpdateChunks(new Vector2(focusPosition.x, focusPosition.z));
        }

        /// <summary>
        /// Add a chunk to the chunk list
        /// </summary>
        /// <param name="chunk">The chunk</param>
        private void AddChunk(Vector2Int chunk)
        {
            chunks.Add(chunk);
        }

        /// <summary>
        /// Remove a chunk from the chunk list
        /// </summary>
        /// <param name="chunk">The chunk</param>
        private void RemoveChunk(Vector2Int chunk)
        {
            chunks.Remove(chunk);
        }

        /// <summary>
        /// Load and unload chunks based on desired chunk list
        /// </summary>
        private void UpdateScenes()
        {
            //Iterate over our desired chunks list
            foreach(Vector2Int chunk in chunks)
            {
                string sceneName = getSceneNameForChunk(chunk);

                //Dont load a chunk if already loaded or doesn't exist
                if (Application.CanStreamedLevelBeLoaded(sceneName) == false || SceneManager.GetSceneByName(sceneName).isLoaded)
                    continue;

                //Dont load a chunk if it's mid load/unload
                if (chunksInOperation.Contains(chunk))
                    continue;

                //Load chunk
                chunksInOperation.Add(chunk);
                AsyncOperation sceneOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                Log.LogMsg("Loading chunk: " + chunk);

                sceneOp.completed += delegate (AsyncOperation op) { 
                    ChunkLoaded(chunk, sceneName); 
                };
            }

            //Iterate over loaded chunks
            foreach(Vector2Int chunk in chunksLoaded)
            {
                //Time to unload
                if(chunks.Contains(chunk) == false)
                {
                    string sceneName = getSceneNameForChunk(chunk);
                    if (SceneManager.GetSceneByName(sceneName).isLoaded == false)
                    {
                        continue;
                    }

                    //Chunk is mid load/unload
                    if (chunksInOperation.Contains(chunk))
                        continue;

                    //Unload chunk
                    chunksInOperation.Add(chunk);
                    AsyncOperation sceneOp = SceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

                    sceneOp.completed += delegate (AsyncOperation op) {
                        ChunkUnloaded(chunk);
                    };
                }
            }
        }

        /// <summary>
        /// Called when the chunk is finished loading
        /// </summary>
        /// <param name="chunk">The chunk</param>
        private void ChunkLoaded(Vector2Int chunk, string sceneName)
        {
            if (chunksInOperation.Contains(chunk))
                chunksInOperation.Remove(chunk);

            chunksLoaded.Add(chunk);

            //Grab scene by name
            Scene scene = SceneManager.GetSceneByName(sceneName);

            //Check root objects
            GameObject[] rootObjects = scene.GetRootGameObjects();

            if (rootObjects.Length == 0)
            {
                Log.LogError("Scene " + sceneName + " contains no root object.");
            }

            //Position root
            GameObject root = rootObjects[0];
            root.transform.position = new Vector3(chunk.x * SharedConfig.WORLD_CHUNK_SIZE, 0, chunk.y * SharedConfig.WORLD_CHUNK_SIZE);

            //Call environment system to scan for environment volumes
            environmentSystem.OnChunkSceneLoaded(scene);
        }

        /// <summary>
        /// Called when the chunk is finished unloading
        /// </summary>
        /// <param name="chunk">The chunk</param>
        private void ChunkUnloaded(Vector2Int chunk)
        {
            if (chunksInOperation.Contains(chunk))
                chunksInOperation.Remove(chunk);

            chunksLoaded.Remove(chunk);
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
