using UnityEngine;

namespace Engine.Loading
{
    /// <summary>
    /// A chunk of data sent from the server to be used in loading the game
    /// </summary>
    public class ClientLoadData
    {
        /// <summary>
        /// The start position of the character
        /// </summary>
        public Vector3 startPosition;

        /// <summary>
        /// The start rotation of the character
        /// </summary>
        public Quaternion startRotation;
    }
}
