using Engine.Factory;
using System.Collections.Generic;

namespace Engine.API
{
    /// <summary>
    /// Used to supply references to the API class.
    /// </summary>
    public class APIManager : Manager
    {

        /*
         * Override Functions
         */

        /// <summary>
        /// Manager startup function
        /// </summary>
        /// <param name="parameters">List of managers needs to be supplied</param>
        public override void init(params object[] parameters)
        {
            List<Manager> managers = (List<Manager>) parameters[0];
            API.SetManagers(managers);
        }

        /// <summary>
        /// Called on shutdown
        /// </summary>
        public override void shutdown()
        {
           
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        public override void update()
        {
            
        }
    }

}