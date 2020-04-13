using Engine.Account;
using Engine.Networking;
using Engine.Logging;

namespace Engine
{
    //Attach to game object in scene. 
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

        /*
         * Internal Functions
         */

    }
}