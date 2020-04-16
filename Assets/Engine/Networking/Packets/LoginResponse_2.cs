using Engine.Networking.Utility;
using System;

namespace Engine.Networking
{
    [Serializable]
    [Packet(2, "Login Response", "Client", ReliabilityScheme.RELIABLE, "Responds to login request.")]
    public class LoginResponse_2 : Packet
    {
        //Data
        public bool accept;
        public string errorResponse;

        public override byte[] getBytes()
        {
            byte[] bytes = null;
            PacketWriter.Add(ref bytes, accept); //1-bool
            PacketWriter.Add(ref bytes, errorResponse); //2-string
            return bytes;
        }

        public override void readPacket(byte[] bytes)
        {
            accept = PacketReader.ReadBool(ref bytes); //1-bool
            errorResponse = PacketReader.ReadString(ref bytes); //2-string
        }

    }
}