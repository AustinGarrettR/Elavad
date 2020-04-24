using Engine.Factory;
using Engine.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine.Logging;
using Engine.Configuration;
using System.Threading.Tasks;

namespace Engine.CameraSystem
{
    /// <summary>
    /// The manager that controls the player camera
    /// </summary>
    public class ClientCameraManager : Manager
    {
        /*
         * Constructor 
         */

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientPlayerManager">The client player manager</param>
        public ClientCameraManager(ClientPlayerManager clientPlayerManager)
        {
            this.clientPlayerManager = clientPlayerManager;
        }

        /*
         * Override Methods
         */

        /// <summary>
        /// Called on initialization
        /// </summary>
        public override void Init()
        {
            //Find camera in scene
            mainCamera = Camera.main;

            //Ensure the camera isnt null
            if(mainCamera == null)
            {
                Log.LogError("Missing main camera object. Make sure the camera is tagged as main camera.");
                return;
            }

            
        }

        /// <summary>
        /// Called every frame
        /// </summary>
        public override void Process()
        {
            if(gameLoaded)
            {
                RotateCameraAroundTarget();
            }
        }


        /// <summary>
        /// Called on game load
        /// </summary>
        /// <returns></returns>
        public override async Task LoadGameTask()
        {
            //Set rotate around target
            this.SetRotateAroundTargetToPlayer();

            //Update camera during load so it's in the correct
            //spot after load
            RotateCameraAroundTarget();

            gameLoaded = true;
            await Task.CompletedTask;
        }

        /// <summary>
        /// Called on shutdown
        /// </summary>
        public override void Shutdown()
        {
            
        }

        /*
         * Public Functions
         */

        /// <summary>
        /// Change rotate around target
        /// </summary>
        /// <param name="target">The target gameobject other than the my player</param>
        public void SetRotateAroundTarget(GameObject target)
        {

            if (target.Equals(clientPlayerManager.GetMyPlayer().GetPlayerObject()))
            {
                Log.LogError("Please use the function SetRotateAroundTargetToPlayer instead to assign the rotation target to my player.");
                return;
            }

            rotateAroundTarget = target;
        }

        /// <summary>
        /// Change rotate around target to the my player
        /// </summary>
        public void SetRotateAroundTargetToPlayer()
        {
            rotateAroundTarget = clientPlayerManager.GetMyPlayer().GetPlayerObject();
        }

        /*
         * Internal Functions
         */

        /// <summary>
        /// Update the camera
        /// </summary>
        private void RotateCameraAroundTarget()
        {
            //Ensure target is not null
            if (rotateAroundTarget == null)
                return;

            //Set offset
            Vector3 cameraOffset = new Vector3(0, ClientConfig.CAMERA_Y_OFFSET, 0);

            //Get input
            float velocityX = rotationXSpeed * -UnityEngine.Input.GetAxis("Horizontal") * Time.deltaTime * 100;
            float velocityY = rotationYSpeed * -UnityEngine.Input.GetAxis("Vertical") * Time.deltaTime * 100;

            //Use input to change the rotation values
            rotationYAxis += velocityX;
            rotationXAxis -= velocityY;

            //Clamp angle
            rotationXAxis = ClampAngle(rotationXAxis, ClientConfig.CAMERA_Y_AXIS_MINIMUM, ClientConfig.CAMERA_Y_AXIS_MAXIMUM);

            //Calculate rotation from the rotation values
            Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
            Quaternion rotation = toRotation;

            //Distance from target
            distanceFromTarget = Mathf.Clamp(distanceFromTarget - UnityEngine.Input.GetAxis("Mouse ScrollWheel") * 5, ClientConfig.CAMERA_DISTANCE_MINIMUM, ClientConfig.CAMERA_DISTANCE_MAXIMUM);
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distanceFromTarget);

