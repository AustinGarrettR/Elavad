using System;
using System.Text;

namespace Engine.Networking.Utility
{
    /// <summary>
    /// Handles converting value types to bytes
    /// </summary>
    public class ByteConverter
    {
        /// <summary>
        /// Returns the bytes from a value type
        /// </summary>
        /// <typeparam name="T">The type of object</typeparam>
        /// <param name="o">The object</param>
        /// <returns></returns>
        public static byte[] getBytes<T>(Object o)
        {
            Type t = typeof(T);
            if (t.IsInstanceOfType(typeof(bool)) || t.IsAssignableFrom(typeof(bool)))
                return getBytes((bool)o);
            if (t.IsAssignableFrom(typeof(int)))
                return getBytes((int)o);
            if (t.IsAssignableFrom(typeof(short)))
                return getBytes((short)o);
            if (t.IsAssignableFrom(typeof(float)))
                return getBytes((float)o);
            if (t.IsAssignableFrom(typeof(long)))
                return getBytes((long)o);
            if (t.IsAssignableFrom(typeof(string)))
                return getBytes((string)o);
            if (t.IsAssignableFrom(typeof(byte[])))
                return (byte[])o;
            else
                throw new Exception("Unsupported value type. Type Name:" + t.Name);

        }
        private static byte[] getBytes(string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }
        private static byte[] getBytes(bool b)
        {
            return BitConverter.GetBytes(b);
        }
        private static byte[] getBytes(int I32)
        {
            return BitConverter.GetBytes(I32);
        }
        private static byte[] getBytes(short s)
        {
            return BitConverter.GetBytes(s);
        }
        private static byte[] getBytes(long l)
        {
            return BitConverter.GetBytes(l);
        }
        private static byte[] getBytes(float f)
        {
            return BitConverter.GetBytes(f);
        }

        /// <summary>
        /// Returns a string from a byte array
        /// </summary>
        /// <param name="bytes">The byte array</param>
        /// <returns></returns>
        public static string getString(byte[] bytes)
        {
            return new string(Encoding.UTF8.GetChars(bytes));
        }

        /// <summary>
        /// Returns an int from a byte array
        /// </summary>
        /// <param name="b">The byte array</param>
        /// <returns></returns>
        public static int getInt(byte[] b)
        {
            return BitConverter.ToInt32(b, 0);
        }

        /// <summary>
        /// Returns a long from a byte array
        /// </summary>
        /// <param name="b">The byte array</param>
        /// <returns></returns>
        public static long getLong(byte[] b)
        {
            return BitConverter.ToInt64(b, 0);
        }

        /// <summary>
        /// Returns a short from a byte array
        /// </summary>
        /// <param name="b">The byte array</param>
        /// <returns></returns>
        public static short getShort(byte[] b)
        {
            return BitConverter.ToInt16(b, 0);
        }

        /// <summary>
        /// Returns a float from a byte array
        /// </summary>
        /// <param name="b">The byte array</param>
        /// <returns></returns>
        public static float getFloat(byte[] b)
        {
            return BitConverter.ToSingle(b, 0);
        }

        /// <summary>
        /// Returns a bool from a byte array
        /// </summary>
        /// <param name="b">The byte array</param>
        /// <returns></returns>
        public static bool getBool(byte[] b)
        {
            return BitConverter.ToBoolean(b, 0);
        }
    }
}