// Copyright (c) 2021 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    // Class for controlling the camera to follow a kart
    public class KartCamera : MonoBehaviour
    {
        Camera cam;
        public Kart targetKart;
        Rigidbody targetBody;
        [System.NonSerialized]
        public Vector3 targetPos = Vector3.zero;
        [System.NonSerialized]
        public Quaternion targetRot = Quaternion.identity;
        [System.NonSerialized]
        public float targetFov = 0.0f;
        float distance = 2.0f;
        public float initialDist = 6.0f;
        float height = 0.0f;
        public float initialHeight = 2.0f;
        public float maxVelDist = 1.0f;

        Transform tempRot;
        Transform smoothObj;
        Vector3 localDir = Vector3.back;
        Vector3 highPoint = Vector3.zero;
        public LayerMask castMask = 1;
        float castDist = 0.0f;
        Vector3 smoothVel = Vector3.zero;
        public float smoothRate = 10f;
        public float inputDeadZone = 0.1f;

        private void Awake()
        {
            cam = GetComponent<Camera>();

            if (GetComponent<AudioListener>() != null)
            {
                // Change velocity update mode because the camera moves in FixedUpdate
                GetComponent<AudioListener>().velocityUpdateMode = AudioVelocityUpdateMode.Fixed;
            }

            Initialize(targetKart);
        }

        // Set up objects to help with movement
        public void Initialize(Kart kart)
        {
            targetKart = kart;
            if (targetKart != null)
            {
                if (smoothObj == null)
                {
                    smoothObj = new GameObject("Kart Camera Smoother").transform;
                }
                smoothObj.position = targetKart.rotator.position;
                smoothObj.rotation = targetKart.rotator.rotation;

                if (tempRot == null)
                {
                    tempRot = new GameObject("Kart Camera Rotator").transform;
                }
                tempRot.parent = smoothObj;
                tempRot.localPosition = Vector3.zero;
                tempRot.localRotation = Quaternion.identity;
                targetBody = targetKart.GetComponent<Rigidbody>();
            }
        }

        private void FixedUpdate()
        {
            if (cam == null || targetKart == null || targetBody == null) { return; }

            // Movement calculations
            smoothVel = Vector3.Lerp(smoothVel, targetKart.localVel, smoothRate * Time.fixedDeltaTime);
            distance = initialDist + Mathf.Min(maxVelDist, new Vector3(smoothVel.x, 0.0f, smoothVel.z).magnitude * 0.1f);
            height = initialHeight - Mathf.Clamp(smoothVel.y * 0.1f, -maxVelDist, maxVelDist);

            smoothObj.position = targetKart.rotator.position - targetKart.rotator.right * Mathf.Clamp(smoothVel.x * 0.1f, -maxVelDist, maxVelDist);
            smoothObj.rotation = Quaternion.Lerp(smoothObj.rotation, targetKart.rotator.rotation, smoothRate * Time.fixedDeltaTime);

            // Getting input
            Vector2 camInput = InputManager.camRotInput;
            Vector2 camInputNormalized = InputManager.camRotInput.normalized;
            bool lookBack = InputManager.lookBackButton;

            // Apply input to movement
            float targetAngle = camInput.magnitude < inputDeadZone ? 0.0f : Mathf.Atan2(camInputNormalized.x, camInputNormalized.y);
            localDir = lookBack ? Vector3.back : Vector3.Slerp(localDir, new Vector3(-Mathf.Sin(targetAngle), 0.0f, -Mathf.Cos(targetAngle)), 0.1f);
            tempRot.localPosition = new Vector3(localDir.x, 0.0f, localDir.z * (lookBack ? -1.0f : 1.0f)) * distance + Vector3.up * height;
            tempRot.rotation = Quaternion.LookRotation(smoothObj.TransformDirection(-localDir), Vector3.up);

            // Raycast upward to determine how high the camera should be placed relative to the kart
            Vector3 targetHighPoint = smoothObj.TransformPoint(Vector3.up * height);
            RaycastHit highHit = new RaycastHit();
            if (Physics.Linecast(smoothObj.position, targetHighPoint, out highHit, castMask, QueryTriggerInteraction.Ignore))
            {
                highPoint = highHit.point + (smoothObj.position - targetHighPoint).normalized * cam.nearClipPlane;
            }
            else
            {
                highPoint = targetHighPoint;
            }

            // Raycast from the high point to determine how far away the camera should be from the kart
            RaycastHit lineHit = new RaycastHit();
            if (Physics.Linecast(highPoint, tempRot.position, out lineHit, castMask, QueryTriggerInteraction.Ignore))
            {
                castDist = 1.0f - lineHit.distance / Vector3.Distance(highPoint, tempRot.position);
            }
            else
            {
                castDist = Mathf.Lerp(castDist, 0.0f, smoothRate * Time.fixedDeltaTime);
            }

            // Set final position and rotation of camera
            targetPos = tempRot.position + (highPoint - tempRot.position) * castDist;
            targetRot = Quaternion.LookRotation(tempRot.rotation * Vector3.forward * (lookBack ? -1.0f : 1.0f), tempRot.rotation * Vector3.up);

            transform.position = targetPos;
            transform.rotation = targetRot;
        }

        private void OnDestroy()
        {
            if (tempRot != null)
            {
                Destroy(tempRot);
            }

            if (smoothObj != null)
            {
                Destroy(smoothObj);
            }
        }
    }
}