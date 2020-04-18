using System.Collections.Generic;
using UnityEngine;
using Engine.Asset;
using Engine.Factory;

namespace Engine.UI
{
    public class ClientUIManager : Manager
    {
        /*
         * Override Methods
         */

        //Initialize method
        public override void init(params object[] parameters)
        {
            //Assign asset manager
            clientAssetManager = (ClientAssetManager) parameters[0];

        }

        //Called on program shutdown
        public override void shutdown()
        {

        }

        //Called every frame
        public override void update()
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
