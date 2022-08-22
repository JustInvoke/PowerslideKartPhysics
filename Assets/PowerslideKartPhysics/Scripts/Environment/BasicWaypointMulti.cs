// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Waypoint class supporting connections to multiple waypoints
    public class BasicWaypointMulti : BasicWaypoint
    {
        public BasicWaypoint[] alternatePoints = new BasicWaypoint[0]; // Alternate waypoints to connect to, should not include the inherited next point
        public BasicWaypoint[] validPoints = new BasicWaypoint[0]; // Points that are valid for lap progression when skipping over other waypoints

        // Returns the next waypoint in the path, randomly picking from alternate points
        public override BasicWaypoint GetNextPoint() {
            if (alternatePoints.Length > 0) {
                return Random.value > 1.0f / (alternatePoints.Length + 1) ? alternatePoints[Random.Range(0, alternatePoints.Length)] : nextPoint;
            }
            else {
                return nextPoint;
            }
        }

        protected override void OnDrawGizmos() {
            base.OnDrawGizmos();
            // Draws lines to alternate points
            if (alternatePoints != null) {
                Gizmos.color = Color.green;
                for (int i = 0; i < alternatePoints.Length; i++) {
                    if (alternatePoints[i] != null) {
                        Gizmos.DrawLine(transform.position, alternatePoints[i].transform.position);
                    }
                }
            }
        }

        protected void OnDrawGizmosSelected() {
            Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 0.2f);
            Gizmos.DrawSphere(transform.position, radius);
            // Draw lines to all valid points
            if (validPoints != null) {
                Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 0.8f);
                for (int i = 0; i < validPoints.Length; i++) {
                    if (validPoints[i] != null) {
                        Gizmos.DrawLine(transform.position, validPoints[i].transform.position);
                        Gizmos.DrawWireSphere(validPoints[i].transform.position, validPoints[i].radius + 0.1f);
                    }
                }
            }
        }
    }
}
