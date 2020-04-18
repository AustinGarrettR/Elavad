using System;
using System.Collections;
using Engine.Networking;
using Engine.Account;
using Engine.Input;
using Engine.UI;
using Engine.Asset;
using Engine.Logging;
using Engine.API;
using Engine.Dispatch;

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

        internal override void init()
        {

            addManager(logManager);
            addManager(clientAssetManager);
            addManager(connectionManager, ConnectionManager.ListenerType.CLIENT);
            addManager(clientLoginManager, this, connectionManager);
            addManager(clientInputManager);
            addManager(clientUIManager, clientAssetManager);
            addManager(apiManager, managers);
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
        public readonly ClientAssetManager clientAssetManager = new ClientAssetManager();
        public readonly ConnectionManager connectionManager = new ConnectionManager();
        public readonly ClientLoginManager clientLoginManager = new ClientLoginManager();
        public readonly ClientUIManager clientUIManager = new ClientUIManager();
        public readonly ClientInputManager clientInputManager = new ClientInputManager();
        public readonly APIManager apiManager = new APIManager();
        public readonly DispatchManager dispatchManager = new DispatchManager();

        /*
         * Internal Functions
         */

    }
}