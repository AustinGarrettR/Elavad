﻿using System;
using System.Collections.Generic;

namespace Engine.Networking.Utility
{
    /// <summary>
    /// Utility method to turn a stream of bytes into a value type
    /// </summary>
    public class PacketReader
    {

        /// <summary>
        /// Reads the next integer in a byte stream
        /// </summary>
        /// <param name="packetBytes">The byte array</param>
        /// <returns></returns>
        public static int ReadInt(ref byte[] packetBytes)
        {
            byte[] array = packetBytes;
            Array.Copy(packetBytes, 0, array, 0, 4);
            int @int = ByteConverter.getInt(array);
            int num = 4;
            int num2 = packetBytes.Length - num;
            byte[] destinationArray = new byte[num2];
            Array.Copy(packetBytes, num, destinationArray, 0, num2);
            packetBytes = destinationArray;
            return @int;
        }

        /// <summary>
        /// Reads the next short in a byte stream
        /// </summary>
        /// <param name="packetBytes">The byte array</param>
        /// <returns></returns>
        public static short ReadShort(ref byte[] packetBytes)
        {
            byte[] array = packetBytes;
            Array.Copy(packetBytes, 0, array, 0, 2);
            short @short = ByteConverter.getShort(array);
            int num = 2;
            int num2 = packetBytes.Length - num;
            byte[] destinationArray = new byte[num2];
            Array.Copy(packetBytes, num, destinationArray, 0, num2);
            packetBytes = destinationArray;
            return @short;
        }

        /// <summary>
        /// Reads the next bool in a byte stream
        /// </summary>
        /// <param name="packetBytes">The byte array</param>
        /// <returns></returns>
        public static bool ReadBool(ref byte[] packetBytes)
        {
            byte[] array = packetBytes;
            Array.Copy(packetBytes, 0, array, 0, 1);
            bool @bool = ByteConverter.getBool(array);
            int num = 1;
            int num2 = packetBytes.Length - num;
            byte[] destinationArray = new byte[num2];
            Array.Copy(packetBytes, num, destinationArray, 0, num2);
            packetBytes = destinationArray;
            return @bool;
        }

        /// <summary>
        /// Reads the next long in a byte stream
        /// </summary>
        /// <param name="packetBytes">The byte array</param>
        /// <returns></returns>
        public static long ReadLong(ref byte[] packetBytes)
        {
            byte[] array = packetBytes;
            Array.Copy(packetBytes, 0, array, 0, 8);
            long @long = ByteConverter.getLong(array);
            short num = 8;
            int num2 = packetBytes.Length - num;
            byte[] destinationArray = new byte[num2];
            Array.Copy(packetBytes, num, destinationArray, 0, num2);
            packetBytes = destinationArray;
            return @long;
        }

        /// <summary>
        /// Reads the next float in a byte stream
        /// </summary>
        /// <param name="packetBytes">The byte array</param>
        /// <returns></returns>
        public static float ReadFloat(ref byte[] packetBytes)
        {
            byte[] array = packetBytes;
            Array.Copy(packetBytes, 0, array, 0, 4);
            float @float = ByteConverter.getFloat(array);
            int num = 4;
            int num2 = packetBytes.Length - num;
            byte[] destinationArray = new byte[num2];
            Array.Copy(packetBytes, num, destinationArray, 0, num2);
            packetBytes = destinationArray;
            return @float;
        }

        /// <summary>
        /// Reads the next string in a byte stream
        /// </summary>
        /// <param name="packetBytes">The byte array</param>
        /// <returns></returns>
        public static string ReadString(ref byte[] packetBytes)
        {
            int num = PacketReader.ReadInt(ref packetBytes);
            List<byte> list = new List<byte>(packetBytes);
            byte[] bytes = list.GetRange(0, num).ToArray();
            string @string = ByteConverter.getString(bytes);
            int num2 = num;
            int num3 = packetBytes.Length - num2;
            byte[] destinationArray = new byte[num3];
            Array.Copy(packetBytes, num2, destinationArray, 0, num3);
            packetBytes = destinationArray;
            return @string;
        }

        /// <summary>
        /// Reads an embedded byte array in a byte stream
        /// </summary>
        /// <param name="packetBytes">The byte array</param>
        /// <returns></returns>
        public static byte[] ReadByteArray(ref byte[] packetBytes)
        {
            int num = PacketReader.ReadInt(ref packetBytes);
            List<byte> list = new List<byte>(packetBytes);
            byte[] bytes = list.GetRange(0, num).ToArray();
            byte[] bytearray = bytes;
            int num2 = num;
            int num3 = packetBytes.Length - num2;
            byte[] destinationArray = new byte[num3];
            Array.Copy(packetBytes, num2, destinationArray, 0, num3);
            packetBytes = destinationArray;
            return bytearray;
        }
    }
}