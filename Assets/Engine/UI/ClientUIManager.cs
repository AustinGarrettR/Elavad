using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Engine.Asset;
using Engine.Input;
using UnityEngine.InputSystem.LowLevel;
using Unity.UIElements.Runtime;
using UnityEngine.UIElements;

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

            //Assign Input Manager
            clientInputManager = (ClientInputManager) parameters[1];

            //Remove later, test only!
            GameObject login = GameObject.Instantiate(clientAssetManager.GetUIPanel("LoginScreen"));
            RegisterUIController(login.GetComponent<UIController>());


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
        private ClientInputManager clientInputManager;
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
