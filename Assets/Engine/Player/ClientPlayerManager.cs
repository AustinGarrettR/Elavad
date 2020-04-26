using UnityEngine;
using Engine.Factory;
using System.Threading.Tasks;
using Engine.Asset;
using Engine.Networking;
using Unity.Networking.Transport;
using System.Threading;
using Engine.Utility;
using Engine.Configuration;
using System;
using Engine.Loading;
using System.Collections.Generic;
using UnityEngine.AI;

namespace Engine.Player
{
    /// <summary>
    /// Manages the client player
    /// </summary>
    public class ClientPlayerManager : Manager
    {

        /*
         * Constructor
         */

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientAssetManager">The client asset manager reference</param>
        public ClientPlayerManager(ClientAssetManager clientAssetManager, ConnectionManager connectionManager)
        {
            this.clientAssetManager = clientAssetManager;
            this.connectionManager = connectionManager;
        }

        /*
         * Override Methods
         */

        /// <summary>
        /// Called on initialization
        /// </summary>
        /// <param name="parameters">Asset manager reference</param>
        public override void Init()
        {
            //Subscribe to events
            connectionManager.NotifyPacketReceived += OnPacketReceived;
        }

        /// <summary>
        /// Called on game load
        /// </summary>
        /// <returns></returns>
        public override async Task LoadGameTask(ClientLoadData clientLoadData)
        {
            Task createPlayerTask = CreateMyPlayerTask(clientLoadData.startPosition, clientLoadData.startRotation);
            await createPlayerTask;

            gameLoaded = true;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        public override void Process()
        {
            if(gameLoaded)
            {
                CheckForClickToWalkInput();
                ProcessMyPlayerMovement();
                ProcessOtherPlayerMovement();
            }
        }

        /// <summary>
        /// Called on shutdown
        /// </summary>
        public override void Shutdown()
        {
            
        }

        /*
         * Public Functions
         */

        public ClientPlayer GetMyPlayer()
        {
            return myPlayer;
        }

        /*
         * Internal Variables
         */

        /// <summary>
        /// The connection manager reference
        /// </summary>
        private ConnectionManager connectionManager;

        /// <summary>
        /// The client asset manager reference
        /// </summary>
        private ClientAssetManager clientAssetManager;

        /// <summary>
        /// Whether the game is loaded or not
        /// </summary>
        private bool gameLoaded;

        /// <summary>
        /// The player object
        /// </summary>
        private ClientPlayer myPlayer;

        /// <summary>
        /// Nearby client players
        /// </summary>
        private List<ClientPlayer> otherPlayers = new List<ClientPlayer>();

        /// <summary>
        /// How long ago the last walk update was sent when holding mouse down
        /// </summary>
        private long lastWalkInputHoldTimestamp;

        /*
         * Internal Functions
         */

        /// <summary>
        /// Called every frame to check for walk input
        /// </summary>
        private void CheckForClickToWalkInput()
        {

            bool sendWalk = false;
            if (Input.GetMouseButtonDown(1))
                sendWalk = true;

            if (sendWalk == false && Input.GetMouseButton(1) && TimeHandler.getTimeInMilliseconds() - lastWalkInputHoldTimestamp > ClientConfig.MOUSE_HOLD_MOVEMENT_TIME_IN_MILLISECONDS)
            {
                lastWalkInputHoldTimestamp = TimeHandler.getTimeInMilliseconds();
                sendWalk = true;
            }

            if (sendWalk)
            {
                int layerMask =~ LayerMask.GetMask(SharedConfig.PLAYERS_LAYER_NAME, SharedConfig.VOLUMES_LAYER_NAME);
                //Find point
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 250f, layerMask))
                {
                    if (NavMesh.SamplePosition(hit.point, out NavMeshHit navHit, 5f, 1 << NavMesh.GetAreaFromName("Walkable")))
                    {
                        SendWalkRequest(navHit.position);
                    }
                }
            }
        }

        /// <summary>
        /// Called every frame to lerp the position
        /// </summary>
        private void ProcessMyPlayerMovement()
        {

            //Interpolate Position
            if (Vector3.Distance(myPlayer.GetPlayerObject().transform.position, myPlayer.targetTransformUpdatePosition) > (myPlayer.movementSpeed * 0.8f / (1000 / SharedConfig.TRANSFORM_UPDATE_INTERVAL_IN_MILLISECONDS)))
            {
                Vector3 positionDirection = (myPlayer.targetTransformUpdatePosition - myPlayer.GetPlayerObject().transform.position).normalized;
                myPlayer.GetPlayerObject().transform.position = myPlayer.GetPlayerObject().transform.position + positionDirection * myPlayer.movementSpeed * Time.deltaTime;
            }

            //Interpolate Rotation
            myPlayer.GetPlayerObject().transform.rotation = Quaternion.RotateTowards(myPlayer.GetPlayerObject().transform.rotation, myPlayer.targetTransformUpdateRotation, Time.deltaTime * myPlayer.angularSpeed);

        }

        /// <summary>
        /// Called every frame to lerp the position
        /// </summary>
        private void ProcessOtherPlayerMovement()
        {
            foreach (ClientPlayer player in otherPlayers)
            {
                //Interpolate Position
                if (Vector3.Distance(player.GetPlayerObject().transform.position, player.targetTransformUpdatePosition) > (player.movementSpeed * 0.8f / (1000 / SharedConfig.NEARBY_PLAYERS_TRANSFORM_UPDATE_INTERVAL_IN_MILLISECONDS)))
                {
                    Vector3 positionDirection = (player.targetTransformUpdatePosition - player.GetPlayerObject().transform.position).normalized;
                    player.GetPlayerObject().transform.position = player.GetPlayerObject().transform.position + positionDirection * player.movementSpeed * Time.deltaTime;
                }

                //Interpolate Rotation
                player.GetPlayerObject().transform.rotation = Quaternion.RotateTowards(player.GetPlayerObject().transform.rotation, player.targetTransformUpdateRotation, Time.deltaTime * player.angularSpeed);
            }
        }

