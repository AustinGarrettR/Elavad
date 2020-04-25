using System;

namespace Engine.Networking
{
    /// <summary>
    /// Base class for packets
    /// </summary>
    [Packet(-1, "Undefined Packet", "Undefined", ReliabilityScheme.RELIABLE, "Undefined")]
    public abstract class Packet
    {

        /// <summary>
        /// The unique ID for the packet
        /// </summary>
        public int packetId
        {
            get
            {
                Type t = this.GetType();
                PacketAttribute MyAttribute =
                    (PacketAttribute)System.Attribute.GetCustomAttribute(t, typeof(PacketAttribute));

                return MyAttribute.PacketId;

            }
        }

        /// <summary>
        /// The name of the packet
        /// </summary>
        public string packetName
        {
            get
            {
                Type t = this.GetType();
                PacketAttribute MyAttribute =
                    (PacketAttribute)System.Attribute.GetCustomAttribute(t, typeof(PacketAttribute));

                return MyAttribute.Name;

            }
        }

        /// <summary>
        /// The target of the packet
        /// </summary>
        public string packetTarget
        {
            get
            {
                Type t = this.GetType();
                PacketAttribute MyAttribute =
                    (PacketAttribute)System.Attribute.GetCustomAttribute(t, typeof(PacketAttribute));

                return MyAttribute.Target;

            }
        }

        /// <summary>
        /// The reliability scheme of the packet
        /// </summary>
        public ReliabilityScheme packetReliabilityScheme
        {
            get
            {
                Type t = this.GetType();
                PacketAttribute MyAttribute =
                    (PacketAttribute)System.Attribute.GetCustomAttribute(t, typeof(PacketAttribute));

                return MyAttribute.ReliabilityScheme;

            }
        }

        /// <summary>
        /// The packet description
        /// </summary>
        public string packetDescription
        {
            get
            {
                Type t = this.GetType();
                PacketAttribute MyAttribute =
                    (PacketAttribute)System.Attribute.GetCustomAttribute(t, typeof(PacketAttribute));

                return MyAttribute.Description;

            }
        }

        /// <summary>
        /// Return the bytes of the packet
        /// </summary>
        /// <returns></returns>
        public abstract byte[] getBytes();

        /// <summary>
        /// Read the packet from the bytes
        /// </summary>
        /// <param name="bytes">The packet bytes</param>
        public abstract void readPacket(byte[] bytes);

    }
}
