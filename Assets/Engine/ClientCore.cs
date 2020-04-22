using Engine.Networking;
using Engine.Account;
using Engine.Input;
using Engine.UI;
using Engine.Asset;
using Engine.Logging;
using Engine.API;
using Engine.Dispatch;
using Engine.World;

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

            AddManager(logManager);
            AddManager(clientAssetManager);
            AddManager(connectionManager, ConnectionManager.ListenerType.CLIENT);
            AddManager(clientWorldManager);
            AddManager(clientLoginManager, connectionManager, managers);
            AddManager(clientInputManager);
            AddManager(clientUIManager, clientAssetManager);
            AddManager(apiManager, managers);
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
        public readonly LogManager logManager = new LogManager();

        /// <summary>
        /// The client asset manager which handles asset loading
        /// </summary>
        public readonly ClientAssetManager clientAssetManager = new ClientAssetManager();

        /// <summary>
        /// The connection manager which handles connections to the server
        /// </summary>
        public readonly ConnectionManager connectionManager = new ConnectionManager();

        /// <summary>
        /// The client world manager which handles loading and running the game world
        /// </summary>
        public readonly ClientWorldManager clientWorldManager = new ClientWorldManager();

        /// <summary>
        /// The client login manager for handling logins to the server
        /// </summary>
        public readonly ClientLoginManager clientLoginManager = new ClientLoginManager();

        /// <summary>
        /// The client ui manager for handling UI panels
        /// </summary>
        public readonly ClientUIManager clientUIManager = new ClientUIManager();

        /// <summary>
        /// The client input manager for handling user input
        /// </summary>
        public readonly ClientInputManager clientInputManager = new ClientInputManager();

        /// <summary>
        /// The api manager which gives references to the API class for the content namespace to access
        /// </summary>
        public readonly APIManager apiManager = new APIManager();

        /// <summary>
        /// The dispatcher which allows asynchronous contexts to execute functions on the main thread
        /// </summary>
        public readonly DispatchManager dispatchManager = new DispatchManager();

    }
}