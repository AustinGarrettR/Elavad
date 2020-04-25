using Engine.Networking.Utility;
using System;

namespace Engine.Networking
{
    /// <summary>
    /// The packet that tells the client the player position and rotation
    /// </summary>
    [Serializable]
    [Packet(3, "Update MyPlayer Transform", "Client", ReliabilityScheme.UNRELIABLE, "Updates the main client player transform")]
    public class UpdateMyPlayerTransform_3 : Packet
    {

        /// <summary>
        /// Internal constructor
        /// </summary>
        internal UpdateMyPlayerTransform_3() { }

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
        /// The movement speed
        /// </summary>
        public float movementSpeed;

        /// <summary>
        /// The rotation speed
        /// </summary>
        public float angularSpeed;

        /// <summary>
        /// If the update is instant
        /// </summary>
        public bool instantUpdate;

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
            PacketWriter.Add(ref bytes, movementSpeed); //8-float
            PacketWriter.Add(ref bytes, angularSpeed); //9-float
            PacketWriter.Add(ref bytes, instantUpdate); //10-bool
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
            movementSpeed = PacketReader.ReadFloat(ref bytes); //8-float
            angularSpeed = PacketReader.ReadFloat(ref bytes); //9-float
            instantUpdate = PacketReader.ReadBool(ref bytes);//10-bool
        }

    }
}