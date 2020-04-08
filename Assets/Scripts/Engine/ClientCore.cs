using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Engine.Networking;
using Engine.Account;

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

            addManager(connectionManager, ConnectionManager.ListenerType.CLIENT);
            addManager(clientLoginManager, this, connectionManager);

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

        public readonly ConnectionManager connectionManager = new ConnectionManager();
        public readonly ClientLoginManager clientLoginManager = new ClientLoginManager();


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