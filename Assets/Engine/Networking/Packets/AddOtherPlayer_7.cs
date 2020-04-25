using Engine.Networking.Utility;
using System;

namespace Engine.Networking
{
    /// <summary>
    /// The packet that adds another nearby player to the target client
    /// </summary>
    [Serializable]
    [Packet(7, "Add Other Player", "Client", ReliabilityScheme.RELIABLE, "Adds other player to client")]
    public class AddOtherPlayer_7 : Packet
    {

        /// <summary>
        /// Internal constructor
        /// </summary>
        internal AddOtherPlayer_7() { }

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
        /// The rotation x axis
        /// </summary>
        public float rotationX;

        /// <summary>
        /// The rotation y axis
        /// </summary>
        public float rotationY;

        /// <summary>
        /// The rotation z axis
        /// </summary>
        public float rotationZ;

        /// <summary>
        /// The rotation w axis
        /// </summary>
        public float rotationW;

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
            PacketWriter.Add(ref bytes, x); //1-float
            PacketWriter.Add(ref bytes, y); //2-float
            PacketWriter.Add(ref bytes, z); //3-float
            PacketWriter.Add(ref bytes, rotationX); //4-float
            PacketWriter.Add(ref bytes, rotationY); //5-float
            PacketWriter.Add(ref bytes, rotationZ); //6-float
            PacketWriter.Add(ref bytes, rotationW); //7-float
            PacketWriter.Add(ref bytes, uniqueId); //8-int
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
            rotationX = PacketReader.ReadFloat(ref bytes); //4-float
            rotationY = PacketReader.ReadFloat(ref bytes); //5-float
            rotationZ = PacketReader.ReadFloat(ref bytes); //6-float
            rotationW = PacketReader.ReadFloat(ref bytes); //7-float
            uniqueId = PacketReader.ReadInt(ref bytes);//8-int
        }

    }
}