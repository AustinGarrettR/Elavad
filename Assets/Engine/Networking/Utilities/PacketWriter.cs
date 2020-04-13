using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Engine.Logging;

namespace Engine.Networking.Utility
{
    public class PacketWriter
    {

        //Converts object to byte array, limited to specific primitives
        public static void Add<T>(ref byte[] packetBytes, T data)
        {
            if (data is int || data is short || data is float || data is long || data is bool || data is byte[] || data is string)
            {
                byte[] bytes = ByteConverter.getBytes<T>(data);
                if (packetBytes == null)
                {
                    packetBytes = new byte[0];
                }

                List<byte> list = new List<byte>(packetBytes);
                List<byte> collection = new List<byte>(bytes);

                if (data is byte[] || data is string)
                {
                    int length = bytes.Length;
                    list.AddRange(ByteConverter.getBytes<int>(length));
                }

                list.AddRange(collection);
                packetBytes = list.ToArray();

                //Cleanup
                list = null;
                collection = null;

            }
            else
            {
                Log.LogError("Object type is unsupported and can not be serialized. Type:" + typeof(T).Name);
            }

        }

    }
}
