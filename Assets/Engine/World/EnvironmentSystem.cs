using Engine.Configuration;
using Engine.Logging;
using Engine.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Engine.World
{
    /// <summary>
    /// Handles changing the environment based on volumes
    /// </summary>
    public class EnvironmentSystem
    {
        /*
         * Internal Variables
         */

        private Light sun;

        private bool instant = true;

        private Cubemap[] skies = new Cubemap[4];
        private float[] skyAlpha = new float[4];
        private int currentSky = 0;

        private long lastUpdate;
        private Queue<EnvironmentVolume> queue = new Queue<EnvironmentVolume>();

        private Color previousSkyTint;
        private Color targetSkyTint;

        private Color previousShadowColor;
        private Color targetShadowColor;

        private Color previousHighlightColor;
        private Color targetHighlightColor;

        private Color previousFogColor;
        private Color targetFogColor;

        private Color previousSunColor;
        private Color targetSunColor;

        private float previousSunIntensity;
        private float targetSunIntensity;

        private Vector3 previousSunRotation;
        private Vector3 targetSunRotation;

        private float environmentInterpolationValue = 1;

        private GameObject focusTarget;

        private SplitToning splitToning;

        /*
         * Constructor
         */

        /// <summary>
        /// Constructor (called on game load)
        /// </summary>
        internal EnvironmentSystem()
        {
            //Get split toning from volume component
            try
            {
                GameObject.FindObjectOfType<Volume>().profile.TryGet<SplitToning>(out splitToning);
            } catch 
            {
                Log.LogError("Unable to find split toning on global volume component.");
                return;
            }

            //Ensure skybox is set to a skybox material
            if(RenderSettings.skybox == null)
            {
                Log.LogError("Missing skybox on main scene in render settings.");
                return;
            }

            //Set skybox defaults
            UpdateSkyOpacities();

            //Ensure sun object is set up correctly
            if (GameObject.FindGameObjectWithTag("Sun") == null)
            {
                Log.LogError("Missing sun object. Ensure you have tagged the sun correctly.");
                return;
            } else if(GameObject.FindGameObjectWithTag("Sun").GetComponent<Light>() == null)
            {
                Log.LogError("Sun object is missing a light component!");
                return;
            }

            //Assign sun object
            sun = GameObject.FindGameObjectWithTag("Sun").GetComponent<Light>();            

            
        }

        /*
         * Public Functions
         */

        /// <summary>
        /// Called every frame
        /// </summary>
        internal void Update()
        {
            //Delay to prevent rapid switching.
            if(TimeHandler.getTimeInMilliseconds() - lastUpdate > 2500)
            {
                //Update is occuring
                if(queue.Count > 0)
                    lastUpdate = TimeHandler.getTimeInMilliseconds();

                foreach (EnvironmentVolume volume in queue)
                {
                    SetActiveVolume(volume, false);
                }
                queue.Clear();

            }


            UpdateSky();
            UpdateEnvironment();
        }

        /*
         * Internal Functions
         */

        /// <summary>
        /// Update the sky opacity values based on time
        /// </summary>
        private void UpdateSky()
        {
            for(int i = 0; i < skies.Length; i++)
            {
                if(i == currentSky)
                {
                    if (skyAlpha[i] < 1)
                    {
                        skyAlpha[i] += Time.deltaTime * ClientConfig.ENVIRONMENT_TRANSITION_SPEED * 0.1f;
                        if (skyAlpha[i] > 1)
                            skyAlpha[i] = 1;

                        UpdateSkyOpacity(i);
                    }
                } else
                {
                    if (skyAlpha[i] > 0) {
                        skyAlpha[i] -= Time.deltaTime * ClientConfig.ENVIRONMENT_TRANSITION_SPEED * 0.1f;
                        if (skyAlpha[i] < 0)
                            skyAlpha[i] = 0;

                        UpdateSkyOpacity(i);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the environment with interpolation based on time
        /// </summary>
        private void UpdateEnvironment()
        {
            if (environmentInterpolationValue >= 1.0f)
                return;

            environmentInterpolationValue += Time.deltaTime * ClientConfig.ENVIRONMENT_TRANSITION_SPEED * 0.1f;

            //Clamp
            if (environmentInterpolationValue > 1)
                environmentInterpolationValue = 1;

            //Set ambient data      
            splitToning.shadows.value = Color.Lerp(previousShadowColor, targetShadowColor, environmentInterpolationValue);
            splitToning.highlights.value = Color.Lerp(previousHighlightColor, targetHighlightColor, environmentInterpolationValue);
            RenderSettings.fogColor = Color.Lerp(previousFogColor, targetFogColor, environmentInterpolationValue);

            RenderSettings.skybox.SetColor("_Tint", Color.Lerp(previousSkyTint, targetSkyTint, environmentInterpolationValue));

            //Set sun data
            sun.color = Color.Lerp(previousSunColor, targetSunColor, environmentInterpolationValue);
            sun.intensity = Mathf.Lerp(previousSunIntensity, targetSunIntensity, environmentInterpolationValue);
            sun.transform.rotation = Quaternion.Lerp(Quaternion.Euler(previousSunRotation), Quaternion.Euler(targetSunRotation), environmentInterpolationValue);
        }

        /// <summary>
        /// Called when a chunk scene finishes loading
        /// </summary>
        /// <param name="scene"></param>
        internal void OnChunkSceneLoaded(Scene scene)
        {

            //Iterate over environment volumes and subscribe
            EnvironmentVolume[] volumes = scene.GetRootGameObjects()[0].GetComponentsInChildren<EnvironmentVolume>();
            foreach(EnvironmentVolume volume in volumes)
            {
                volume.SetTarget(focusTarget);
                volume.SubscribeToVolumeEnteredEvent(OnVolumeEntered);
            }  
            
            
        }

        /// <summary>
        /// Sets the focus target to volume test
        /// </summary>
        /// <param name="focusTarget">The focus target</param>
        internal void SetFocusTarget(GameObject focusTarget)
        {
            this.focusTarget = focusTarget;
        }

        /// <summary>
        /// Called when the camera enters a volume
        /// </summary>
        /// <param name="volume">The environment volume</param>
        private void OnVolumeEntered(EnvironmentVolume volume)
        {
            if(instant)
            {
                SetActiveVolume(volume, true);
                Log.LogMsg("Loaded volume instantly. Scene name:" + volume.gameObject.scene.name);
                instant = false;
            } else
            {
                queue.Enqueue(volume);
            }
        }

        private void SetActiveVolume(EnvironmentVolume volume, bool loadInstantly)
        {

            //Ensure volume still exists
            if (volume == null)
                return;

            //Ensure target is still in the volume
            if (volume.isTargetInsideVolume() == false)
                return;

            environmentInterpolationValue = 0;

            currentSky += 1;
            if (currentSky >= skies.Length)
                currentSky = 0;
            skyAlpha[currentSky] = 0;

            //Load sky instantly
            if (loadInstantly)
            {
                //Clear other skies
                for (int i = 0; i < skies.Length; i++)
                    skyAlpha[i] = 0;

                //Set new sky immeditely to maximum
                skyAlpha[currentSky] = 1;

                previousShadowColor = volume.shadowColor;
                previousHighlightColor = volume.highlightColor;
                previousFogColor = volume.fogColor;
                previousSkyTint = volume.skyTint;
                previousSunColor = volume.sunColor;
                previousSunIntensity = volume.sunIntensity;
                previousSunRotation = volume.sunRotation;
            }
            else
            {
                previousShadowColor = splitToning.shadows.value;
                previousHighlightColor = splitToning.highlights.value;
                previousFogColor = RenderSettings.fogColor;
                previousSkyTint = RenderSettings.skybox.GetColor("_Tint");
                previousSunColor = sun.color;
                previousSunIntensity = sun.intensity;
                previousSunRotation = sun.transform.rotation.eulerAngles;
            }

            skies[currentSky] = volume.sky;
            targetSkyTint = volume.skyTint;
            targetShadowColor = volume.shadowColor;
            targetHighlightColor = volume.highlightColor;
            targetFogColor = volume.fogColor;
            targetSunColor = volume.sunColor;
            targetSunIntensity = volume.sunIntensity;
            targetSunRotation = volume.sunRotation;

            //Update sky
            UpdateSkyOpacities();
            UpdateSkyTexture(currentSky);
        }

        /// <summary>
        /// Updates the sky material with the correct opacity values
        /// </summary>
        private void UpdateSkyOpacities()
        {
            for(int i = 0; i < skies.Length; i++)
            {
                RenderSettings.skybox.SetFloat("_Opacity" + (i + 1), skyAlpha[i]);
            }
        }

        /// <summary>
        /// Updates the sky material opacity value for one entry
        /// </summary>
        private void UpdateSkyOpacity(int skyIndex)
        {
            RenderSettings.skybox.SetFloat("_Opacity" + (skyIndex + 1), skyAlpha[skyIndex]);            
        }

        private void UpdateSkyTexture(int skyIndex)
        {
            RenderSettings.skybox.SetTexture("_Tex" + (skyIndex + 1), skies[skyIndex]);
        }


    }
}