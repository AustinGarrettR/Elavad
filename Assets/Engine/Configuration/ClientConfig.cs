namespace Engine.Configuration
{
    /// <summary>
    /// Constant values for the client
    /// </summary>
    public class ClientConfig
    {
        /*
        * Environment
        */

        /// <summary>
        /// The speed the sky transitions when changing environments
        /// </summary>
        public static readonly float ENVIRONMENT_TRANSITION_SPEED = 1.0f;

        /*
         * Camera Constants
         */

        /// <summary>
        /// The camera rotation for the x axis
        /// </summary>
        public static readonly float CAMERA_Y_OFFSET = 0.5f;

        /// <summary>
        /// The camera distance minimum
        /// </summary>
        public static readonly float CAMERA_DISTANCE_MINIMUM = 3;

        /// <summary>
        /// The camera distance maximum
        /// </summary>
        public static readonly float CAMERA_DISTANCE_MAXIMUM = 20;

        /// <summary>
        /// The camera  y minimum limit
        /// </summary>
        public static readonly float CAMERA_Y_AXIS_MINIMUM = 5;

        /// <summary>
        /// The camera  y maximum limit
        /// </summary>
        public static readonly float CAMERA_Y_AXIS_MAXIMUM = 90;

        /*
         * Camera Defaults
         */

        /// <summary>
        /// The default camera rotation speed for the x axis
        /// </summary>
        public static readonly float CAMERA_DEFAULT_X_AXIS_ROTATION_SPEED = 1.25f;

        /// <summary>
        /// The default camera rotation speed for the y axis
        /// </summary>
        public static readonly float CAMERA_DEFAULT_Y_AXIS_ROTATION_SPEED = 1.0f;

        /// <summary>
        /// The default camera rotation for the x axis
        /// </summary>
        public static readonly float CAMERA_DEFAULT_X_AXIS_ROTATION = 0;

        /// <summary>
        /// The default camera rotation for the y axis
        /// </summary>
        public static readonly float CAMERA_DEFAULT_Y_AXIS_ROTATION = 25;

        /// <summary>
        /// The default camera distance
        /// </summary>
        public static readonly float CAMERA_DEFAULT_DISTANCE = 8;

        /// <summary>
        /// The default setting for the zamera zoom when an object interceps
        /// </summary>
        public static readonly bool CAMERA_DEFAULT_ZOOM_WHEN_INTERCEPTED = true;

    }
}