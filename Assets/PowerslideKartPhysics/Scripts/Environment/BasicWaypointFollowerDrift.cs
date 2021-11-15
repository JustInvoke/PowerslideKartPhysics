// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Kart input class for following waypoints
    public class BasicWaypointFollowerDrift : KartInput
    {
        Transform tr;
        Rigidbody rb;
        Kart kart;
        public BasicWaypoint targetPoint;
        BasicWaypoint nextPoint;
        public float steerAmount = 10f;
        public float maxBrake = 0.2f;
        public float minAccel = 0.5f;
        bool reversing = false;
        float reverseTime = 0.0f;
        public float reverseSpeedLimit = 5.0f;
        public float reverseTimeThreshold = 3.0f;
        public float reverseDuration = 1.0f;
        float reverseSteer = 1.0f;

        [Header("Drift")]
        public float driftStartThreshold = 1.0f;
        public float driftEndThreshold = 0.2f;
        public float driftSpeedMultiplier = 0.02f;
        public float driftSpeedMultiplierCap = 2.0f;
        public float distanceAdvanceFactor = 0.05f;

        public override void Awake() {
            base.Awake();
            tr = transform;
            rb = GetComponent<Rigidbody>();
            kart = GetComponent<Kart>();
            if (targetPoint != null) {
                nextPoint = targetPoint.nextPoint;
            }
        }

        public override void Update() {
            if (targetPoint == null || nextPoint == null || theKart == null) { return; }
            if (theKart.rotator == null) { return; }

            Vector3 targetPos = Vector3.Lerp(targetPoint.transform.position, nextPoint.transform.position, 1.0f - Mathf.Clamp01(Vector3.Distance(tr.position, targetPoint.transform.position) * distanceAdvanceFactor));

            // Setting next point upon touching point
            Vector3 targetDir = targetPos - tr.position;
            if ((targetPoint.transform.position - tr.position).sqrMagnitude <= targetPoint.radius * targetPoint.radius) {
                targetPoint = targetPoint.nextPoint;
                if (targetPoint != null) {
                    nextPoint = targetPoint.nextPoint;
                }
            }

            float forwardDot = Vector3.Dot(targetDir.normalized, theKart.rotator.forward);
            float rightDot = Vector3.Dot(targetDir.normalized, theKart.rotator.right);

            if (!reversing) {
                // Starting reverse process if stuck
                if (theKart.velMag < reverseSpeedLimit) {
                    reverseTime += Time.deltaTime;
                    if (reverseTime > reverseTimeThreshold) {
                        reversing = true;
                        reverseTime = 0.0f;
                        reverseSteer = -Mathf.Sign(rightDot);
                    }
                }
                else {
                    reverseTime = 0.0f;
                }

                // Setting normal drive input
                targetAccel = Mathf.Clamp(forwardDot, minAccel, 1.0f);
                targetBrake = Mathf.Clamp(-forwardDot * theKart.velMag, 0.0f, maxBrake);
                targetSteer = forwardDot > 0 ? rightDot * steerAmount : Mathf.Sign(rightDot);

                // Setting drift input
                if (rb != null && kart != null && nextPoint != null) {
                    if (kart.rotator != null) {
                        Vector3 localNextDir = kart.rotator.InverseTransformDirection((nextPoint.transform.position - targetPoint.transform.position).normalized);
                        float driftVal = Mathf.Abs(rightDot + localNextDir.x) * Mathf.Min(rb.velocity.magnitude * driftSpeedMultiplier, driftSpeedMultiplierCap);

                        if (!drifting && driftVal > driftStartThreshold) {
                            drifting = true;
                        }
                        else if (drifting && driftVal < driftEndThreshold) {
                            drifting = false;
                        }
                    }
                }
            }
            else {
                // Reverse timing
                reverseTime += Time.deltaTime;
                if (reverseTime > reverseDuration) {
                    reversing = false;
                    reverseTime = 0.0f;
                }

                // Setting reverse input
                targetAccel = 0.0f;
                targetBrake = 1.0f;
                targetSteer = reverseSteer;
                drifting = false;
            }

            base.Update();
        }
    }
}
