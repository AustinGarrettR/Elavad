using Engine.Networking;
using Unity.Networking.Transport;
using Engine.Factory;
using Engine.Logging;

namespace Engine.Account
{
    /// <summary>
    /// Manager for server to accept logins from clients
    /// </summary>
    public class ServerLoginManager : Manager
    {
        /*
         * Constructor
         */

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionManager">The connection manager reference</param>
        public ServerLoginManager(ConnectionManager connectionManager)
        { 
            this.connectionManager = connectionManager;
        }

        /*
         * Override Methods
         */

        /// <summary>
        /// Called on initialization
        /// </summary>
        /// <param name="parameters">Connection manager parameter</param>
        public override void Init()
        {
            //Subscribe to events
            connectionManager.NotifyPacketReceived += OnPacketReceived;
            connectionManager.NotifyClientDisconnected += OnClientDisconnected;
        }

        /// <summary>
        /// Called on program shutdown
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

        /*
         * Public Variables
         */

        /// <summary>
        /// Event delegate for a successful login and load
        /// </summary>
        /// <param name="c">The network connection</param>
        public delegate void NotifyOnLoginAndLoaded(NetworkConnection c);

        /// <summary>
        /// Event for a successful login and load
        /// </summary>
        public event NotifyOnLoginAndLoaded NotifyClientLoggedInAndLoaded;

        /// <summary>
        /// Event delegate for a successful login
        /// </summary>
        /// <param name="c">The network connection</param>
        public delegate void NotifyOnLogin(NetworkConnection c, LoginResponse_2 responsePacket);

        /// <summary>
        /// Event for a successful login
        /// </summary>
        public event NotifyOnLogin NotifyClientLoggedIn;

        /// <summary>
        /// Event delegate for logging out
        /// </summary>
        /// <param name="c">The network connection</param>
        public delegate void NotifyOnLogout(NetworkConnection c);

        /// <summary>
        /// Event for a successful login
        /// </summary>
        public event NotifyOnLogout NotifyClientLoggedOut;

        /*
         * Internal Variables
         */

        private ConnectionManager connectionManager;

        /*
         * Event Functions
         */

        /// <summary>
        /// Called when a packet is received
        /// </summary>
        /// <param name="c">The player connection</param>
        /// <param name="packetId">The packet ID</param>
        /// <param name="packetBytes">The bytes to process</param>
        private void OnPacketReceived(NetworkConnection c, int packetId, byte[] packetBytes)
        {
            if (packetId == 1)
            {
                //Read packet
                LoginRequest_1 packet = connectionManager.GetPacket<LoginRequest_1>();
                packet.readPacket(packetBytes);

                LogInPlayer(c, packet.email, packet.password);
            } else
            if (packetId == 4)
            {
                //Packet is empty, ignore contents
                NotifyClientLoggedInAndLoaded?.Invoke(c);
            }
        }

        /// <summary>
        /// Called when a client disconnects
        /// </summary>
        /// <param name="c">The client connection</param>
        private void OnClientDisconnected(NetworkConnection c)
        {
            NotifyClientLoggedOut?.Invoke(c);
        }

        /*
         * Internal Functions
         */

        /// <summary>
        /// Process a login request
        /// </summary>
        /// <param name="c">The player connection</param>
        /// <param name="email">Inputted player email</param>
        /// <param name="password">Inputted player password</param>
        private void LogInPlayer(NetworkConnection c, string email, string password)
        {
            LoginResponse_2 packet = connectionManager.GetPacket<LoginResponse_2>();

            //TODO
            if (email.Equals("austin@gmail.com", System.StringComparison.OrdinalIgnoreCase))
            {
                packet.accept = true;
                packet.errorResponse = "Logging in...";

                NotifyClientLoggedIn?.Invoke(c, packet);

                connectionManager.SendPacketToClient(c, packet);
            }
            else
            {
                packet.accept = false;
                packet.errorResponse = "Invalid email or password.";


                connectionManager.SendPacketToClient(c, packet);
            }


        }

    }

}