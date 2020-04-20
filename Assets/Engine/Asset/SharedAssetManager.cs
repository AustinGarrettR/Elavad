using Engine.Factory;
using UnityEngine;
using Engine.Logging;

namespace Engine.Asset
{
    /// <summary>
    /// Handles shared asset loading
    /// </summary>
    public class SharedAssetManager : Manager
    {

        /*
         * Override Methods
         */

        /// <summary>
        /// Called on initialize
        /// </summary>
        /// <param name="parameters">Variable parameters</param>
        public override void Init(params object[] parameters)
        {
            //Load data pack from resources
            sharedAssetPack = Resources.Load<SharedAssetPack>("Shared_Assets");

            if (sharedAssetPack == null)
            {
                Log.LogError("Error loading shared asset pack! Pack loaded is null.");
            }
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
        * Public Functions
        */


        /*
         * Internal Variables
         */

        private SharedAssetPack sharedAssetPack;
    }
}