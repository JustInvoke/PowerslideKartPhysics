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

        // Returns the next waypoint in the path
        public virtual BasicWaypoint GetNextPoint()
        {
            return nextPoint;
        }

        protected virtual void OnDrawGizmos()
        {
            // Draw radius and line to next point
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, radius);
            if (nextPoint != null) {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, nextPoint.transform.position);
            }
        }
    }
}
