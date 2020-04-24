using UnityEngine;
using System;
using Engine.Logging;
using Engine.Factory;

namespace Engine.Asset
{
    /// <summary>
    /// The manager that handles asset loading
    /// </summary>
    public class ServerAssetManager : Manager
    {
        /*
         * Override Methods
         */

        /// <summary>
        /// Initialize method
        /// </summary>
        public override void Init()
        {
            //Load data pack from resources
            serverAssetPack = Resources.Load<ServerAssetPack>("Server_Assets");

            if(serverAssetPack == null)
            {
                Log.LogError("Error loading server asset pack! Pack loaded is null.");
            }
        }

        /// <summary>
        /// Called on program shutdown
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
         * Public Functions
         */

        /// <summary>
        /// Returns the server player prefab
        /// </summary>
        /// <returns>The player prefab</returns>
        public GameObject GetPlayerPrefab()
        {
            return serverAssetPack.serverPlayerPack.playerPrefab;
        }
        
        /*
         * Internal Variables
         */

        private ServerAssetPack serverAssetPack;

        /*
         * Internal Functions
         */


    }
}
