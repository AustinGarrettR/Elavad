using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Engine.Asset;

namespace Engine.UI
{
    public class ClientUIManager : Manager
    {
        /*
         * Override Methods
         */

        //Initialize method
        internal override void init(params object[] parameters)
        {
            //Assign asset manager
            clientAssetManager = (ClientAssetManager) parameters[0];

            //Remove later, test only!
            GameObject.Instantiate(clientAssetManager.GetUIPanel("LoginScreen"));
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
         * Internal Variables
         */

        private ClientAssetManager clientAssetManager;
        private List<UIController> registeredControllers = new List<UIController>();

        /*
         * Internal Functions
         */

        private void RegisterUIController(UIController controller)
        {
            if (registeredControllers.Contains(controller) == false)
                registeredControllers.Add(controller);
        }

        private void UnregisterUIController(UIController controller)
        {
            if (registeredControllers.Contains(controller))
                registeredControllers.Remove(controller);
        }

    }
}
