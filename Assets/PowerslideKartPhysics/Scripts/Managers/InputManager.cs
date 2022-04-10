// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PowerslideKartPhysics
{
    [DisallowMultipleComponent]
    // This class manages static input values, fetched either from Unity's Input Manager or mobile input functions
    public class InputManager : MonoBehaviour
    {
        // Standard input manager axis names (legacy)
        [Header("Standard Input")]
        public bool enableStandardInput = true;
        public string accelAxisName = "Accel";
        public static float accelInput = 0.0f;
        public string brakeAxisName = "Brake";
        public static float brakeInput = 0.0f;
        public string steerAxisName = "Horizontal";
        public static float steerInput = 0.0f;
        public string driftButtonName = "Drift";
        public static bool driftButton = false;
        public string boostButtonName = "Boost";
        public static bool boostButton = false;
        public static bool boostButtonDown;
        public string itemButtonName = "Item";
        public static bool itemButtonDown = false;
        public string cameraXAxisName = "Camera X";
        public string cameraYAxisName = "Camera Y";
        public static Vector2 camRotInput = Vector2.zero;
        public string lookBackButtonName = "Look Back";
        public static bool lookBackButton = false;
        public string restartButtonName = "Restart";

        // Mobile input settings
        [Header("Mobile Input")]
        public bool enableMobileInput = true;
        [Range(0.0f, 1.0f)]
        public float mobileAutoAccel = 0.0f;
        [Range(0.0f, 1.0f)]
        public float mobileSteerAccel = 0.0f;
        [Range(0.0f, 1.0f)]
        public float mobileDriftAccel = 0.0f;
        [Range(0.0f, 1.0f)]
        public float mobileBoostAccel = 0.0f;
        [Range(0.0f, 1.0f)]
        public float mobileDriftHoldSteer = 0.0f;
        public static MobileInputStruct MobileInput = new MobileInputStruct();
        MobileInputStruct MobileInputTarget = new MobileInputStruct();
        float lastMobileSteer = 0.0f;

        void Update() {
            // Try/catch is used for fetching input in case the axes named do not exist
#if UNITY_EDITOR
            try {
#endif
                // Fetch standard input
                if (enableStandardInput) {
                    accelInput = Input.GetAxis(accelAxisName);
                    brakeInput = Input.GetAxis(brakeAxisName);
                    steerInput = Input.GetAxis(steerAxisName);
                    driftButton = Input.GetButton(driftButtonName);
                    boostButton = Input.GetButton(boostButtonName);
                    boostButtonDown = Input.GetButtonDown(boostButtonName);
                    itemButtonDown = Input.GetButtonDown(itemButtonName);
                    camRotInput = new Vector2(Input.GetAxis(cameraXAxisName), Input.GetAxis(cameraYAxisName));
                    lookBackButton = Input.GetButton(lookBackButtonName);

                    if (!string.IsNullOrEmpty(restartButtonName)) {
                        if (Input.GetButtonDown(restartButtonName)) {
                            RestartLevel();
                        }
                    }
                }

                // Fetch mobile input
                if (enableMobileInput) {
                    MobileInput.brakeInput = MobileInputTarget.brakeInput;
                    MobileInput.steerInput = MobileInputTarget.steerInput;
                    if (Mathf.Abs(MobileInput.steerInput) > 0) {
                        lastMobileSteer = MobileInput.steerInput;
                    }

                    MobileInput.driftButton = MobileInputTarget.driftButton;
                    // Auto steer holding for drift
                    if (MobileInput.driftButton && mobileDriftHoldSteer > 0 && Mathf.Abs(lastMobileSteer) > Mathf.Abs(MobileInput.steerInput)) {
                        MobileInput.steerInput = mobileDriftHoldSteer * lastMobileSteer;
                    }

                    MobileInput.boostButton = MobileInputTarget.boostButton;
                    MobileInput.itemButtonDown = MobileInputTarget.itemButtonDown;

                    // Setting accel input based on maxmimum auto factor
                    MobileInput.accelInput = Mathf.Max(
                        MobileInputTarget.accelInput,
                        mobileAutoAccel - MobileInput.brakeInput,
                        Mathf.Abs(MobileInput.steerInput) * mobileSteerAccel - MobileInput.brakeInput,
                        (MobileInput.driftButton ? mobileDriftAccel : 0.0f) - MobileInput.brakeInput,
                        (MobileInput.boostButton ? mobileBoostAccel : 0.0f) - MobileInput.brakeInput);
                }
#if UNITY_EDITOR
            }
            catch (System.Exception e) {
                Debug.LogWarning(e.Message);
            }
#endif
        }

        // Reloads the active scene
        public void RestartLevel() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // Set mobile accel input
        public void SetAccelMobile(float accel) {
            MobileInputTarget.accelInput = accel;
        }

        // Set mobile brake input
        public void SetBrakeMobile(float brake) {
            MobileInputTarget.brakeInput = brake;
        }

        // Set mobile steer input
        public void SetSteerMobile(float steer) {
            MobileInputTarget.steerInput = steer;
        }

        // Set mobile drift input
        public void SetDriftMobile(bool drift) {
            MobileInputTarget.driftButton = drift;
        }

        // Set mobile boost input
        public void SetBoostMobile(bool boostIn) {
            MobileInputTarget.boostButton = boostIn;
        }

        // Set the mobile item input to true for one frame
        public void PressItemMobile() {
            StartCoroutine(MobileItemPress());
        }

        // Mobile item press process
        IEnumerator MobileItemPress() {
            MobileInputTarget.itemButtonDown = true;
            yield return null;
            MobileInputTarget.itemButtonDown = false;
        }

        // Struct for containing mobile input values
        public struct MobileInputStruct
        {
            public float accelInput;
            public float brakeInput;
            public float steerInput;
            public bool driftButton;
            public bool boostButton;
            public bool itemButtonDown;
        }
    }
}
