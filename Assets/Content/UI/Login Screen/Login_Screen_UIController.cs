using Engine.UI;
using Engine.API;
using UnityEngine.UIElements;
using Engine.Logging;
using UnityEngine;

namespace Content.UI
{
    public class Login_Screen_UIController : UIController
    {
        private TextField username;
        private TextField password;
        private Button loginButton, forgotButton, newUserButton;
        private Toggle rememberMe;
        private Label statusMessage;
        private VisualElement statusPanel;

        /// <summary>Called when the panel finished loading the UXML</summary>
        public override void onPanelLoaded()
        {
            username = getElement<TextField>("Username");            
            password = getElement<TextField>("Password");
            rememberMe = getElement<Toggle>("Remember_Me");
            loginButton = getElement<Button>("Login_Button");
            forgotButton = getElement<Button>("Forgot_Button");
            newUserButton = getElement<Button>("New_User_Button");
            statusMessage = getElement<Label>("Status_Message");
            statusPanel = getElement<VisualElement>("Status_Panel");

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
            API.Client.AttemptLogin(this.getPlaceholderTextFieldRealValue(username), this.getPlaceholderTextFieldRealValue(password), rememberMe.value, updateStatusMessage);
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

        /// <summary>
        /// Updates the status message of the login screen
        /// </summary>
        /// <param name="message">The status message.</param>
        private void updateStatusMessage(Color color, string message)
        {
            statusPanel.visible = true;
            statusMessage.text = message;
            statusMessage.style.color = new StyleColor(color);
        }
    }

}