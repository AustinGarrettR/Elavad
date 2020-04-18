using Engine.Networking;
using System;
using System.Threading.Tasks;
using Unity.Networking.Transport;
using UnityEngine;
using Engine.Factory;
using UnityEngine.SceneManagement;
using Engine.Logging;
using Engine.Dispatch;

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
            connectionManager.NotifyFailedConnect += OnFailedToConnect;
            connectionManager.NotifyOnConnectedToServer += OnConnectedToServer;
            connectionManager.NotifyPacketReceived += OnPacketReceived;
            connectionManager.NotifyOnDisconnectedFromServer += OnDisconnectedFromServer;

            //Load login screen
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
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
        /// <param name="statusMessageAction">The action called to update the status message.</param>
        /// <param name="opacityAction">The action called to update the panel opacity.</param>
        public void AttemptLogin(string email, string password, bool rememberMe, Action<bool, Color, string> statusMessageAction, Action<float> opacityAction)
        {
            this.email = email;
            this.password = password;
            this.rememberMe = rememberMe;
            this.statusMessageAction = statusMessageAction;
            this.opacityAction = opacityAction;

            //Check input
            if (email.Length == 0 || password.Length == 0)
            {
                this.statusMessageAction(false, new Color(214f/255f, 111f/255f, 111/255f), "Please enter your email and password.");
                return;
            }
            if (IsValidEmail(email) == false)
            {
                this.statusMessageAction(false, new Color(214f / 255f, 111f / 255f, 111f / 255f), "Invalid email address.");
                return;
            }

            //Start connection
            connectionManager.start();
            this.statusMessageAction(true, Color.white, "Connecting to server...");
        }

        /*
         * Internal Variables
         */

        private ConnectionManager connectionManager;

        private string email;
        private string password;
        private bool rememberMe;
        private bool loggedIn;

        private Action<bool, Color, string> statusMessageAction;
        private Action<float> opacityAction;

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
        private void OnDisconnectedFromServer()
        {
            if (loggedIn)
            {
                loggedIn = false;
                Log.LogMsg("Disconnected from server.");

                SceneManager.LoadScene(0);
            }
        }

        //Unable to connect to server
        private void OnFailedToConnect()
        {
            Log.LogMsg("Failed to connect.");
            this.statusMessageAction(false, new Color(214f / 255f, 111f / 255f, 111f / 255f), "Unable to connect to server.");
        }

        //Connected to server (Pre-Authentication)
        private void OnConnectedToServer()
        {

            Log.LogMsg("Connected to server. Sending login details...");

            //Send login details
            this.statusMessageAction(true, Color.white, "Sending login details...");

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

                Log.LogMsg("Login details accepted.");

                this.statusMessageAction(true, Color.white, "Loading...");

                //Run game load asynchronously
                Task.Run(BeginGameLoad);
                
                loggedIn = true;

            }
            else
            {
                connectionManager.Disconnect();
                this.statusMessageAction(false, new Color(214f / 255f, 111f / 255f, 111f / 255f), errorResponse);
            }
        }

        //Begin game load on login
        private async Task BeginGameLoad()
        {
            Log.LogMsg("Loading game...");
            Task loadGameTask = LoadGame();
            await loadGameTask;

            Log.LogMsg("Game has been loaded. Fading out login screen...");
            Task fadeLoginTask = FadeLoginScreen();
            await fadeLoginTask;

            Log.LogMsg("Login screen has been faded. Ending game load.");
            Task endLoadTask = EndGameLoad();
            await endLoadTask;

            Log.LogMsg("Completed the end phase of game loading.");
            await Task.CompletedTask;
        }

        //Load game call upon successful login
        private async Task LoadGame()
        {
            await Task.CompletedTask;
        }

        //Fade login screen out
        private async Task FadeLoginScreen()
        {
            int iterations = 500;
            int delayInMillisecondsPerIteration = 3;
            for (int i = iterations; i > 0; i--)
            {
                float opacity = ((float)i) / (float) iterations;

                Dispatcher.Invoke(() => {
                     opacityAction(opacity);
                });
                await Task.Delay(delayInMillisecondsPerIteration);
            }

            Dispatcher.Invoke(() => {
                opacityAction(0);
            });
        }

        //Game is done loading, cleanup login screen
        private async Task EndGameLoad()
        {
            Dispatcher.Invoke(() => {
                SceneManager.UnloadSceneAsync(1, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            });
            await Task.CompletedTask;
        }

        //Returns if player is logged in
        public bool IsPlayerLoggedIn()
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