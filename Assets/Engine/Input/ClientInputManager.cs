using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

namespace Engine.Input
{
    public class ClientInputManager : Manager
    {
        /*
         * Override Methods
         */

        //Initialize method
        internal override void init(params object[] parameters)
        {

        }

        //Called on program shutdown
        internal override void shutdown()
        {

        }

        //Called every frame
        internal override void update()
        {           

            Mouse mouse = Mouse.current;
            Keyboard keyboard = Keyboard.current;

            //Check mouse for input
            if (NotifyOnMouseClickEvent != null)
            {
                if (mouse.leftButton.wasReleasedThisFrame)
                    NotifyOnMouseClickEvent.Invoke(MouseButton.Left);
                if (mouse.rightButton.wasReleasedThisFrame)
                    NotifyOnMouseClickEvent.Invoke(MouseButton.Right);
                if (mouse.middleButton.wasReleasedThisFrame)
                    NotifyOnMouseClickEvent.Invoke(MouseButton.Middle);
            }

            //Check for keyboard input
            if (NotifyOnKeypressEvent != null)
            {
                foreach (KeyControl keyControl in keyboard.allKeys)
                {
                    if (keyControl.wasReleasedThisFrame)
                        NotifyOnKeypressEvent.Invoke(keyControl.keyCode);

                }
            }

        }

        /*
         * Internal Variables
         */

        //Delegates
        internal delegate void NotifyOnClickDelegate(MouseButton mouseButton);
        internal delegate void NotifyOnKeypressDelegate(Key key);

        //Events
        internal event NotifyOnClickDelegate NotifyOnMouseClickEvent;
        internal event NotifyOnKeypressDelegate NotifyOnKeypressEvent;

    }
}
