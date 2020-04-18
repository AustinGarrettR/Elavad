using Engine.Networking.Utility;
using System;

namespace Engine.Networking
{
    /// <summary>
    /// The packet that responds to the login request.
    /// </summary>
    [Serializable]
    [Packet(2, "Login Response", "Client", ReliabilityScheme.RELIABLE, "Responds to login request.")]
    public class LoginResponse_2 : Packet
    {
        /// <summary>
        /// If the request was accepted
        /// </summary>
        public bool accept;

        /// <summary>
        /// If not accepted, the error response
        /// </summary>
        public string errorResponse;

        /// <summary>
        /// Converts the packet value types to a byte array
        /// </summary>
        /// <returns></returns>
        public override byte[] getBytes()
        {
            byte[] bytes = null;
            PacketWriter.Add(ref bytes, accept); //1-bool
            PacketWriter.Add(ref bytes, errorResponse); //2-string
            return bytes;
        }

        /// <summary>
        /// Reads the byte array into the packet value types
        /// </summary>
        /// <param name="bytes">The packet bytes to read</param>
        public override void readPacket(byte[] bytes)
        {
            accept = PacketReader.ReadBool(ref bytes); //1-bool
            errorResponse = PacketReader.ReadString(ref bytes); //2-string
        }

    }
}