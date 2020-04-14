using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Engine.Logging;

namespace Engine.Asset
{
    public class ClientAssetManager : Manager
    {
        /*
         * Override Methods
         */

        //Initialize method
        internal override void init(params object[] parameters)
        {
            //Load data pack from resources
            clientDataPack = Resources.Load<ClientAssetPack>("Client_Assets");

            if(clientDataPack == null)
            {
                Log.LogError("Error loading client asset pack! Pack loaded is null.");
            }
        }

        //Called on program shutdown
        internal override void shutdown()
        {

        }

        //Called every frame
        internal override void update()
        {

        }

        /*
         * Public Functions
         */

        internal GameObject GetUIPanel(string name)
        {
            GameObject[] panelObjects = clientDataPack.uiPack.UI_Panels;
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

        private ClientAssetPack clientDataPack;

        /*
         * Internal Functions
         */


    }
}
