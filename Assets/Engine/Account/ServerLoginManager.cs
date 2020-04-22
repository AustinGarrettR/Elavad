using Engine.Networking;
using Unity.Networking.Transport;
using Engine.Factory;

namespace Engine.Account
{
    /// <summary>
    /// Manager for server to accept logins from clients
    /// </summary>
    public class ServerLoginManager : Manager
    {
        /*
         * Override Methods
         */

        /// <summary>
        /// Called on initialization
        /// </summary>
        /// <param name="parameters">Connection manager parameter</param>
        public override void Init(params object[] parameters)
        {
            connectionManager = (ConnectionManager)parameters[0];

            //Subscribe to events
            connectionManager.NotifyPacketReceived += OnPacketReceived;
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
                LoginRequest_1 packet = new LoginRequest_1();
                packet.readPacket(packetBytes);

                logInPlayer(c, packet.email, packet.password);
            }
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
        private void logInPlayer(NetworkConnection c, string email, string password)
        {
            LoginResponse_2 packet = new LoginResponse_2();

            if (email.Equals("austin@gmail.com", System.StringComparison.OrdinalIgnoreCase))
            {
                packet.accept = true;
                packet.errorResponse = "Logging in...";
            }
            else
            {
                packet.accept = false;
                packet.errorResponse = "Invalid email or password.";
            }

            connectionManager.sendPacketToClient(c, packet);

        }
    }

}