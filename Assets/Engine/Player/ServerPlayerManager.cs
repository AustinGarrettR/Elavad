using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine.Factory;
using UnityEngine.AI;
using Engine.Account;
using Unity.Networking.Transport;
using Engine.Asset;
using Engine.Networking;
using System;

namespace Engine.Player
{
    public class ServerPlayerManager : Manager
    {
        /*
         * Constructor
         */

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverLoginManager">The server login manager reference</param>
        /// <param name="serverAssetManager">The server asset manager reference</param>
        /// <param name="connectionManager">The connection manager reference</param>
        public ServerPlayerManager(ServerLoginManager serverLoginManager, ServerAssetManager serverAssetManager, ConnectionManager connectionManager)
        {
            this.serverLoginManager = serverLoginManager;
            this.serverAssetManager = serverAssetManager;
            this.connectionManager = connectionManager;
        }

        /*
         * Override Functions
         */

        /// <summary>
        /// Called on initialization
        /// </summary>
        public override void Init()
        {
            //Subscribe to events
            serverLoginManager.NotifyClientLoggedInAndLoaded += OnLoginAndLoaded;
            serverLoginManager.NotifyClientLoggedOut += OnLogout;

            connectionManager.NotifyPacketReceived += OnPacketReceived;
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        public override void Process()
        {
            ProcessPlayers();
        }

        /// <summary>
        /// Called on shutdown
        /// </summary>
        public override void Shutdown()
        {

        }

        /*
         * Internal Variables
         */

        /// <summary>
        /// The players hashtable.
        /// </summary>
        private Hashtable players = new Hashtable();

        /// <summary>
        /// Connection manager reference
        /// </summary>
        private ServerLoginManager serverLoginManager;

        /// <summary>
        /// Server asset manager reference
        /// </summary>
        private ServerAssetManager serverAssetManager;

        /// <summary>
        /// The connection manager reference
        /// </summary>
        private ConnectionManager connectionManager;

        /*
         * Event Functions 
         */

        /// <summary>
        /// Called on player successful login
        /// </summary>
        /// <param name="client">The network connection for the client</param>
        private void OnLoginAndLoaded(NetworkConnection client)
        {
            ServerPlayer serverPlayer = new ServerPlayer(client);
            LoadPlayer(serverPlayer);
            players.Add(client, serverPlayer);
        }

        /// <summary>
        /// Called on client logout
        /// </summary>
        /// <param name="client">The network connection for the client</param>
        private void OnLogout(NetworkConnection client)
        {
            UnloadPlayer((ServerPlayer) players[client]);
            players.Remove(client);
        }

        /// <summary>
        /// Called on packet receive
        /// </summary>
        /// <param name="connection">The client connection</param>
        /// <param name="packetId">The packet ID</param>
        /// <param name="rawPacket">The packet contents in bytes</param>
        private void OnPacketReceived(NetworkConnection connection, int packetId, byte[] rawPacket)
        {
            if(packetId == 5)
            {
                WalkRequest_5 packet = new WalkRequest_5();
                packet.readPacket(rawPacket);

                Vector3 targetPosition = new Vector3(packet.x, packet.y, packet.z);
                ServerPlayer player = (ServerPlayer) players[connection];

                WalkToTargetRequest(player, targetPosition);
            }
        }

        /*
         * Internal Functions
         */

        /// <summary>
        /// Requests a walk to coordinates
        /// </summary>
        /// <param name="player">The server player</param>
        /// <param name="targetPosition">The position Vector3</param>
        private void WalkToTargetRequest(ServerPlayer player, Vector3 targetPosition)
        {
            Debug.Log("Walk Target request:" + targetPosition);
            //Find closest point on navmesh or don't change
            if(NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
            {
                //Get hit position
                Vector3 newPosition = hit.position;

                //Set destination
                player.GetPlayerAgent().SetDestination(newPosition);
            }
        }

        /// <summary>
        /// Loads the player after a login
        /// </summary>
        /// <param name="player">The server player reference</param>
        private void LoadPlayer(ServerPlayer player)
        {
            GameObject playerObject = GameObject.Instantiate(serverAssetManager.GetPlayerPrefab());
            playerObject.name = "Player_" + player.getConnection().InternalId;
            playerObject.transform.position = new Vector3(125, 0, 125);

            player.SetPlayerGameObject(playerObject);

            NavMeshAgent agent = playerObject.AddComponent<NavMeshAgent>();
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
            player.SetPlayerAgent(agent);
            
        }

        /// <summary>
        /// Unloads a player after a logout
        /// </summary>
        /// <param name="player">The server player reference</param>
        private void UnloadPlayer(ServerPlayer player)
        {

        }

        /// <summary>
        /// Called every frame to process players
        /// </summary>
        private void ProcessPlayers()
        {
            //Iterate players
            foreach(System.Object playerObject in players.Values)
            {
                ServerPlayer player = (ServerPlayer) playerObject;

                //Update position if moving
                if (player.GetPlayerAgent().velocity.magnitude > 0)
                {
                    UpdateMyPlayerPosition_3 packet = new UpdateMyPlayerPosition_3();
                    packet.x = player.GetPlayerGameObject().transform.position.x;
                    packet.y = player.GetPlayerGameObject().transform.position.y;
                    packet.z = player.GetPlayerGameObject().transform.position.z;

                    connectionManager.SendPacketToClient(player.getConnection(), packet);
                }
            }
        }
    }
}