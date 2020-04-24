using Engine.Networking.Utility;
using System;

namespace Engine.Networking
{
    /// <summary>
    /// Informs the server the client finished loading
    /// </summary>
    [Serializable]
    [Packet(4, "Finished Loading", "Server", ReliabilityScheme.RELIABLE, "Informs the server the client finished loading")]
    public class FinishedLoading_4 : Packet
    {        
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