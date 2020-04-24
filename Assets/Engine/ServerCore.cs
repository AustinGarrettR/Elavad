using Engine.Account;
using Engine.Networking;
using Engine.Logging;
using Engine.Dispatch;
using Engine.World;
using Engine.Player;
using Engine.Asset;

namespace Engine
{
    /// <summary>
    /// Core monobehavior for server
    /// </summary>
    public class ServerCore : GlobalCoreBase
    {

        /*
         * Override Methods
         */

        /// <summary>
        /// Called on initialization
        /// </summary>
        internal override void Init()
        {
            //Must be executed in order
            logManager = new LogManager();
            serverWorldManager = new ServerWorldManager(OnWorldLoaded);

            AddManager(logManager);
            AddManager(serverWorldManager);

        }

        /// <summary>
        /// Called every frame
        /// </summary>
        internal override void Process()
        {
            UpdateManagers();
        }

        /// <summary>
        /// Called on program shutdown
        /// </summary>
        internal override void Shutdown()
        {
            ShutdownManagers();
        }

        /*
         * Internal Functions
         */

        /// <summary>
        /// Called when the world is finished loading
        /// </summary>
        private void OnWorldLoaded()
        {
            //Must be executed in order
            serverAssetManager = new ServerAssetManager();
            connectionManager = new ConnectionManager(ConnectionManager.ListenerType.SERVER);
            serverLoginManager = new ServerLoginManager(connectionManager);
            serverPlayerManager = new ServerPlayerManager(serverLoginManager, serverAssetManager, connectionManager);
            dispatchManager = new DispatchManager();

            AddManager(serverAssetManager);
            AddManager(serverLoginManager);
            AddManager(serverPlayerManager);
            AddManager(dispatchManager);

            //Run connection manager at the end to open server
            AddManager(connectionManager);
        }

        /*
         * Internal Variables
         */

        /// <summary>
        /// The log manager which handles errors and messages
        /// </summary>
        internal LogManager logManager;

        /// <summary>
        /// The connection manager which handles connections to the clients
        /// </summary>
        internal ConnectionManager connectionManager;

        /// <summary>
        /// The server login manager which handles incoming connections from clients to be logged in
        /// </summary>
        internal ServerLoginManager serverLoginManager;

        /// <summary>
        /// The dispatcher manager which allows asynchronous contexts to execute functions on the main thread
        /// </summary>
        internal DispatchManager dispatchManager;

        /// <summary>
        /// The server world manager loads the world
        /// </summary>
        internal ServerWorldManager serverWorldManager;

        /// <summary>
        /// The server player manager manages player instances
        /// </summary>
        internal ServerPlayerManager serverPlayerManager;

        /// <summary>
        /// The server asset manager manages asset loading
        /// </summary>
        internal ServerAssetManager serverAssetManager;
    }
}