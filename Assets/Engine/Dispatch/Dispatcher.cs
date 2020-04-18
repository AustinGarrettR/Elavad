using System;
using System.Collections.Generic;

namespace Engine.Dispatch
{
    /// <summary>
    /// Used to dispatch events on the main thread
    /// </summary>
    public static class Dispatcher
    {

        /*
         * Internal Variables
         */

        private static readonly List<Action> pending = new List<Action>();

        /*
         * Public Variables
         */

        /// <summary>
        /// Invoke an action on the main thread.
        /// </summary>
        /// <param name="action"></param>
        public static void Invoke(Action action)
        {
            lock (pending)
            {
                pending.Add(action);
            }
        }

        /*
         * Internal Functions
         */

        private static void InvokePending()
        {
            lock (pending)
            {
                foreach (Action action in pending)
                {
                    action();
                }

                pending.Clear();
            }
        }

        internal static void Update()
        {
            InvokePending();
        }
    }
}