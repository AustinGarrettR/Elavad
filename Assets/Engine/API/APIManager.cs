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
         * Constructor
         */

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="managers">The list of managers for the api to reference</param>
        public APIManager(List<Manager> managers)
        {
            API.SetManagers(managers);
        }

        /*
         * Override Functions
         */

        /// <summary>
        /// Manager startup function
        /// </summary>
        /// <param name="parameters">List of managers needs to be supplied</param>
        public override void Init()
        {

        }

        /// <summary>
        /// Called on shutdown
        /// </summary>
        public override void Shutdown()
        {
           
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        public override void Process()
        {
            
        }
    }

}