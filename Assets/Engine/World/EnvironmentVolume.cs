using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Engine.World
{
    /// <summary>
    /// Applies a specific environment setting to an area
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class EnvironmentVolume : MonoBehaviour
    {
        /*
         * Public Variables
         */

        /// <summary>
        /// The sky cubemap texture for the volume
        /// </summary>
        public Cubemap sky;

        /// <summary>
        /// The ambient light for the volume
        /// </summary>
        public Color ambientLight;

        /// <summary>
        /// The color of the fog for the volume
        /// </summary>
        public Color fogColor;

        /// <summary>
        /// The color of the sun (Default light yellow)
        /// </summary>
        public Color sunColor = new Color(255f / 255f, 252f / 255f, 233f / 255f);

        /// <summary>
        /// The intensity of the sun (Default 1.3f)
        /// </summary>
        public float sunIntensity = 1.3f;

        /// <summary>
        /// The rotation of the sun object
        /// </summary>
        public Vector3 sunRotation = new Vector3(50, -150, 0);

        /*
         * Internal Variables
         */

        /// <summary>
        /// The target to check which volume they're in
        /// </summary>
        private GameObject targetGameObject;

        /// <summary>
        /// The event called when entering the volume
        /// </summary>
        private event volumeEnteredDelegate volumeEnteredEvent;

        /// <summary>
        /// Reference to the box collider component
        /// </summary>
        private BoxCollider boxCollider;

        /// <summary>
        /// Keeps track to send events on change
        /// </summary>
        private bool targetIsInsideCollider;

        /*
         * Public Variables 
         */ 

        /// <summary>
        /// The delegate called when entering the volume
        /// </summary>
        public delegate void volumeEnteredDelegate(EnvironmentVolume environmentVolume);


        /*
         * Monobehavior Functions
         */
        
        /// <summary>
        /// Called on initialize
        /// </summary>
        public void Start()
        {
            //Set collider to trigger
            boxCollider = gameObject.GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        public void Update()
        {
            //Ensure target isn't null
            if(targetGameObject == null || volumeEnteredEvent == null)
            {
                return;
            }

            //Test if inside
            if (boxCollider.bounds.Contains(targetGameObject.transform.position))
            {
                if(targetIsInsideCollider == false)
                {
                    targetIsInsideCollider = true;
                    volumeEnteredEvent?.Invoke(this);
                }
            } else
            {
                targetIsInsideCollider = false;                
            }
        }

        /*
         * Public Functions
         */

        /// <summary>
        /// Allow classes to subscribe to on trigger events
        /// </summary>
        /// <param name="function">The function to call upon event</param>
        public void SubscribeToVolumeEnteredEvent(volumeEnteredDelegate function)
        {
            //Prevent double subscribes
            if (volumeEnteredEvent != null && volumeEnteredEvent.GetInvocationList().Contains(function))
                return;

            //Subscribe
            volumeEnteredEvent += function;
        }

        /// <summary>
        /// Sets the target focus gameobject for volume testing
        /// </summary>
        /// <param name="target">The target gameobject</param>
        public void SetTarget(GameObject target)
        {
            targetGameObject = target;
        }

        /// <summary>
        /// Returns if the target gameobject is inside the volume
        /// </summary>
        /// <returns></returns>
        public bool isTargetInsideVolume()
        {
            return this.targetIsInsideCollider;
        }
    }
}