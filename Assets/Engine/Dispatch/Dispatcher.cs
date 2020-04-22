using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        /// <summary>
        /// Invoke any actions queue'd up
        /// </summary>
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

        /// <summary>
        /// Called every frame by a manager
        /// </summary>
        internal static void Update()
        {
            InvokePending();
        }
    }
}