using System;

namespace Engine.Configuration
{
    /// <summary>
    /// Constant values for both the client and server
    /// </summary>
    public class SharedConfig
    {
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

        /// <summary>
        /// Max length for usernames
        /// </summary>
        public static readonly int MAX_USERNAME_LENGTH = 35;

        /// <summary>
        /// Max length for passwords
        /// </summary>
        public static readonly int MAX_PASSWORD_LENGTH = 35;

    }
}