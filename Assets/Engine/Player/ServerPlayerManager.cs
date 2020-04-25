using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine.Factory;
using UnityEngine.AI;
using Engine.Account;
using Unity.Networking.Transport;
using Engine.Asset;
using Engine.Networking;
using Engine.Utility;
using Engine.Configuration;

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
            serverLoginManager.NotifyClientLoggedIn += OnLogin;

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
        /// Called on client login
        /// </summary>
        /// <param name="client">The network connection for the client</param>
        /// <param name="packet">The login response packet for loading</param>
        private void OnLogin(NetworkConnection client, LoginResponse_2 packet)
        {
            ServerPlayer serverPlayer = new ServerPlayer(client);
            LoadPlayer(serverPlayer);

            packet.x = serverPlayer.GetPlayerGameObject().transform.position.x;
            packet.y = serverPlayer.GetPlayerGameObject().transform.position.y;
            packet.z = serverPlayer.GetPlayerGameObject().transform.position.z;
            packet.rotationX = serverPlayer.GetPlayerGameObject().transform.rotation.x;
            packet.rotationY = serverPlayer.GetPlayerGameObject().transform.rotation.y;
            packet.rotationZ = serverPlayer.GetPlayerGameObject().transform.rotation.z;
            packet.rotationW = serverPlayer.GetPlayerGameObject().transform.rotation.w;

            players.Add(client, serverPlayer);
        }

        /// <summary>
        /// Called on player successful login and load
        /// </summary>
        /// <param name="client">The network connection for the client</param>
        private void OnLoginAndLoaded(NetworkConnection client)
        {

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
                WalkRequest_5 packet = connectionManager.GetPacket<WalkRequest_5>();
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
            playerObject.transform.position = new Vector3(135, 0, 125);

            player.SetPlayerGameObject(playerObject);

            NavMeshAgent agent = playerObject.AddComponent<NavMeshAgent>();
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
            agent.acceleration = 1000;
            agent.angularSpeed = 360;
            agent.stoppingDistance = 0.1f;
            agent.autoBraking = false;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            player.SetPlayerAgent(agent);

        }

        /// <summary>
        /// Unloads a player after a logout
        /// </summary>
        /// <param name="player">The server player reference</param>
        private void UnloadPlayer(ServerPlayer player)
        {
            //Destroy game object
            GameObject.Destroy(player.GetPlayerGameObject());

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
                if (player.GetPlayerAgent().velocity.magnitude > 0 && TimeHandler.getTimeInMilliseconds() - player.lastMovementUpdateTimestamp >= SharedConfig.TRANSFORM_UPDATE_INTERVAL_IN_MILLISECONDS)
                {
                    player.lastMovementUpdateTimestamp = TimeHandler.getTimeInMilliseconds();
                    SendMyPlayerTransformUpdateForPlayer(player, false);
                }

                CheckForNearbyPlayers(player);
                UpdateNearbyPlayers(player);
            }
        }

        private void CheckForNearbyPlayers(ServerPlayer player)
        {
            //Only check every so often set by interval
            if (TimeHandler.getTimeInMilliseconds() - player.lastNearbyPlayerCheckTimestamp < SharedConfig.NEARBY_PLAYERS_CHECK_INTERVAL_IN_MILLISECONDS)
                return;

            player.lastNearbyPlayerCheckTimestamp = TimeHandler.getTimeInMilliseconds();

            foreach(ServerPlayer otherPlayer in players.Values)
            {
                //Skip ourselves
                if (otherPlayer.Equals(player))
                    continue;

                //Convert to 2D space ignoring Y coordinate
                Vector2 playerPos = new Vector2(player.GetPlayerGameObject().transform.position.x, player.GetPlayerGameObject().transform.position.z);
                Vector2 otherPlayerPos = new Vector2(otherPlayer.GetPlayerGameObject().transform.position.x, otherPlayer.GetPlayerGameObject().transform.position.z);

                //Get the distance between them horizontally
                float distanceBetween = Vector2.Distance(playerPos, otherPlayerPos);

                if(distanceBetween < SharedConfig.NEARBY_PLAYERS_DISTANCE)
                {
                    //Add
                    AddPlayerToNearbyPlayers(player, otherPlayer);
                } else
                {
                    //Remove
                    RemovePlayerFromNearbyPlayers(player, otherPlayer);
                }
            }

        }

        /// <summary>
        /// Adds another player to a player's nearby players list
        /// </summary>
        /// <param name="player">The server player instance</param>
        /// <param name="otherPlayer">The player being added to the nearby players</param>
        private void AddPlayerToNearbyPlayers(ServerPlayer player, ServerPlayer otherPlayer)
        {
            //Check if the player is already in there
            if (player.nearbyPlayers.Contains(otherPlayer))
                return;

            player.nearbyPlayers.Add(otherPlayer);

            //Send add player packet
            AddOtherPlayerToPlayer(player, otherPlayer);

        }

        /// <summary>
        /// Removes another player to a player's nearby players list
        /// </summary>
        /// <param name="player">The server player instance</param>
        /// <param name="otherPlayer">The player being removed to the nearby players</param>
        private void RemovePlayerFromNearbyPlayers(ServerPlayer player, ServerPlayer otherPlayer)
        {
            //Check if the player is already in there
            if (player.nearbyPlayers.Contains(otherPlayer) == false)
                return;

            player.nearbyPlayers.Remove(otherPlayer);

            //Send remove player packet
            RemoveOtherPlayerToPlayer(player, otherPlayer);
        }

        /// <summary>
        /// Update nearby players positions
        /// </summary>
        /// <param name="player">The player to receive the updates</param>
        private void UpdateNearbyPlayers(ServerPlayer player)
        {
            //Check if its been enough time
            if(TimeHandler.getTimeInMilliseconds() - player.lastNearbyPlayerUpdateTimestamp < SharedConfig.NEARBY_PLAYERS_TRANSFORM_UPDATE_INTERVAL_IN_MILLISECONDS)            
                return;            

            player.lastNearbyPlayerUpdateTimestamp = TimeHandler.getTimeInMilliseconds();

            //iterate other players to update their positions
            foreach(ServerPlayer otherPlayer in player.nearbyPlayers)
            {
                //Check to make sure they're moving
                if(otherPlayer.GetPlayerAgent().velocity.magnitude > 0)
                {
                    //Update position
                    SendOtherPlayerTransformUpdateForPlayer(player, otherPlayer, false);
                }
            }
        }

        /// <summary>
        /// Send a transform update for the user's myPlayer
        /// </summary>
        /// <param name="player">The server player instance</param>
        /// <param name="instant">If the position should be instantly updated.</param>
        private void SendMyPlayerTransformUpdateForPlayer(ServerPlayer player, bool instant)
        {
            UpdateMyPlayerTransform_3 packet = connectionManager.GetPacket<UpdateMyPlayerTransform_3>();
            packet.x = player.GetPlayerGameObject().transform.position.x;
            packet.y = player.GetPlayerGameObject().transform.position.y;
            packet.z = player.GetPlayerGameObject().transform.position.z;
            packet.rotationX = player.GetPlayerGameObject().transform.rotation.x;
            packet.rotationY = player.GetPlayerGameObject().transform.rotation.y;
            packet.rotationZ = player.GetPlayerGameObject().transform.rotation.z;
            packet.rotationW = player.GetPlayerGameObject().transform.rotation.w;
            packet.movementSpeed = player.GetPlayerAgent().speed;
            packet.angularSpeed = player.GetPlayerAgent().angularSpeed;
            packet.instantUpdate = instant;

            connectionManager.SendPacketToClient(player.getConnection(), packet);
        }

        /// <summary>
        /// Send a transform update for the user's myPlayer
        /// </summary>
        /// <param name="player">The server player instance</param>
        /// <param name="otherPlayer">The other player instance</param>
        /// <param name="instant">If the position should be instantly updated.</param>
        private void SendOtherPlayerTransformUpdateForPlayer(ServerPlayer player, ServerPlayer otherPlayer, bool instant)
        {
            UpdateOtherPlayerTransform_6 packet = connectionManager.GetPacket<UpdateOtherPlayerTransform_6>();
            packet.x = otherPlayer.GetPlayerGameObject().transform.position.x;
            packet.y = otherPlayer.GetPlayerGameObject().transform.position.y;
            packet.z = otherPlayer.GetPlayerGameObject().transform.position.z;
            packet.rotationX = otherPlayer.GetPlayerGameObject().transform.rotation.x;
            packet.rotationY = otherPlayer.GetPlayerGameObject().transform.rotation.y;
            packet.rotationZ = otherPlayer.GetPlayerGameObject().transform.rotation.z;
            packet.rotationW = otherPlayer.GetPlayerGameObject().transform.rotation.w;
            packet.movementSpeed = otherPlayer.GetPlayerAgent().speed;
            packet.angularSpeed = otherPlayer.GetPlayerAgent().angularSpeed;
            packet.instantUpdate = instant;
            packet.uniqueId = player.GetPlayerGameObject().GetInstanceID();

            connectionManager.SendPacketToClient(player.getConnection(), packet);
        }

        /// <summary>
        /// Adds another player to the nearby players on the client
        /// </summary>
        /// <param name="player">The server player instance</param>
        /// <param name="otherPlayer">The other player instance</param>
        private void AddOtherPlayerToPlayer(ServerPlayer player, ServerPlayer otherPlayer)
        {
            AddOtherPlayer_7 packet = connectionManager.GetPacket<AddOtherPlayer_7>();
            packet.x = otherPlayer.GetPlayerGameObject().transform.position.x;
            packet.y = otherPlayer.GetPlayerGameObject().transform.position.y;
            packet.z = otherPlayer.GetPlayerGameObject().transform.position.z;
            packet.rotationX = otherPlayer.GetPlayerGameObject().transform.rotation.x;
            packet.rotationY = otherPlayer.GetPlayerGameObject().transform.rotation.y;
            packet.rotationZ = otherPlayer.GetPlayerGameObject().transform.rotation.z;
            packet.rotationW = otherPlayer.GetPlayerGameObject().transform.rotation.w;
            packet.uniqueId = player.GetPlayerGameObject().GetInstanceID();

            connectionManager.SendPacketToClient(player.getConnection(), packet);
        }

        /// <summary>
        /// Removes another player from the nearby players on the client
        /// </summary>
        /// <param name="player">The server player instance</param>
        /// <param name="otherPlayer">The other player instance</param>
        private void RemoveOtherPlayerToPlayer(ServerPlayer player, ServerPlayer otherPlayer)
        {
            RemoveOtherPlayer_8 packet = connectionManager.GetPacket<RemoveOtherPlayer_8>();
            packet.uniqueId = player.GetPlayerGameObject().GetInstanceID();

            connectionManager.SendPacketToClient(player.getConnection(), packet);
        }
    }
}