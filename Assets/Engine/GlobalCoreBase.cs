﻿using System.Collections.Generic;
using UnityEngine;
using System;

namespace Engine
{
    public abstract class GlobalCoreBase : MonoBehaviour
    {

        /*
         * Overrides
         */
        internal abstract void init();
        internal abstract void update();
        internal abstract void shutdown();

        /*
         * Mono Behavior Inherits
         */

        private void Start()
        {
            //Assign static variable
            if (this is ClientCore)
                clientManagerObject = (ClientCore)this;
            else
                serverManagerObject = (ServerCore)this;
            //Call abstract start method
            init();
        }

        private void Update()
        {
            //Call abstract method for derived class
            update();
        }

        private void OnApplicationQuit()
        {
            //Call abstract method for derived class
            shutdown();
        }

        /*
        * Internal Variables
        */

        internal List<Manager> managers = new List<Manager>();

        /*
         * Internal Functions
         */

        //Instantiate manager of type
        internal void addManager(Manager m, params System.Object[] parameters)
        {
            //Init manager
            m.init(parameters);

            //Add to list for updating
            managers.Add(m);

        }

        //Iterate managers and update them
        internal void updateManagers()
        {
            foreach (Manager m in managers)
            {
                m.update();
            }
        }

        //Iterate managers and inform of shutdown
        internal void shutdownManagers()
        {
            foreach (Manager m in managers)
            {
                m.shutdown();
            }
        }

        /*
         * Static Access
         */

        private static ClientCore clientManagerObject;
        private static ServerCore serverManagerObject;

        //Get manager in client form
        public static ClientCore GetClientManager()
        {
            return (ClientCore)clientManagerObject;
        }

        //Get manager in server form
        public static ServerCore GetServerManager()
        {
            return (ServerCore)serverManagerObject;
        }


    }
}