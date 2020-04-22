using Engine.Networking;
using System;
using System.Threading.Tasks;
using Unity.Networking.Transport;
using UnityEngine;
using Engine.Factory;
using UnityEngine.SceneManagement;
using Engine.Logging;
using System.Collections.Generic;

namespace Engine.Account
{
    /// <summary>
    /// Manager for handling client logins to the server
    /// </summary>
    public class ClientLoginManager : Manager
    {

        /*
         * Constructor
         */

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionManager">The connection manager</param>
        /// <param name="managers">All the managers</param>
        public ClientLoginManager(ConnectionManager connectionManager, List<Manager> managers)
        {
            this.connectionManager = connectionManager;
            this.managers = managers;
        } 

        /*
         * Override Methods
         */

        /// <summary>
        /// Initialize method
        /// </summary>
        public override void Init()
        {
            //Subscribe to events
            connectionManager.NotifyFailedConnect += OnFailedToConnect;
            connectionManager.NotifyOnConnectedToServer += OnConnectedToServer;
            connectionManager.NotifyPacketReceived += OnPacketReceived;
            connectionManager.NotifyOnDisconnectedFromServer += OnDisconnectedFromServer;

            //Load login screen
            SceneManager.LoadScene(1, LoadSceneMode.Additive);
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
            connectionManager.Start();
            this.statusMessageAction(true, Color.white, "Connecting to server...");
        }

        /*
         * Internal Variables
         */

        private List<Manager> managers;
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

        /// <summary>
        /// Packet received
        /// </summary>
        /// <param name="c">The network connection</param>
        /// <param name="packetId">The packet ID</param>
        /// <param name="packetBytes">The bytes to process</param>
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

        /// <summary>
        /// Disconnected from server
        /// </summary>
        private void OnDisconnectedFromServer()
        {
            if (loggedIn)
            {
                loggedIn = false;
                Log.LogMsg("Disconnected from server.");

                SceneManager.LoadScene(0);
            }
        }

        /// <summary>
        /// Unable to connect to server
        /// </summary>
        private void OnFailedToConnect()
        {
            Log.LogMsg("Failed to connect.");
            this.statusMessageAction(false, new Color(214f / 255f, 111f / 255f, 111f / 255f), "Unable to connect to server.");
        }

        /// <summary>
        /// Connected to server (Pre-Authentication)
        /// </summary>
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

        /// <summary>
        /// Login authentication response
        /// </summary>
        /// <param name="accepted">If it was accepted</param>
        /// <param name="errorResponse">If not accepted, the reason why</param>
        private void LoginResponse(bool accepted, string errorResponse)
        {
            if (accepted)
            {

                Log.LogMsg("Login details accepted.");

                this.statusMessageAction(true, Color.white, "Loading...");


                //Run game load asynchronously on main thread
                Task task = BeginGameLoad();
                task.RunSynchronously();

                loggedIn = true;

            }
            else
            {
                connectionManager.Disconnect();
                this.statusMessageAction(false, new Color(214f / 255f, 111f / 255f, 111f / 255f), errorResponse);
            }
        }

        /// <summary>
        /// Begin game load on login
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Load game call upon successful login
        /// </summary>
        /// <returns></returns>
        private async Task LoadGame()
        {
            foreach (Manager manager in managers)
            {
                try
                {
                    Task loadGameTask = manager.LoadGameTask();
                    await loadGameTask;
                }
                catch (Exception e)
                {
                    Log.LogError("Failed to load game on manager " + manager.GetType().Name + ". Error:" + e.Message+" Stack Trace:"+e.StackTrace);
                }
            }
        }

        /// <summary>
        /// Fade login screen out
        /// </summary>
        /// <returns></returns>
        private async Task FadeLoginScreen()
        {
            float opacity = 1f;
            float speed = 0.65f;
            while (opacity > 0f)
            {
                opacity -= (Time.deltaTime) * speed;
                if (opacity < 0)
                    opacity = 0;

                opacityAction(opacity);

                await Task.Delay(5);
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Game is done loading, cleanup login screen
        /// </summary>
        /// <returns></returns>
        private async Task EndGameLoad()
        {
            SceneManager.UnloadSceneAsync(1, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Returns if player is logged in
        /// </summary>
        /// <returns></returns>
        public bool IsPlayerLoggedIn()
        {
            return this.loggedIn;
        }

        /*
         * Utility Methods
         */

        /// <summary>
        /// Checks if string input is of valid email format
        /// </summary>
        /// <param name="email">The email</param>
        /// <returns></returns>
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