using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Factory;
using Engine.Logging;

namespace Engine.API
{
    /// <summary>
    /// API to interface with the engine
    /// </summary>
    public static partial class API
    {

        internal static Hashtable managers = new Hashtable();

        internal static void SetManagers(List<Manager> listedManagers)
        {
            foreach (Manager manager in listedManagers)
            {
                managers.Add(manager.GetType(), manager);
            }
        }

        private static T GetManager<T>() where T : Manager
        {
            Type typeParameter = typeof(T);
            if (managers.ContainsKey(typeParameter))
            {
                return (T)managers[typeParameter];
            }
            else
            {
                Log.LogError("API Called for manager type '" + (typeParameter.Name) + "' which is not a valid manager.");
                return null;
            }
        }

        /// <summary>
        /// The client section of the API
        /// </summary>
        public static partial class Client { }

        /// <summary>
        /// The server section of the API
        /// </summary>
        public static partial class Server { }

    }
}