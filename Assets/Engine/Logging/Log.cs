using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Logging
{
    /// <summary>
    /// Static class to log messages and errors
    /// </summary>
    public static class Log
    {

        /*
         * Internal Variables
         */

        /// <summary>
        /// The manager that we forward messages and errors
        /// </summary>
        private static LogManager logManager;

        /*
         * Public Functions
         */

        /// <summary>
        /// Sets the log manager by the engine to be used
        /// </summary>
        /// <param name="manager">The log manager</param>
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
        /// <param name="error">The error message.</param>
        public static void LogError(string error)
        {
            logManager.LogError(error);
        }
    }
}