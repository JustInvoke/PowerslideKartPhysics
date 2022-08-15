// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Waypoint class supporting connections to multiple waypoints for battle mode
    public class BattleWaypoint : MonoBehaviour
    {
        public float radius = 1.0f;
        public List<BattleWaypoint> connectedPoints = new List<BattleWaypoint>(); // Points that this one is connected to

        private void Awake() {
            ValidateConnections();
        }

        // Make sure all connected points also reference this one
        void ValidateConnections() {
            foreach (BattleWaypoint point in connectedPoints) {
                if (!point.connectedPoints.Contains(this)) {
                    point.connectedPoints.Add(this);
                }
            }
        }

        public BattleWaypoint GetRandomPoint() {
            return connectedPoints[Random.Range(0, connectedPoints.Count)];
        }

        protected void OnDrawGizmos() {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, radius);
            // Draws lines to connected points
            Gizmos.color = Color.green;
            foreach (BattleWaypoint point in connectedPoints) {
                Gizmos.DrawLine(transform.position, point.transform.position);
            }
        }
    }
}
