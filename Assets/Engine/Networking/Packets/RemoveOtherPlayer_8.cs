using Engine.Networking.Utility;
using System;

namespace Engine.Networking
{
    /// <summary>
    /// The packet that removes another nearby player from the target client
    /// </summary>
    [Serializable]
    [Packet(8, "Remove Other Player", "Client", ReliabilityScheme.RELIABLE, "Removes other player to client")]
    public class RemoveOtherPlayer_8 : Packet
    {

        /// <summary>
        /// Internal constructor
        /// </summary>
        internal RemoveOtherPlayer_8() { }       

        /// <summary>
        /// Unique id of the player
        /// </summary>
        public int uniqueId;

        /// <summary>
        /// Converts the packet value types to a byte array
        /// </summary>
        /// <returns></returns>
        public override byte[] getBytes()
        {
            byte[] bytes = null;
            PacketWriter.Add(ref bytes, uniqueId); //1-int
            return bytes;
        }

        /// <summary>
        /// Reads the byte array into the packet value types
        /// </summary>
        /// <param name="bytes">The packet bytes to read</param>
        public override void readPacket(byte[] bytes)
        {
            uniqueId = PacketReader.ReadInt(ref bytes);//1-int
        }

    }
}