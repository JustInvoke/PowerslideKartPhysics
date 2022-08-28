// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Waypoint class
    public class BasicWaypoint : MonoBehaviour
    {
        public BasicWaypoint nextPoint;
        public float radius = 1.0f;
        public int index = 0;
        public float groundSnapOffset = 1.0f;
        public float maxGroundSnapSteps = 1000;

        // Returns the next waypoint in the path
        public virtual BasicWaypoint GetNextPoint() {
            return nextPoint;
        }

        protected virtual void OnDrawGizmos() {
            // Draw radius and line to next point
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, radius);
            if (nextPoint != null) {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, nextPoint.transform.position);
            }
        }

        public void SnapToGround() {
            int stepCount = 0;
            Vector3 castStart = transform.position + Vector3.up * 5.0f;
            while (stepCount < maxGroundSnapSteps) {
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(castStart, Vector3.down, out hit, Mathf.Infinity, LayerInfo.AllExcludingKarts, QueryTriggerInteraction.Ignore)) {
                    transform.position = hit.point + Vector3.up * groundSnapOffset;
                    break;
                }
                castStart += Vector3.up;
                stepCount++;
            }
        }
    }
}
