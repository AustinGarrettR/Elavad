using Engine.Account;
using System;
using UnityEngine;

namespace Engine.API
{

    public static partial class API
    {
        public static partial class Client {

            /// <summary>
            /// Attempts a login for the player based on input.
            /// </summary>
            /// <param name="username">The username input.</param>
            /// <param name="password">The password input.</param>
            /// <param name="rememberMe">If the client should remember the player.</param>
            /// <param name="statusMessageAction">Action that processes the status message.</param>
            /// <param name="opacityAction">Action that processes the opacity value for the panel.</param>
            public static void AttemptLogin(string username, string password, bool rememberMe, Action<bool, Color, string> statusMessageAction, Action<float> opacityAction)
            {
                GetManager<ClientLoginManager>().AttemptLogin(username, password, rememberMe, statusMessageAction, opacityAction);
            }


        }

    }

}