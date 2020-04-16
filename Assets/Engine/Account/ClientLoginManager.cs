using Engine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.Networking.Transport;
using UnityEngine;
using Engine.Factory;

namespace Engine.Account
{
    public class ClientLoginManager : Manager
    {
        /*
         * Override Methods
         */

        //Initialize method
        public override void init(params object[] parameters)
        {
            this.connectionManager = (ConnectionManager)parameters[1];

            //Subscribe to events
            connectionManager.NotifyFailedConnect += onFailedToConnect;
            connectionManager.NotifyOnConnectedToServer += onConnectedToServer;
            connectionManager.NotifyPacketReceived += OnPacketReceived;
            connectionManager.NotifyOnDisconnectedFromServer += onDisconnectedFromServer;
        }

        //Called on program shutdown
        public override void shutdown()
        {

        }

        //Called every frame
        public override void update()
        {

        }


        /*
         * Public Functions
         */

        //Login button pressed
        /// <summary>Call when the login button pressed.</summary>
        /// <param name="email">The email.</param>
        /// <param name="password">The password.</param>
        /// <param name="rememberMe">if set to <c>true</c> [remember me].</param>
        public void onLoginButtonPressed(string email, string password, bool rememberMe)
        {
            this.email = email;
            this.password = password;
            this.rememberMe = rememberMe;

            //Check input
            if (email.Length == 0 || password.Length == 0)
            {
                //LoginScreenUI.getSingleton().setStatusMessage("<color=#d66f6f>Please enter your email and password.</color>");
                return;
            }
            if (IsValidEmail(email) == false)
            {
                // LoginScreenUI.getSingleton().setStatusMessage("<color=#d66f6f>Invalid email address.</color>");
                return;
            }

            //Start connection
            connectionManager.start();
            //  LoginScreenUI.getSingleton().setStatusMessage("Connecting to server...");

        }

        /*
         * Internal Variables
         */

        private ConnectionManager connectionManager;

        private string email;
        private string password;
        private bool rememberMe;

        private bool loggedIn;


        /*
         * Event Functions
         */

        //Packet received
        private void OnPacketReceived(NetworkConnection c, int packetId, byte[] packetBytes)
        {
            if (packetId == 2)
            {
                //Read packet
                LoginResponse_2 packet = new LoginResponse_2();
                packet.readPacket(packetBytes);

                LoginResponse(packet.accept, packet.errorResponse);
            }
        }

        //Disconnected from server
        private void onDisconnectedFromServer()
        {
            if (loggedIn)
            {
                // LoginScreenUI.getSingleton().setStatusVisibility(false);
                loggedIn = false;
            }
            //LoginScreenUI.getSingleton().setContainerVisibility(true);
        }

        //Unable to connect to server
        private void onFailedToConnect()
        {
            // LoginScreenUI.getSingleton().setStatusMessage("<color=#d66f6f>Unable to connect to server.</color>");
        }

        //Connected to server (Pre-Authentication)
        private void onConnectedToServer()
        {
            //Send login details
            //LoginScreenUI.getSingleton().setStatusMessage("Sending login details...");

            //Form packet
            LoginRequest_1 packet = new LoginRequest_1();
            packet.email = email;
            packet.password = password;

            //Send packet
            connectionManager.sendPacketToServer(packet);
        }

        /*
         * Internal Functions
         */

        //Login authentication response
        private void LoginResponse(bool accepted, string errorResponse)
        {
            if (accepted)
            {
                //LoginScreenUI.getSingleton().setStatusMessage("Loading...");

                beginGameLoad();
                /*Action completeMethod = delegate
                {
                    beginGameLoad();
                };

                LoginScreenUI.getSingleton().FadeAlpha(false, completeMethod);*/
                loggedIn = true;

            }
            else
            {
                connectionManager.Disconnect();
                // LoginScreenUI.getSingleton().setStatusMessage("<color=#d66f6f>" + errorResponse + "</color>");
            }
        }

        //Begin game load on login
        private void beginGameLoad()
        {
            // clientLoadingScreenManager.createLoadingScreen(endGameLoad, loadGame, 1000, "Logging In...", 2000, false);
        }

        //Load game call upon successful login
        private void loadGame()
        {
            //Hide login screen here
            // LoginScreenUI.getSingleton().setContainerVisibility(false);

            //Load game

        }

        //Game is done loading
        private void endGameLoad()
        {
            Debug.Log("Done loading.");
        }

        //Returns if player is logged in
        public bool isPlayerLoggedIn()
        {
            return this.loggedIn;
        }

        /*
         * Utility Methods
         */

        //Checks if email is of valid format
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}