﻿using UnityEngine;

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
        internal ClientPlayer(string name, GameObject prefab, Vector3 startPosition, Quaternion startRotation)
        {
            playerObject = GameObject.Instantiate(prefab);
            playerObject.name = name;
            playerObject.transform.position = startPosition;
            playerObject.transform.rotation = startRotation;
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
        /// The current transform update position
        /// </summary>
        internal Vector3 targetTransformUpdatePosition;

        /// <summary>
        /// The current transform update rotation
        /// </summary>
        internal Quaternion targetTransformUpdateRotation;

        /// <summary>
        /// The speed in units per second the player moves
        /// </summary>
        internal float movementSpeed;

        /// <summary>
        /// The speed in degrees per second the player rotates
        /// </summary>
        internal float angularSpeed;

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