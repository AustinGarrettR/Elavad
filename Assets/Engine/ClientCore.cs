using Engine.Networking;
using Engine.Account;
using Engine.Input;
using Engine.UI;
using Engine.Asset;
using Engine.Logging;
using Engine.API;
using Engine.Dispatch;
using Engine.World;
using Engine.Player;
using Engine.CameraSystem;

namespace Engine
{
    /// <summary>
    /// Core monobehavior for client
    /// </summary>
    public class ClientCore : GlobalCoreBase
    {

        /*
         * Override Methods
         */

        /// <summary>
        /// Called on initialization
        /// </summary>
        internal override void Init()
        {
            //Must be executed in the order

            logManager = new LogManager();
            clientAssetManager = new ClientAssetManager();
            connectionManager = new ConnectionManager(ConnectionManager.ListenerType.CLIENT);
            clientPlayerManager = new ClientPlayerManager(clientAssetManager, connectionManager);
            clientWorldManager = new ClientWorldManager(clientPlayerManager);
            clientInputManager = new ClientInputManager();
            clientUIManager = new ClientUIManager(clientAssetManager);
            clientCameraManager = new ClientCameraManager(clientPlayerManager);
            clientLoginManager = new ClientLoginManager(connectionManager, managers);
            dispatchManager = new DispatchManager();

            AddManager(logManager);
            AddManager(clientAssetManager);
            AddManager(connectionManager);
            AddManager(clientPlayerManager);
            AddManager(clientWorldManager);
            AddManager(clientInputManager);
            AddManager(clientUIManager);
            AddManager(clientCameraManager);
            AddManager(clientLoginManager);
            AddManager(dispatchManager);


            //API needs managers reference, so construct after managers have been added
            apiManager = new APIManager(managers);
            AddManager(apiManager);

        }

        /// <summary>
        /// Called every frame
        /// </summary>
        internal override void Process()
        {
            UpdateManagers();
        }

        /// <summary>
        /// Called on shutdown
        /// </summary>
        internal override void Shutdown()
        {
            ShutdownManagers();
        }


        /*
         * Internal Variables
         */

        /// <summary>
        /// The log manager for logging errors and messages
        /// </summary>
        internal LogManager logManager;

        /// <summary>
        /// The client asset manager which handles asset loading
        /// </summary>
        internal ClientAssetManager clientAssetManager;

        /// <summary>
        /// The connection manager which handles connections to the server
        /// </summary>
        internal ConnectionManager connectionManager;

        /// <summary>
        /// The client player manager for handling client-sided player instances
        /// </summary>
        internal ClientPlayerManager clientPlayerManager;

        /// <summary>
        /// The client world manager which handles loading and running the game world
        /// </summary>
        internal ClientWorldManager clientWorldManager;

        /// <summary>
        /// The client login manager for handling logins to the server
        /// </summary>
        internal ClientLoginManager clientLoginManager;

        /// <summary>
        /// The client ui manager for handling UI panels
        /// </summary>
        internal ClientUIManager clientUIManager;

        /// <summary>
        /// The client input manager for handling user input
        /// </summary>
        internal ClientInputManager clientInputManager;

        /// <summary>
        /// The api manager which gives references to the API class for the content namespace to access
        /// </summary>
        internal APIManager apiManager;

        /// <summary>
        /// The dispatcher which allows asynchronous contexts to execute functions on the main thread
        /// </summary>
        internal DispatchManager dispatchManager;

        /// <summary>
        /// The client camera manager processes the camera system
        /// </summary>
        internal ClientCameraManager clientCameraManager;

    }
}