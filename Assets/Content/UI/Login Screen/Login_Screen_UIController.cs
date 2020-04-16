using Engine.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Engine.Logging;

namespace Content.UI
{
    public class Login_Screen_UIController : UIController
    {
        private TextField username;
        private TextField password;
        private Button loginButton, forgotButton, newUserButton;

        /// <summary>Called when the panel finished loading the UXML</summary>
        public override void onPanelLoaded()
        {
            username = getElement<TextField>("Username");            
            password = getElement<TextField>("Password");
            loginButton = getElement<Button>("Login_Button");
            forgotButton = getElement<Button>("Forgot_Button");
            newUserButton = getElement<Button>("New_User_Button");

            RegisterPlaceholderTextField(username);
            RegisterPlaceholderTextField(password);

            RegisterFirstLetterUpperCaseFormat(username);
            RegisterPlaceholderWithPasswordFormat(password);

            loginButton.RegisterCallback<MouseUpEvent>(loginButtonPressed);
            forgotButton.RegisterCallback<MouseUpEvent>(forgotButtonPressed);
            newUserButton.RegisterCallback<MouseUpEvent>(newUserButtonPressed);
        }

        /// <summary>
        /// When the login button is pressed this function is called.
        /// </summary>
        /// <param name="mouseUpEvent">The mouse up event.</param>
        private void loginButtonPressed(MouseUpEvent mouseUpEvent)
        {
            //TODO, LOGIN
            Log.LogMsg("TODO: Log in button has been pressed.");
        }

        /// <summary>
        /// When the trouble logging in button is pressed this function is called.
        /// </summary>
        /// <param name="mouseUpEvent">The mouse up event.</param>
        private void forgotButtonPressed(MouseUpEvent mouseUpEvent)
        {
            //TODO, LOGIN
            Log.LogMsg("TODO: Trouble Logging in button has been pressed.");
        }

        /// <summary>
        /// When the new user button is pressed this function is called.
        /// </summary>
        /// <param name="mouseUpEvent">The mouse up event.</param>
        private void newUserButtonPressed(MouseUpEvent mouseUpEvent)
        {
            //TODO, LOGIN
            Log.LogMsg("TODO: New User button has been pressed.");
        }
    }

}