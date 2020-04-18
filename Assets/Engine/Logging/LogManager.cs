using UnityEngine;
using Engine.Factory;

namespace Engine.Logging
{
    /// <summary>
    /// Manages log messages and errors
    /// </summary>
    public class LogManager : Manager
    {

        /*
         * Override Functions
         */

        /// <summary>
        /// Called on initialization
        /// </summary>
        /// <param name="parameters">No parameters used</param>
        public override void init(params object[] parameters)
        {
            Log.setLogManager(this);
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        public override void update()
        {
           
        }

        /// <summary>
        /// Called on shutdown
        /// </summary>
        public override void shutdown()
        {
            
        }

        /*
         * Internal Variables
         */

        /// <summary>
        /// What level of messages to show
        /// </summary>
        private LogLevel logLevel = LogLevel.ALL;

        /*
         * Public Functions
         */

        /// <summary>
        /// Returns the log level
        /// </summary>
        /// <returns>LogLevel enum</returns>
        public LogLevel getLogLevel()
        {
            return logLevel;
        } 

        /// <summary>
        /// Set the log level
        /// </summary>
        /// <param name="level">the LogLevel enum value</param>
        public void SetLogLevel(LogLevel level)
        {
            logLevel = level;
        }

        /// <summary>
        /// Logs a message from the API
        /// </summary>
        /// <param name="message">The message to log</param>
        internal void LogMsg(string message)
        {
            Debug.Log(message);

            if(getLogLevel() == LogLevel.ALL)
            {
                //TODO
            }
        }

        /// <summary>
        /// Logs an error from the API
        /// </summary>
        /// <param name="error">The error message</param>
        internal void LogError(string error)
        {
            if (getLogLevel() == LogLevel.ERRORS || getLogLevel() == LogLevel.ALL)
            {
                //TODO
            }

            throw new System.Exception(error);
        }

    }

    /// <summary>
    /// The level of messages to show
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Show no messages
        /// </summary>
        NONE,

        /// <summary>
        /// Show only errors
        /// </summary>
        ERRORS,

        /// <summary>
        /// Show all messages and errors
        /// </summary>
        ALL
    }
}