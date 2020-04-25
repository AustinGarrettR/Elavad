using System;

namespace Engine.Networking
{
    /// <summary>
    /// Informs the server the client finished loading
    /// </summary>
    [Serializable]
    [Packet(0, "Keep Alive", "Server", ReliabilityScheme.RELIABLE, "Pings to keep the connection alive.")]
    public class KeepAlive_0 : Packet
    {

        /// <summary>
        /// Internal constructor
        /// </summary>
        internal KeepAlive_0() { }

        /// <summary>
        /// Converts the data to a byte array
        /// </summary>
        /// <returns></returns>
        public override byte[] getBytes()
        {
            //Empty packet
            byte[] bytes = new byte[0];
            return bytes;
        }

        /// <summary>
        /// Converts a byte array to the packet value types
        /// </summary>
        /// <param name="bytes"></param>
        public override void readPacket(byte[] bytes)
        {
            //Empty Packet, don't do anything
        }

    }
}