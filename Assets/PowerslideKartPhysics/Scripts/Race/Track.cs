// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Class representing a racetrack
    [DisallowMultipleComponent]
    public class Track : MonoBehaviour
    {
        BasicWaypoint[] trackPoints = new BasicWaypoint[0];
        public BasicWaypoint startPoint; // First waypoint in a lap
        public int maxIndex = 0; // Maximum waypoint index, set automatically by CalculateWaypointIndices()
        FinishLine finishLine;

        private void Awake() {
            // Find waypoints and finish line
            trackPoints = GetComponentsInChildren<BasicWaypoint>();
            finishLine = GetComponentInChildren<FinishLine>();
            if (finishLine != null) {
                finishLine.track = this;
            }
        }

        // Calculates the indices for each waypoint
        public void CalculateWaypointIndices() {
            if (startPoint != null) {
                trackPoints = GetComponentsInChildren<BasicWaypoint>();
                for (int i = 0; i < trackPoints.Length; i++) {
                    trackPoints[i].index = -1;
                }

                EnumeratePoint(startPoint, 0);

                maxIndex = 0;
                for (int i = 0; i < trackPoints.Length; i++) {
                    maxIndex = Mathf.Max(maxIndex, trackPoints[i].index);
                }
            }
        }

        // Recursive function for calculating waypoint indices
        void EnumeratePoint(BasicWaypoint point, int index) {
            if (point != null) {
                if (point.index < 0) {
                    point.index = index;
                    EnumeratePoint(point.nextPoint, index + 1);
                    if (point is BasicWaypointMulti) {
                        BasicWaypointMulti multiPoint = (BasicWaypointMulti)point;
                        for (int i = 0; i < multiPoint.alternatePoints.Length; i++) {
                            EnumeratePoint(multiPoint.alternatePoints[i], index + 1);
                        }
                    }
                }
            }
        }
    }
}
