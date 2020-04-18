using Engine.Networking.Utility;
using System;

namespace Engine.Networking
{
    /// <summary>
    /// The login request to the server packet
    /// </summary>
    [Serializable]
    [Packet(1, "Login Request", "Server", ReliabilityScheme.RELIABLE, "Login request to the server.")]
    public class LoginRequest_1 : Packet
    {
        /// <summary>
        /// The input email
        /// </summary>
        public string email;

        /// <summary>
        /// The input password
        /// </summary>
        public string password;

        /// <summary>
        /// Converts the data to a byte array
        /// </summary>
        /// <returns></returns>
        public override byte[] getBytes()
        {
            byte[] bytes = null;
            PacketWriter.Add(ref bytes, email); //1-string
            PacketWriter.Add(ref bytes, password); //2-string
            return bytes;
        }

        /// <summary>
        /// Converts a byte array to the packet value types
        /// </summary>
        /// <param name="bytes"></param>
        public override void readPacket(byte[] bytes)
        {
            email = PacketReader.ReadString(ref bytes); //1-string
            password = PacketReader.ReadString(ref bytes); //2-string
        }

    }
}