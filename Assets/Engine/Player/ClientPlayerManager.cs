using UnityEngine;
using Engine.Factory;
using System.Threading.Tasks;
using Engine.Asset;
using Engine.Networking;
using Unity.Networking.Transport;
using System.Threading;

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
        public override async Task LoadGameTask()
        {
            Task createPlayerTask = CreateMyPlayerTask();
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

        /*
         * Internal Functions
         */

        /// <summary>
        /// Called every frame to check for walk input
        /// </summary>
        private void CheckForClickToWalkInput()
        {
            //Check for right click
            //TODO change to permanent solution
            if (Input.GetMouseButtonDown(1))
            {
                //Find point
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 250f))
                {
                    SendWalkRequest(hit.point);
                }
            }
        }

        /// <summary>
        /// Send walk request to the server
        /// </summary>
        /// <param name="targetPosition">The target position</param>
        private void SendWalkRequest(Vector3 targetPosition)
        {
            WalkRequest_5 packet = new WalkRequest_5();
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
                UpdateMyPlayerPosition_3 packet = new UpdateMyPlayerPosition_3();
                packet.readPacket(packetBytes);

                UpdateMyPlayerPosition(new Vector3(packet.x, packet.y, packet.z));
            }
        }



        /// <summary>
        /// Updates the client position from the server
        /// </summary>
        /// <param name="newPosition">The new position vector3</param>
        private void UpdateMyPlayerPosition(Vector3 newPosition)
        {
            myPlayer.GetPlayerObject().transform.position = newPosition;
        }

        /// <summary>
        /// Async task to trigger create player on game load
        /// </summary>
        /// <returns></returns>
        private async Task CreateMyPlayerTask()
        {
            //Create instance for my player
            myPlayer = new ClientPlayer("_MyPlayer", clientAssetManager.GetPlayerPrefab(), new Vector3(125, 0, 125), Vector3.zero);

            await Task.CompletedTask;
        }

    }
}