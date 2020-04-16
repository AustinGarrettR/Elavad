using UnityEngine;
using Engine.Factory;

namespace Engine.Logging
{
    public class LogManager : Manager
    {

        /*
         * Override Functions
         */

        public override void init(params object[] parameters)
        {
            Log.setLogManager(this);
        }

        public override void update()
        {
           
        }

        public override void shutdown()
        {
            
        }

        /*
         * Internal Variables
         */

        private LogLevel logLevel = LogLevel.ALL;

        /*
         * Public Functions
         */

        internal LogLevel getLogLevel()
        {
            return logLevel;
        } 

        internal void SetLogLevel(LogLevel level)
        {
            logLevel = level;
        }

        //Logs a message
        internal void LogMsg(string message)
        {
            Debug.Log(message);

            if(getLogLevel() == LogLevel.ALL)
            {
                //TODO
            }
        }

        //Logs an error
        internal void LogError(string error)
        {
            if (getLogLevel() == LogLevel.ERRORS || getLogLevel() == LogLevel.ALL)
            {
                //TODO
            }

            throw new System.Exception(error);
        }

    }
    public enum LogLevel
    {
        NONE,
        ERRORS,
        ALL
    }
}