        /// <summary>
        /// Send walk request to the server
        /// </summary>
        /// <param name="targetPosition">The target position</param>
        private void SendWalkRequest(Vector3 targetPosition)
        {
            WalkRequest_5 packet = connectionManager.GetPacket<WalkRequest_5>();
            packet.x = targetPosition.x;
            packet.y = targetPosition.y;
            packet.z = targetPosition.z;

            connectionManager.SendPacketToServer(packet);
        }

        /// <summary>
        /// Called when a packet is received
        /// </summary>
        /// <param name="c">The player connection</param>
        /// <param name="packetId">The packet ID</param>
        /// <param name="packetBytes">The bytes to process</param>
        private void OnPacketReceived(NetworkConnection c, int packetId, byte[] packetBytes)
        {
            if (packetId == 3)
            {
                //Read packet
                UpdateMyPlayerTransform_3 packet = connectionManager.GetPacket<UpdateMyPlayerTransform_3>();
                packet.readPacket(packetBytes);

                UpdateMyPlayerPosition(new Vector3(packet.x, packet.y, packet.z), new Quaternion(packet.rotationX, packet.rotationY, packet.rotationZ, packet.rotationW), packet.movementSpeed, packet.angularSpeed, packet.instantUpdate);
            } else
            if (packetId == 6)
            {
                //Read packet
                UpdateOtherPlayerTransform_6 packet = connectionManager.GetPacket<UpdateOtherPlayerTransform_6>();
                packet.readPacket(packetBytes);

                UpdateOtherPlayerPosition(packet.uniqueId, new Vector3(packet.x, packet.y, packet.z), new Quaternion(packet.rotationX, packet.rotationY, packet.rotationZ, packet.rotationW), packet.movementSpeed, packet.angularSpeed, packet.instantUpdate);
            }
            else
            if (packetId == 7)
            {                
                //Read packet
                AddOtherPlayer_7 packet = connectionManager.GetPacket<AddOtherPlayer_7>();
                packet.readPacket(packetBytes);

                AddOtherPlayer(packet.uniqueId, new Vector3(packet.x, packet.y, packet.z), new Quaternion(packet.rotationX, packet.rotationY, packet.rotationZ, packet.rotationW));
            }
            else
            if (packetId == 8)
            {
                //Read packet
                RemoveOtherPlayer_8 packet = connectionManager.GetPacket<RemoveOtherPlayer_8>();
                packet.readPacket(packetBytes);

                RemoveOtherPlayer(packet.uniqueId);
            }
        }

        /// <summary>
        /// Add other player to the game
        /// </summary>
        /// <param name="uniqueId">The player's unique id</param>
        /// <param name="startPosition">The player's start position</param>
        /// <param name="startRotation">The player's start rotation</param>
        private void AddOtherPlayer(int uniqueId, Vector3 startPosition, Quaternion startRotation)
        {
           ClientPlayer otherPlayer = new ClientPlayer(uniqueId, clientAssetManager.GetPlayerPrefab(), startPosition, startRotation);
           otherPlayers.Add(otherPlayer);
        }

        /// <summary>
        /// Remove other player from the game
        /// </summary>
        /// <param name="uniqueId">The player's unique id</param>
        private void RemoveOtherPlayer(int uniqueId)
        {
            foreach(ClientPlayer player in otherPlayers)
            {
                if (player.uniqueId != uniqueId)
                    continue;

                GameObject.Destroy(player.GetPlayerObject());
                otherPlayers.Remove(player);
                break;
            }
        }

        /// <summary>
        /// Updates the client position from the server
        /// </summary>
        /// <param name="newPosition">The new position vector3</param>
        private void UpdateOtherPlayerPosition(int uniqueId, Vector3 newPosition, Quaternion newRotation, float movementSpeed, float angularSpeed, bool instantUpdate)
        {
            foreach (ClientPlayer player in otherPlayers)
            {
                if (player.uniqueId != uniqueId)
                    continue;

                player.movementSpeed = movementSpeed;
                player.angularSpeed = angularSpeed;
                player.targetTransformUpdatePosition = newPosition;
                player.targetTransformUpdateRotation = newRotation;

                if (instantUpdate)
                {
                    player.GetPlayerObject().transform.position = newPosition;
                    player.GetPlayerObject().transform.rotation = newRotation;
                }
                break;
            }
        }

        /// <summary>
        /// Updates the client position from the server
        /// </summary>
        /// <param name="newPosition">The new position vector3</param>
        private void UpdateMyPlayerPosition(Vector3 newPosition, Quaternion newRotation, float movementSpeed, float angularSpeed, bool instantUpdate)
        {
            myPlayer.movementSpeed = movementSpeed;
            myPlayer.angularSpeed = angularSpeed;
            myPlayer.targetTransformUpdatePosition = newPosition;
            myPlayer.targetTransformUpdateRotation = newRotation;

            if(instantUpdate)
            {
                myPlayer.GetPlayerObject().transform.position = newPosition;
                myPlayer.GetPlayerObject().transform.rotation = newRotation;
            }
        }

        /// <summary>
        /// Create my player
        /// </summary>
        /// <returns></returns>
        private async Task CreateMyPlayerTask(Vector3 startPosition, Quaternion startRotation)
        {
            //Create instance for my player
            myPlayer = new ClientPlayer(-1, clientAssetManager.GetPlayerPrefab(), startPosition, startRotation);
            await Task.CompletedTask;
        }

    }
}