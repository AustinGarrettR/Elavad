using System.Collections.Generic;
using UnityEngine;
using System;
using Engine.Factory;

namespace Engine
{
    /// <summary>
    /// The base monobehavior for the client and server cores
    /// </summary>
    public abstract class GlobalCoreBase : MonoBehaviour
    {

        /*
         * Overrides
         */

        /// <summary>
        /// Abstract initialization method for inheriting class
        /// </summary>
        internal abstract void Init();

        /// <summary>
        /// Abstract update method for inhereting class
        /// </summary>
        internal abstract void Process();

        /// <summary>
        /// Abstract shutdown method for inherting class
        /// </summary>
        internal abstract void Shutdown();

        /*
         * Mono Behavior Inherits
         */

        /// <summary>
        /// Called by mono behavior on start
        /// </summary>
        internal void Start()
        {
            //Call abstract start method
            Init();
        }

        /// <summary>
        /// Called by mono behavior to update every frame
        /// </summary>
        internal void Update()
        {
            //Call abstract method for derived class
            Process();
        }

        /// <summary>
        /// Called by mono behavior on application quit
        /// </summary>
        internal void OnApplicationQuit()
        {
            //Call abstract method for derived class
            Shutdown();
        }

        /*
        * Internal Variables
        */

        /// <summary>
        /// List of Managers for processing
        /// </summary>
        internal List<Manager> managers = new List<Manager>();

        /*
         * Internal Functions
         */

        /// <summary>
        /// Instantiate manager of type
        /// </summary>
        /// <param name="m"></param>
        /// <param name="parameters"></param>
        internal void AddManager(Manager m, params System.Object[] parameters)
        {
            //Init manager
            m.Init(parameters);

            //Add to list for updating
            managers.Add(m);

        }

        /// <summary>
        /// Iterate managers and update them
        /// </summary>
        internal void UpdateManagers()
        {
            foreach (Manager m in managers)
            {
                m.Process();
            }
        }

        /// <summary>
        /// Iterate managers and inform of shutdown
        /// </summary>
        internal void ShutdownManagers()
        {
            foreach (Manager m in managers)
            {
                m.Shutdown();
            }
        }

    }
}