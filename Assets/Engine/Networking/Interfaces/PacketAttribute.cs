using System;

namespace Engine.Networking
{
    /// <summary>
    /// Used to assign properties to a packet
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class PacketAttribute : Attribute
    {
        // Private fields.
        private int packetId;
        private string name;
        private string target;
        private ReliabilityScheme reliabilityScheme;
        private string description;

        public PacketAttribute(int packetId, string name, string target, ReliabilityScheme reliabilityScheme, string description)
        {
            this.packetId = packetId;
            this.name = name;
            this.target = target;
            this.reliabilityScheme = reliabilityScheme;
            this.description = description;
        }

        // This is a read-only attribute.

        public virtual int PacketId
        {
            get { return packetId; }
        }

        // This is a read-only attribute.

        public virtual string Name
        {
            get { return name; }
        }

        // This is a read-only attribute.

        public virtual string Target
        {
            get { return target; }
        }

        // This is a read-only attribute.
        public virtual ReliabilityScheme ReliabilityScheme
        {
            get { return reliabilityScheme; }
        }

        // This is a read-only attribute.

        public virtual string Description
        {
            get { return description; }
        }

    }
}