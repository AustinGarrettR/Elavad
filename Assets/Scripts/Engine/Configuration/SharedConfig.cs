using System;
using UnityEngine;

namespace Engine.Configuration
{
    public class SharedConfig
    {
        //Networking
        public static readonly byte ESCAPE = 0xBC;
        public static readonly byte DELIMITER = 0x00;
        public static readonly ushort PORT = 45955;
        public static readonly int MAX_BUFFER_SIZE = 1024;

        //Account creation
        public static readonly int MAX_USERNAME_LENGTH = 21;
        public static readonly int MAX_PASSWORD_LENGTH = 30;

        //Accounts

        //Movement
        public static readonly int POSITION_AND_ROTATION_UPDATE_TIME_IN_MILLISECONDS = 40;
    }
}