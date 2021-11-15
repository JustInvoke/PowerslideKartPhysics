// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    [DisallowMultipleComponent]
    public class KartWheel : MonoBehaviour
    {
        Transform tr;
        Kart kartParent;
        public float suspensionDistance = 1.0f;
        [Range(0.0f, 1.0f)]
        public float maxExtension = 1.0f;
        float extendDistance = 0.0f;
        public float airExtendRate = 20f;
        public float compressionTiltAmount = 0.0f;
        public float radius = 1.0f;
        public float width = 1.0f;
        public float steerAmount = 0.0f;
        public bool driven = false;

        public Transform visualWheel;
        [System.NonSerialized]
        public bool grounded = false;
        [System.NonSerialized]
        public float rotationRate = 0.0f;
        public float burnoutRotateSpeed = 1000f;
        [System.NonSerialized]
        public Vector3 localVel = Vector3.zero;
        [System.NonSerialized]
        public int flippedSideFactor = 1;

        // This is for the sake of maintaining contact point information for staggered raycasting (not every frame)
        Vector3 localContactPoint = Vector3.zero;
        public Vector3 contactPoint {
            get {
                return transform.TransformPoint(localContactPoint);
            }

            set {
                localContactPoint = transform.InverseTransformPoint(value);
            }
        }

        [System.NonSerialized]
        public Vector3 contactNormal = Vector3.zero;
        [System.NonSerialized]
        public float contactDistance = 0.0f;
        [System.NonSerialized]
        public Transform contactTr;
        [System.NonSerialized]
        public GroundSurfacePreset surfaceProps;
        [System.NonSerialized]
        public float surfaceFriction = 1.0f;
        [System.NonSerialized]
        public float surfaceSpeed = 1.0f;
        float rotAngle = 0.0f;
        Vector3 steerAngle = Vector3.zero;
        [System.NonSerialized]
        public bool sliding = false;

        void Awake() {
            tr = transform;
            kartParent = tr.root.GetComponent<Kart>();
            flippedSideFactor = tr.localPosition.x > -0.01f ? 1 : -1;
        }

        void Update() {
            if (kartParent == null) { return; }

            // Set wheel rotation rate
            if (kartParent.burnout) {
                rotationRate = driven ? burnoutRotateSpeed * flippedSideFactor : 0.0f;
            }
            else {
                rotationRate = (localVel.z / Mathf.Max(0.001f, radius * 2.0f * Mathf.PI)) * (Mathf.PI * 100f) * flippedSideFactor;
            }

            // Calculate rotation angle and suspension distance
            rotAngle = Mathf.Repeat(rotAngle - rotationRate * Time.deltaTime * flippedSideFactor, 360f);
            steerAngle = tr.forward * kartParent.GetVisualSteer() * flippedSideFactor * -steerAmount;
            extendDistance = grounded ? contactDistance : Mathf.Lerp(extendDistance, suspensionDistance, airExtendRate * Time.deltaTime);

            Vector3 groundPoint = grounded ? contactPoint : tr.TransformPoint(Vector3.down * extendDistance);

            // Set final position and rotation of the wheel
            if (visualWheel != null) {
                float compression = 1.0f;
                if (suspensionDistance > 0) {
                    compression = Mathf.Clamp01(Vector3.Distance(tr.position, groundPoint) / Mathf.Max(suspensionDistance, 0.01f));
                }
                visualWheel.localPosition = Vector3.down * Mathf.Clamp(compression * suspensionDistance - radius, 0.0f, suspensionDistance) * Mathf.Clamp01(maxExtension);
                visualWheel.rotation = Quaternion.LookRotation(tr.right * flippedSideFactor + tr.up * (0.5f - compression) * compressionTiltAmount + steerAngle, tr.TransformDirection(0.0f, Mathf.Sin(rotAngle * Mathf.Deg2Rad), Mathf.Cos(rotAngle * Mathf.Deg2Rad)));
            }

            bool groundAlwaysSlide = false;

            if (surfaceProps != null) {
                groundAlwaysSlide = surfaceProps.alwaysSlide;
            }

            // Determine whether the wheel is sliding
            sliding = grounded &&
                ((kartParent.drifting && kartParent.driftDir != 0)
                || (driven && kartParent.burnout)
                || kartParent.spinningOut
                || (groundAlwaysSlide && localVel.magnitude >= kartParent.autoStopSpeed));
        }

        // Set the surface type of the ground that the wheel is on
        public void SetSurface(GroundSurface surface) {
            if (surface != null) {
                if (surface is TerrainSurface) {
                    TerrainSurface terSurf = (TerrainSurface)surface;
                    surfaceProps = terSurf.GetDominantGroundSurfaceAtPoint(tr.position);
                    surfaceFriction = terSurf.GetFriction(surfaceProps);
                    surfaceSpeed = terSurf.GetSpeed(surfaceProps);
                }
                else {
                    surfaceProps = surface.props;
                    surfaceFriction = surface.GetFriction(tr.position);
                    surfaceSpeed = surface.GetSpeed(tr.position);
                }
            }
            else {
                surfaceProps = null;
                surfaceFriction = 1.0f;
                surfaceSpeed = 1.0f;
            }
        }

        // Visualize the wheel size and suspension distance
        void OnDrawGizmosSelected() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, -transform.up * suspensionDistance);
            if (visualWheel != null) {
                GizmosExtra.DrawWireCylinder(transform.position, visualWheel.forward, radius, width);
            }
            else {
                GizmosExtra.DrawWireCylinder(transform.position, transform.right, radius, width);
            }
        }
    }
}