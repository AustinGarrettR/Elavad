using Engine.Account;
using Engine.Networking;
using Engine.Logging;
using Engine.Dispatch;

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
            AddManager(logManager);
            AddManager(connectionManager, ConnectionManager.ListenerType.SERVER);
            AddManager(serverLoginManager, connectionManager);
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
        /// Called on program shutdown
        /// </summary>
        internal override void Shutdown()
        {
            ShutdownManagers();
        }

        /*
         * Internal Variables
         */

        /// <summary>
        /// The log manager which handles errors and messages
        /// </summary>
        public readonly LogManager logManager = new LogManager();

        /// <summary>
        /// The connection manager which handles connections to the clients
        /// </summary>
        public readonly ConnectionManager connectionManager = new ConnectionManager();

        /// <summary>
        /// The server login manager which handles incoming connections from clients to be logged in
        /// </summary>
        public readonly ServerLoginManager serverLoginManager = new ServerLoginManager();

        /// <summary>
        /// The dispatcher manager which allows asynchronous contexts to execute functions on the main thread
        /// </summary>
        public readonly DispatchManager dispatchManager = new DispatchManager();

    }
}