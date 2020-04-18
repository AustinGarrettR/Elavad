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

        /// <summary>
        /// Packet attribute for holding properties about the packet
        /// </summary>
        /// <param name="packetId">The packet id</param>
        /// <param name="name">The packet name</param>
        /// <param name="target">Server or client target</param>
        /// <param name="reliabilityScheme">Reliable or unreliable</param>
        /// <param name="description">The packet description</param>
        public PacketAttribute(int packetId, string name, string target, ReliabilityScheme reliabilityScheme, string description)
        {
            this.packetId = packetId;
            this.name = name;
            this.target = target;
            this.reliabilityScheme = reliabilityScheme;
            this.description = description;
        }

        /// <summary>
        /// Read only packet ID
        /// </summary>
        public virtual int PacketId
        {
            get { return packetId; }
        }

        /// <summary>
        /// Read only packet name
        /// </summary>
        public virtual string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Read only packet target
        /// </summary>
        public virtual string Target
        {
            get { return target; }
        }

        /// <summary>
        /// Read only packet reliability scheme
        /// </summary>
        public virtual ReliabilityScheme ReliabilityScheme
        {
            get { return reliabilityScheme; }
        }

        
        /// <summary>
        /// Read only packet description
        /// </summary>
        public virtual string Description
        {
            get { return description; }
        }

    }
}