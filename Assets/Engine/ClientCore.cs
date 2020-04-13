using System;
using System.Collections;
using Engine.Networking;
using Engine.Account;
using Engine.Input;
using Engine.UI;
using Engine.Asset;
using Engine.Logging;

namespace Engine
{
    //Attach to game object in scene. 
    public class ClientCore : GlobalCoreBase
    {

        /*
         * Override Methods
         */

        internal override void init()
        {
            if(clientDataPack == null)
            {
                throw new Exception("Missing client data pack.");
            }

            addManager(logManager);
            addManager(clientAssetManager, clientDataPack);
            addManager(connectionManager, ConnectionManager.ListenerType.CLIENT);
            addManager(clientLoginManager, this, connectionManager);
            addManager(clientUIManager, clientAssetManager);
            addManager(clientInputManager);

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
         * Public Variables
         */

        public ClientDataPack clientDataPack;

        /*
         * Internal Variables
         */

        public readonly LogManager logManager = new LogManager();
        public readonly ClientAssetManager clientAssetManager = new ClientAssetManager();
        public readonly ConnectionManager connectionManager = new ConnectionManager();
        public readonly ClientLoginManager clientLoginManager = new ClientLoginManager();
        public readonly ClientUIManager clientUIManager = new ClientUIManager();
        public readonly ClientInputManager clientInputManager = new ClientInputManager();

        /*
         * Internal Functions
         */

        //Async load method on login. Start main game managers here
        internal IEnumerator loadGameOnLogin(Action onComplete)
        {

            //Done loading        
            return null;
        }
    }
}