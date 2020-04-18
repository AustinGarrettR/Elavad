using Engine.UI;
using Engine.API;
using UnityEngine.UIElements;
using Unity.UIElements.Runtime;
using Engine.Logging;
using UnityEngine;

namespace Content.UI
{
    /// <summary>
    /// UI Controller for the login screen UI panel
    /// </summary>
    public class Login_Screen_UIController : UIController
    {

        /*
         * Internal Variables
         */

        private TextField username;
        private TextField password;
        private Button loginButton, forgotButton, newUserButton;
        private Toggle rememberMe;
        private Label statusMessage;
        private VisualElement statusPanel, background;

        /*
         * Functions
         */

        /// <summary>Called when the panel finished loading the UXML</summary>
        public override void onPanelLoaded()
        {
            //Load elements
            background = getElement<VisualElement>("Background");
            username = getElement<TextField>("Username");            
            password = getElement<TextField>("Password");
            rememberMe = getElement<Toggle>("Remember_Me");
            loginButton = getElement<Button>("Login_Button");
            forgotButton = getElement<Button>("Forgot_Button");
            newUserButton = getElement<Button>("New_User_Button");
            statusMessage = getElement<Label>("Status_Message");
            statusPanel = getElement<VisualElement>("Status_Panel");

            //Delegate focus to children
            //(weird bug where selecting outer div-
            //-of the text field doesn't allow input.)
            username.delegatesFocus = true;
            password.delegatesFocus = true;

            //Register the field as a placeholder
            RegisterPlaceholderTextField(username);
            RegisterPlaceholderTextField(password);

            //Set username to use capital letter for the first input
            RegisterFirstLetterUpperCaseFormat(username);

            //Set password to mask with astericks when not displaying placeholder text
            //Setting password masking via uxml would also mask placeholder text
            RegisterPlaceholderWithPasswordFormat(password);

            //Callbacks for button presses.
            loginButton.RegisterCallback<MouseUpEvent>(LoginButtonPressed);
            forgotButton.RegisterCallback<MouseUpEvent>(ForgotButtonPressed);
            newUserButton.RegisterCallback<MouseUpEvent>(NewUserButtonPressed);
        }

        /// <summary>
        /// When the login button is pressed this function is called.
        /// </summary>
        /// <param name="mouseUpEvent">The mouse up event.</param>
        private void LoginButtonPressed(MouseUpEvent mouseUpEvent)
        {
            API.Client.AttemptLogin(this.getPlaceholderTextFieldRealValue(username), this.getPlaceholderTextFieldRealValue(password), rememberMe.value, updateStatusMessage, updateOpacity);
        }

        /// <summary>
        /// When the trouble logging in button is pressed this function is called.
        /// </summary>
        /// <param name="mouseUpEvent">The mouse up event.</param>
        private void ForgotButtonPressed(MouseUpEvent mouseUpEvent)
        {
            //TODO, LOGIN
            Log.LogMsg("TODO: Trouble Logging in button has been pressed.");
        }

        /// <summary>
        /// When the new user button is pressed this function is called.
        /// </summary>
        /// <param name="mouseUpEvent">The mouse up event.</param>
        private void NewUserButtonPressed(MouseUpEvent mouseUpEvent)
        {
            //TODO, LOGIN
            Log.LogMsg("TODO: New User button has been pressed.");
        }

        /// <summary>
        /// Updates the login status message.
        /// </summary>
        /// <param name="disableInteraction">if true, disable panel interaction</param>
        /// <param name="color">The color of the message.</param>
        /// <param name="message">The message.</param>
        private void updateStatusMessage(bool disableInteraction, Color color, string message)
        {
            statusPanel.visible = true;
            statusMessage.text = message;
            statusMessage.style.color = new StyleColor(color);
            gameObject.GetComponent<UIElementsEventSystem>().enabled = disableInteraction;
        }

        /// <summary>
        /// Updates the opacity of the main panel
        /// </summary>
        /// <param name="opacity">The opacity value.</param>
        private void updateOpacity(float opacity)
        {
            background.style.opacity = new StyleFloat(opacity);
        }

    }

}