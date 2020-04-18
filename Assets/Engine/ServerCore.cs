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

        internal override void init()
        {
            addManager(logManager);
            addManager(connectionManager, ConnectionManager.ListenerType.SERVER);
            addManager(serverLoginManager, connectionManager);
            addManager(dispatchManager);
        }

        internal override void update()
        {
            updateManagers();
        }

        internal override void shutdown()
        {
            shutdownManagers();
        }

        /*
         * Internal Variables
         */

        public readonly LogManager logManager = new LogManager();
        public readonly ConnectionManager connectionManager = new ConnectionManager();
        public readonly ServerLoginManager serverLoginManager = new ServerLoginManager();
        public readonly DispatchManager dispatchManager = new DispatchManager();

        /*
         * Internal Functions
         */

    }
}