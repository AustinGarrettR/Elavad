﻿using UnityEngine;

namespace Engine.Player
{
    /// <summary>
    /// This class represents a player instance
    /// </summary>
    public class Player
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
        internal Player(string name, GameObject prefab, Vector3 startPosition, Vector3 startRotation)
        {
            playerObject = GameObject.Instantiate(prefab);
            playerObject.name = name;
            playerObject.transform.position = startPosition;
            playerObject.transform.rotation = Quaternion.Euler(startRotation);
        }

        /*
         * Internal
         */

        /// <summary>
        /// The player game object
        /// </summary>
        private GameObject playerObject;


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