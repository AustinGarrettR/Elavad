namespace Engine.Configuration
{
    /// <summary>
    /// Constant values for both the client and server
    /// </summary>
    public class SharedConfig
    {

        /*
         * Networking
         */

        /// <summary>
        /// The byte sequence to signify escaping the next byte
        /// </summary>
        public static readonly byte ESCAPE = 0xBC;
        
        /// <summary>
        /// The byte sequence to signify the end of a packet
        /// </summary>
        public static readonly byte DELIMITER = 0x00;

        /// <summary>
        /// The server port
        /// </summary>
        public static readonly ushort PORT = 45955;

        /// <summary>
        /// Max size for the network buffer
        /// </summary>
        public static readonly int MAX_BUFFER_SIZE = 1024;

        /*
         * Account
         */

        /// <summary>
        /// Max length for usernames
        /// </summary>
        public static readonly int MAX_USERNAME_LENGTH = 35;

        /// <summary>
        /// Max length for passwords
        /// </summary>
        public static readonly int MAX_PASSWORD_LENGTH = 35;

        /*
         * World
         */

        /// <summary>
        /// The size of the world chunks
        /// </summary>
        public static readonly int WORLD_CHUNK_SIZE = 250;

        /// <summary>
        /// How close to a chunk position you need to be for it to load
        /// </summary>
        public static readonly int CHUNK_LOAD_VIEW_DISTANCE = 500;

        /// <summary>
        /// How far from a chunk position before it unloads
        /// </summary>
        public static readonly int CHUNK_UNLOAD_VIEW_DISTANCE = 600;

        /*
         * Scenes
         */

        /// <summary>
        /// Location of the main scene
        /// </summary>
        public static readonly string MAIN_SCENE_PATH = "Assets/Scenes/Engine/Main.unity";

        /// <summary>
        /// Location of the login scene
        /// </summary>
        public static readonly string LOGIN_SCENE_PATH = "Assets/Scenes/Engine/Login.unity";

        /// <summary>
        /// Location of the world scenes
        /// </summary>
        public static readonly string WORLD_SCENES_FOLDER = "Assets/Scenes/World";


    }
}