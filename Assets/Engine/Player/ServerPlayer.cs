using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.AI;

namespace Engine.Player
{
    /// <summary>
    /// This class represents a player instance
    /// </summary>
    public class ServerPlayer
    {
        /*
         * Constructor
         */

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connection">The client network connection</param>
        internal ServerPlayer(NetworkConnection connection)
        {
            this.connection = connection;
        }

        /*
         * Internal
         */

        /// <summary>
        /// The network connection reference
        /// </summary>
        private NetworkConnection connection;

        /// <summary>
        /// The server side player game object
        /// </summary>
        private GameObject playerGameObject;

        /// <summary>
        /// The navmeshagent component reference
        /// </summary>
        private NavMeshAgent navMeshAgent;

        /*
         * Public (Internal)
         */

        /// <summary>
        /// The last time a movement update was received
        /// </summary>
        internal long lastMovementUpdateTimestamp;

        /// <summary>
        /// Getter for the network connection reference
        /// </summary>
        /// <returns></returns>
        internal NetworkConnection getConnection()
        {
            return connection;
        }

        /// <summary>
        /// Set the player game object
        /// </summary>
        /// <param name="playerGameObject">The game object</param>
        internal void SetPlayerGameObject(GameObject playerGameObject)
        {
            this.playerGameObject = playerGameObject;
        }

        /// <summary>
        /// Set the navmeshagent reference
        /// </summary>
        /// <param name="playerGameObject">The navmeshagent component</param>
        internal void SetPlayerAgent(NavMeshAgent agent)
        {
            this.navMeshAgent = agent;
        }

        /*
         * Public
         */

        /// <summary>
        /// Returns the server sided player game object
        /// </summary>
        /// <returns></returns>
        public GameObject GetPlayerGameObject()
        {
            return this.playerGameObject;
        }

        /// <summary>
        /// Returns the navmeshagent component on the game object
        /// </summary>
        /// <returns></returns>
        public NavMeshAgent GetPlayerAgent()
        {
            return this.navMeshAgent;
        }
    }
}