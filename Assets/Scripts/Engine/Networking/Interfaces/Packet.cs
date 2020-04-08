using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Networking
{
    [Packet(-1, "Undefined Packet", "Undefined", ReliabilityScheme.RELIABLE, "Undefined")]
    public abstract class Packet
    {
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

        public abstract byte[] getBytes();
        public abstract void readPacket(byte[] bytes);

    }
}
