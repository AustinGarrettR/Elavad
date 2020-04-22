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
using UnityEditor.PackageManager;

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
            //Must be executed in the desired order
            //process will be called

            logManager = new LogManager();
            AddManager(logManager);

            clientAssetManager = new ClientAssetManager();
            AddManager(clientAssetManager);

            connectionManager = new ConnectionManager(ConnectionManager.ListenerType.CLIENT);
            AddManager(connectionManager);

            clientPlayerManager = new ClientPlayerManager(clientAssetManager.GetPlayerPrefab());
            AddManager(clientPlayerManager);

            clientWorldManager = new ClientWorldManager(clientPlayerManager);
            AddManager(clientWorldManager);

            clientInputManager = new ClientInputManager();
            AddManager(clientInputManager);

            clientUIManager = new ClientUIManager(clientAssetManager);
            AddManager(clientUIManager);

            clientLoginManager = new ClientLoginManager(connectionManager, managers);
            AddManager(clientLoginManager);

            apiManager = new APIManager(managers);
            AddManager(apiManager);

            dispatchManager = new DispatchManager();
            AddManager(dispatchManager);

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

    }
}