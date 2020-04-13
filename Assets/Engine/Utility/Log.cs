using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Logging
{
    public static class Log
    {

        /*
         * Internal Variables
         */

        private static LogManager logManager;

        /*
         * Public Functions
         */

        //Sets the log manager by the engine to be used
        internal static void setLogManager(LogManager manager)
        {
            logManager = manager;
        }

        /*
         * API
         */

        /// <summary>Logs a message to console</summary>
        /// <param name="message">The message.</param>
        public static void LogMsg(string message)
        {
            logManager.LogMsg(message);
        }

        /// <summary>Logs an error to console</summary>
        /// <param name="message">The error message.</param>
        public static void LogError(string error)
        {
            logManager.LogError(error);
        }
    }
}