            //Zoom closer when an object interceps the sight line
          /*  bool intercepted = false;

            if (zoomWhenIntercepted)
            {
                Vector3 desiredDistanceVector = new Vector3(0.0f, 0.0f, -distanceFromTarget);
                Vector3 positionBeforeRaycast = rotation * desiredDistanceVector + rotateAroundTarget.transform.position + cameraOffset;

                //Layer mask all but players layer (=~ inverts the bits)
                int layerMask =~ LayerMask.GetMask(SharedConfig.PLAYERS_LAYER_NAME);

                RaycastHit[] hits = Physics.SphereCastAll(rotateAroundTarget.transform.position + cameraOffset, 0.35f, positionBeforeRaycast - rotateAroundTarget.transform.position + cameraOffset, distanceFromTarget, layerMask);
                if (hits.Length > 0)
                {
                    //How far extra to move in
                    float offset = 0.15f;
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.collider.bounds.Contains(positionBeforeRaycast) || hit.collider.bounds.Contains(positionBeforeRaycast + new Vector3(0, 0, -offset)))
                        {
                            if (smoothDistanceFromTarget > hit.distance - offset)
                            {
                                //Update distance to the hit and offset
                                float updatedDistance = hit.distance - offset;

                                //Only update the actual zoom instantly if forcing zoom in.
                                //Zooming out should still be dampened.
                                //if (updatedDistance < smoothDistanceFromTarget + offset)
                                //{
                                    negDistance = new Vector3(0.0f, 0.0f, -updatedDistance);
                                    smoothDistanceFromTarget = updatedDistance;

                                    //Set intercepted to true so we dont dampen the zoom
                                    intercepted = true;
                                //}
                            }
                        }
                    }

                }
            }
            */

            //If not intercepted, change zoom gradually
            //if(intercepted == false)
            //{

                smoothDistanceFromTarget = Mathf.Lerp(smoothDistanceFromTarget, distanceFromTarget, smoothZoomSpeed * Time.deltaTime);

                negDistance = new Vector3(0.0f, 0.0f, -smoothDistanceFromTarget);
            //}

            //Calcualate new position with distance
            Vector3 position = rotation * negDistance + rotateAroundTarget.transform.position + cameraOffset;

            //Apply
            mainCamera.transform.rotation = rotation;
            mainCamera.transform.position = position;

        }

        /*
         * Internal Variables
         */

        /// <summary>
        /// If the game is loaded or not
        /// </summary>
        private bool gameLoaded;

        /// <summary>
        /// Main camera reference
        /// </summary>
        private Camera mainCamera;

        /// <summary>
        /// The target for the camera to rotate around
        /// </summary>
        private GameObject rotateAroundTarget;

        /// <summary>
        /// The client player manager reference
        /// </summary>
        private ClientPlayerManager clientPlayerManager;

        /// <summary>
        /// The x axis rotation of the camera
        /// </summary>
        private float rotationXAxis = ClientConfig.CAMERA_DEFAULT_X_AXIS_ROTATION;

        /// <summary>
        /// The y axis rotation of the camera
        /// </summary>
        private float rotationYAxis = ClientConfig.CAMERA_DEFAULT_Y_AXIS_ROTATION;

        /// <summary>
        /// The x axis rotation of the camera
        /// </summary>
        private float rotationXSpeed = ClientConfig.CAMERA_DEFAULT_X_AXIS_ROTATION_SPEED;

        /// <summary>
        /// The x axis rotation of the camera
        /// </summary>
        private float rotationYSpeed = ClientConfig.CAMERA_DEFAULT_Y_AXIS_ROTATION_SPEED;

        /// <summary>
        /// The distance from the player
        /// </summary>
        private float distanceFromTarget = ClientConfig.CAMERA_DEFAULT_DISTANCE;

        /// <summary>
        /// The distance from the player
        /// </summary>
        private float smoothDistanceFromTarget = ClientConfig.CAMERA_DEFAULT_DISTANCE;

        /// <summary>
        /// The default speed the distance changes
        /// </summary>
        private float smoothZoomSpeed = ClientConfig.CAMERA_DEFAULT_SMOOTH_ZOOM_SPEED;

        /// <summary>
        /// If to zoom when intercepted by an object
        /// </summary>
        private bool zoomWhenIntercepted = ClientConfig.CAMERA_DEFAULT_ZOOM_WHEN_INTERCEPTED;

        /*
         * Utility Methods
         */
        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}