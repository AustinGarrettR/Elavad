using Engine.Factory;
using Engine.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Engine.Logging;
using Engine.Configuration;
using System.Threading.Tasks;
using Engine.Loading;

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
        public override async Task LoadGameTask(ClientLoadData clientLoadData)
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

            if(instantCameraMove)
            {
                instantCameraMove = false;
                smoothTargetPosition = rotateAroundTarget.transform.position;
            }

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
            smoothDistanceFromTarget = Mathf.Lerp(smoothDistanceFromTarget, distanceFromTarget, smoothZoomSpeed * Time.deltaTime);

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -smoothDistanceFromTarget);

            smoothTargetPosition = Vector3.Lerp(smoothTargetPosition, rotateAroundTarget.transform.position, ClientConfig.CAMERA_DEFAULT_SMOOTH_POSITION_SPEED * Time.deltaTime);

            //Calcualate new position with distance
            Vector3 position = rotation * negDistance + smoothTargetPosition + cameraOffset;
        

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
        /// If to set the smooth position on start
        /// </summary>
        private bool instantCameraMove = true;

        /// <summary>
        /// Smooth position for camera
        /// </summary>
        private Vector3 smoothTargetPosition;

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
        /// The default speed for camera dampening
        /// </summary>
        private float smoothCameraSpeed = ClientConfig.CAMERA_DEFAULT_SMOOTH_POSITION_SPEED;

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