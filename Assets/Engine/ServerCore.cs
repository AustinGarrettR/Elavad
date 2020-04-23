﻿using Engine.Account;
using Engine.Networking;
using Engine.Logging;
using Engine.Dispatch;
using Engine.World;
using Engine.Player;

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
            logManager = new LogManager();
            AddManager(logManager);

            serverWorldManager = new ServerWorldManager(OnWorldLoaded);
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
            serverPlayerManager = new ServerPlayerManager();
            AddManager(serverPlayerManager);

            connectionManager = new ConnectionManager(ConnectionManager.ListenerType.SERVER);
            AddManager(connectionManager);

            serverLoginManager = new ServerLoginManager(connectionManager);
            AddManager(serverLoginManager);

            dispatchManager = new DispatchManager();
            AddManager(dispatchManager);
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

    }
}