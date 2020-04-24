using Engine.Networking.Utility;
using System;

namespace Engine.Networking
{
    /// <summary>
    /// The packet that tells the client the player position
    /// </summary>
    [Serializable]
    [Packet(3, "Update My Player Position", "Client", ReliabilityScheme.UNRELIABLE, "Updates the main client player position")]
    public class UpdateMyPlayerPosition_3 : Packet
    {
        /// <summary>
        /// The x coordinate
        /// </summary>
        public float x;

        /// <summary>
        /// The y coordinate
        /// </summary>
        public float y;

        /// <summary>
        /// The z coordinate
        /// </summary>
        public float z;

        /// <summary>
        /// Converts the packet value types to a byte array
        /// </summary>
        /// <returns></returns>
        public override byte[] getBytes()
        {
            byte[] bytes = null;
            PacketWriter.Add(ref bytes, x); //1-float
            PacketWriter.Add(ref bytes, y); //2-float
            PacketWriter.Add(ref bytes, z); //3-float
            return bytes;
        }

        /// <summary>
        /// Reads the byte array into the packet value types
        /// </summary>
        /// <param name="bytes">The packet bytes to read</param>
        public override void readPacket(byte[] bytes)
        {
            x = PacketReader.ReadFloat(ref bytes); //1-float
            y = PacketReader.ReadFloat(ref bytes); //2-float
            z = PacketReader.ReadFloat(ref bytes); //3-float
        }

    }
}