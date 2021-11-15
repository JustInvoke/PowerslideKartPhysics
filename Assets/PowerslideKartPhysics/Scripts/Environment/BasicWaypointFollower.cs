// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Kart input class for following waypoints
    public class BasicWaypointFollower : KartInput
    {
        Transform tr;
        public BasicWaypoint targetPoint;
        public float steerAmount = 10f;
        public float maxBrake = 0.2f;
        public float minAccel = 0.5f;
        bool reversing = false;
        float reverseTime = 0.0f;
        public float reverseSpeedLimit = 5.0f;
        public float reverseTimeThreshold = 3.0f;
        public float reverseDuration = 1.0f;
        float reverseSteer = 1.0f;

        public override void Awake() {
            base.Awake();
            tr = transform;
        }

        public override void Update() {
            if (targetPoint == null || theKart == null) { return; }
            if (theKart.rotator == null) { return; }

            // Setting next point upon touching point
            Vector3 targetDir = targetPoint.transform.position - tr.position;
            if (targetDir.sqrMagnitude <= targetPoint.radius * targetPoint.radius) {
                targetPoint = targetPoint.nextPoint;
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
            }

            base.Update();
        }
    }
}
