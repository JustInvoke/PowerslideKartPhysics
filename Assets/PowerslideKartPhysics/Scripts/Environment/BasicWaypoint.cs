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
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(transform.position + Vector3.up * 5.0f, Vector3.down, out hit, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore)) {
                transform.position = hit.point + Vector3.up * groundSnapOffset;
            }
        }
    }
}
