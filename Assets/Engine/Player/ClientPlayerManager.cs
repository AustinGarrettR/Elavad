using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine.Factory;
using System.Threading.Tasks;
using Engine.Asset;


namespace Engine.Player
{
    /// <summary>
    /// Manages the client player
    /// </summary>
    public class ClientPlayerManager : Manager
    {

        /*
         * Constructor
         */

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="playerPrefab">The player prefab</param>
        public ClientPlayerManager(GameObject playerPrefab)
        {
            this.playerPrefab = playerPrefab;
        }

        /*
         * Override Methods
         */

        /// <summary>
        /// Called on initialization
        /// </summary>
        /// <param name="parameters">Asset manager reference</param>
        public override void Init()
        {
           
        }

        /// <summary>
        /// Called on game load
        /// </summary>
        /// <returns></returns>
        public override async Task LoadGameTask()
        {
            await CreateMyPlayerTask();
            gameLoaded = true;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        public override void Process()
        {
            if(gameLoaded)
            {

            }
        }

        /// <summary>
        /// Called on shutdown
        /// </summary>
        public override void Shutdown()
        {
            
        }

        /*
         * Public Functions
         */

        public ClientPlayer GetMyPlayer()
        {
            return myPlayer;
        }

        /*
         * Internal Variables
         */

        /// <summary>
        /// The prefab for the player
        /// </summary>
        private GameObject playerPrefab;

        /// <summary>
        /// Whether the game is loaded or not
        /// </summary>
        private bool gameLoaded;

        /// <summary>
        /// The player object
        /// </summary>
        private ClientPlayer myPlayer;

        /*
         * Internal Functions
         */

        /// <summary>
        /// Async task to trigger create player on game load
        /// </summary>
        /// <returns></returns>
        private async Task CreateMyPlayerTask()
        {
            //Create instance for my player
            myPlayer = new ClientPlayer("_MyPlayer", playerPrefab, new Vector3(125, 0, 125), Vector3.zero);

            await Task.CompletedTask;
        }

    }
}