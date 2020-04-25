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
        /// Internal constructor
        /// </summary>
        internal LoginResponse_2() { }

        /// <summary>
        /// If the request was accepted
        /// </summary>
        public bool accept;

        /// <summary>
        /// If not accepted, the error response
        /// </summary>
        public string errorResponse;

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
        /// Converts the packet value types to a byte array
        /// </summary>
        /// <returns></returns>
        public override byte[] getBytes()
        {
            byte[] bytes = null;
            PacketWriter.Add(ref bytes, accept); //1-bool
            PacketWriter.Add(ref bytes, errorResponse); //2-string
            PacketWriter.Add(ref bytes, x); //3-float
            PacketWriter.Add(ref bytes, y); //4-float
            PacketWriter.Add(ref bytes, z); //5-float
            PacketWriter.Add(ref bytes, rotationX); //6-float
            PacketWriter.Add(ref bytes, rotationY); //7-float
            PacketWriter.Add(ref bytes, rotationZ); //8-float
            PacketWriter.Add(ref bytes, rotationW); //9-float
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
            x = PacketReader.ReadFloat(ref bytes); //3-float
            y = PacketReader.ReadFloat(ref bytes); //4-float
            z = PacketReader.ReadFloat(ref bytes); //5-float
            rotationX = PacketReader.ReadFloat(ref bytes); //6-float
            rotationY = PacketReader.ReadFloat(ref bytes); //7-float
            rotationZ = PacketReader.ReadFloat(ref bytes); //8-float
            rotationW = PacketReader.ReadFloat(ref bytes); //9-float
        }

    }
}