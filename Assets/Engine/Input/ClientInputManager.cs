using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using Engine.Factory;

namespace Engine.Input
{
    /// <summary>
    /// Handles input
    /// </summary>
    public class ClientInputManager : Manager
    {
        /*
         * Override Methods
         */

        /// <summary>
        /// Initialize method
        /// </summary>
        public override void Init()
        {

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

        /// <summary>
        /// Delegate for mouse click events
        /// </summary>
        /// <param name="mouseButton">The button clicked on the mouse</param>
        internal delegate void NotifyOnClickDelegate(MouseButton mouseButton);
        
        /// <summary>
        /// Delegate for a key press
        /// </summary>
        /// <param name="key">The key that was pressed</param>
        internal delegate void NotifyOnKeypressDelegate(Key key);

        /// <summary>
        /// Event for the mouse click delegate
        /// </summary>
        internal event NotifyOnClickDelegate NotifyOnMouseClickEvent;

        /// <summary>
        /// Event for the key press delegate
        /// </summary>
        internal event NotifyOnKeypressDelegate NotifyOnKeypressEvent;

    }
}
