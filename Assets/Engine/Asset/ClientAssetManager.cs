using UnityEngine;
using System;
using Engine.Logging;
using Engine.Factory;

namespace Engine.Asset
{
    /// <summary>
    /// The manager that handles asset loading
    /// </summary>
    public class ClientAssetManager : Manager
    {
        /*
         * Override Methods
         */

        /// <summary>
        /// Initialize method
        /// </summary>
        /// <param name="parameters"></param>
        public override void Init(params object[] parameters)
        {
            //Load data pack from resources
            clientAssetPack = Resources.Load<ClientAssetPack>("Client_Assets");

            if(clientAssetPack == null)
            {
                Log.LogError("Error loading client asset pack! Pack loaded is null.");
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
        /// Return a UI panel from the asset pack
        /// </summary>
        /// <param name="name">The name of the panel</param>
        /// <returns></returns>
        public GameObject GetUIPanel(string name)
        {
            GameObject[] panelObjects = clientAssetPack.uiPack.UI_Panels;
            foreach(GameObject obj in panelObjects)
            {
                if(obj.name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return obj;
                }
            }


            Log.LogError("Asset Error. Unable to find UI Panel:" + name);

            return null;
        }
        
        /*
         * Internal Variables
         */

        private ClientAssetPack clientAssetPack;

        /*
         * Internal Functions
         */


    }
}
