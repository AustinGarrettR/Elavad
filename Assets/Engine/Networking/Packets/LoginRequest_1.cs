using Engine.Networking.Utility;
using System;

namespace Engine.Networking
{
    [Serializable]
    [Packet(1, "Login Request", "Server", ReliabilityScheme.RELIABLE, "Login request to the server.")]
    public class LoginRequest_1 : Packet
    {
        //Data
        public string email;
        public string password;

        public override byte[] getBytes()
        {
            byte[] bytes = null;
            PacketWriter.Add(ref bytes, email); //1-string
            PacketWriter.Add(ref bytes, password); //2-string
            return bytes;
        }

        public override void readPacket(byte[] bytes)
        {
            email = PacketReader.ReadString(ref bytes); //1-string
            password = PacketReader.ReadString(ref bytes); //2-string
        }

    }
}