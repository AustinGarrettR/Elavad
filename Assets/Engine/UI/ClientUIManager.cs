using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
            //Load ui panels from data pack
            ClientDataPack dataPack = (ClientDataPack) parameters[0];
            UI_Panels = dataPack.UI_Panels;

            //Remove later, test only!
            GameObject.Instantiate(UI_Panels[0]);
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

        /*
         * Internal Variables
         */

        private GameObject[] UI_Panels;
        private List<UIController> registeredControllers = new List<UIController>();

        /*
         * Internal Functions
         */

        private void CreateGameObjects()
        {

        }
    }
}
