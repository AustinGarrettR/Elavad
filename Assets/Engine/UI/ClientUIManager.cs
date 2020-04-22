using System.Collections.Generic;
using Engine.Asset;
using Engine.Factory;

namespace Engine.UI
{
    /// <summary>
    /// Manager that handles UI
    /// </summary>
    public class ClientUIManager : Manager
    {
        /*
         * Override Methods
         */

        /// <summary>
        /// Initialize method
        /// </summary>
        /// <param name="parameters">ClientAsetManager input</param>
        public override void Init(params object[] parameters)
        {
            //Assign asset manager
            clientAssetManager = (ClientAssetManager) parameters[0];

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
         * Internal Variables
         */

        /// <summary>
        /// The client asset manager reference
        /// </summary>
        private ClientAssetManager clientAssetManager;

        /// <summary>
        /// Registered UI Panel Controllers
        /// </summary>
        private List<UIController> registeredControllers = new List<UIController>();

        /*
         * Internal Functions
         */

        /// <summary>
        /// Register a UI panel controller
        /// </summary>
        /// <param name="controller">The controller</param>
        private void RegisterUIController(UIController controller)
        {
            if (registeredControllers.Contains(controller) == false)
                registeredControllers.Add(controller);
        }

        /// <summary>
        /// Unregister a UI panel controller
        /// </summary>
        /// <param name="controller">The controller</param>
        private void UnregisterUIController(UIController controller)
        {
            if (registeredControllers.Contains(controller))
                registeredControllers.Remove(controller);
        }

    }
}
