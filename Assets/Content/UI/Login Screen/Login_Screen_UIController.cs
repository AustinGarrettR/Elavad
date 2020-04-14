using Engine.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Content.UI
{
    public class Login_Screen_UIController : UIController
    {

        public override void onPanelLoaded()
        {

        }

        /*  //Public UI element references
          public CanvasGroup containerGroup;
          public Label emailField, passwordField;
          public Toggle rememberMeField;
          public Label statusMessage;

          //Event delegate
          public delegate void loginButtonPressedEventDelegate(string email, string password, bool rememberMe);

          //Event
          public event loginButtonPressedEventDelegate loginButtonPressedEvent;

          //Awake function
          public void Start()
          {
              //Set invisible
              setStatusVisibility(false);
          }

          //Called upon login button press
          public void onLoginButtonPress()
          {
              //Fire event
              if (loginButtonPressedEvent != null)
                  loginButtonPressedEvent(getEmail(), getPassword(), getRememberMe());
              else
                  Debug.LogError("No event listeners for login button pressed event. At least 1 or more listeners were expected.");
          }

          //Sets UI status message
          public void setStatusMessage(string message)
          {
              //Enable if not visible
              if (!statusMessage.transform.parent.gameObject.activeSelf)
                  setStatusVisibility(true);

              if (message.Contains("</"))
                  statusMessage.supportRichText = true;
              statusMessage.text = message;
          }

          //Sets UI status message
          public void setStatusVisibility(bool visible)
          {
              statusMessage.transform.parent.gameObject.SetActive(visible);
          }

          //Trigger fade event for container
          public void FadeAlpha(bool increasePercent, Action onCompleteMethod = null)
          {
              Action<float, bool> fadeAction = delegate (float percent, bool complete)
              {
                  setContainerAlpha(percent, complete);

                  if(complete && onCompleteMethod != null)
                      onCompleteMethod();
              };
              GlobalCoreBase.GetClientManager().StartCoroutine(FadeSystem.FadePercent(increasePercent, 500, fadeAction));
          }

          //Set container alpha to a percentage
          private void setContainerAlpha(float percent, bool complete)
          {
              if(complete)
              {
                  setContainerVisibility(percent == 1);
              } else
              {
                  containerGroup.alpha = percent;
              }
          }

          //Getters
          private string getEmail()
          {
              return emailField.text;
          }

          private string getPassword()
          {
              return passwordField.text;
          }    

          private bool getRememberMe()
          {
              return rememberMeField.isOn;
          }*/

    }

}