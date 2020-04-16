using Engine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.Networking.Transport;
using UnityEngine;
using Engine.Factory;

namespace Engine.Account
{
    public class ServerLoginManager : Manager
    {
        /*
         * Override Methods
         */

        public override void init(params object[] parameters)
        {
            connectionManager = (ConnectionManager)parameters[0];

            //Subscribe to events
            connectionManager.NotifyPacketReceived += OnPacketReceived;
        }

        public override void shutdown()
        {

        }

        public override void update()
        {

        }

        /*
         * Internal Variables
         */

        private ConnectionManager connectionManager;

        /*
         * Event Functions
         */

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