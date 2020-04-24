using UnityEngine;

namespace Engine.Player
{
    /// <summary>
    /// This class represents a player instance
    /// </summary>
    public class ClientPlayer
    {
        /*
         * Constructor
         */

        /// <summary>
        /// Construct the player instance
        /// </summary>
        /// <param name="prefab">The player prefab</param>
        /// <param name="startPosition">The starting position</param>
        /// <param name="startRotation">The starting rotation</param>
        internal ClientPlayer(string name, GameObject prefab, Vector3 startPosition, Vector3 startRotation)
        {
            playerObject = GameObject.Instantiate(prefab);
            playerObject.name = name;
            playerObject.transform.position = startPosition;
            playerObject.transform.rotation = Quaternion.Euler(startRotation);
        }

        /*
         * Private
         */

        /// <summary>
        /// The player game object
        /// </summary>
        private GameObject playerObject;

        /*
         * Internal
         */

        /// <summary>
        /// The last time a transform update was received
        /// </summary>
        internal long lastTransformUpdateTimestamp;

        /// <summary>
        /// The last transform update position
        /// </summary>
        internal Vector3 lastTransformUpdatePosition;

        /// <summary>
        /// The current transform update position
        /// </summary>
        internal Vector3 currentTransformUpdatePosition;

        /// <summary>
        /// The last transform update rotation
        /// </summary>
        internal Quaternion lastTransformUpdateRotation;

        /// <summary>
        /// The current transform update rotation
        /// </summary>
        internal Quaternion currentTransformUpdateRotation;

        /// <summary>
        /// Whether the position has been set at least once
        /// </summary>
        internal bool positionSet;

        /*
         * Public 
         */

        /// <summary>
        /// Returns the player object
        /// </summary>
        /// <returns></returns>
        public GameObject GetPlayerObject()
        {
            return playerObject;
        }

    }
}