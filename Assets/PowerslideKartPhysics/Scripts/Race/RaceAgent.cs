// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Class attached to a kart that participates in a race
    public class RaceAgent : ModeAgent
    {
        Track track;
        RaceController rc;
        [System.NonSerialized]
        public int lap = 1; // Current lap of the race
        [System.NonSerialized]
        public int lapsCompleted = 0; // Number of laps completed
        public Events.IntRaceAgent lapCompletedEvent; // Event invoked when a lap is completed
        [System.NonSerialized]
        public bool finishedRace = false; // True if the kart has crossed the finish line after completing all laps

        protected override void Awake() {
            base.Awake();
            track = FindObjectOfType<Track>();
            rc = FindObjectOfType<RaceController>();
        }

        private void FixedUpdate() {
            if (currentPoint != null) {
                // Check to see if the kart is overlapping a succeeding valid waypoint
                if (currentPoint is BasicWaypointMulti) {
                    BasicWaypointMulti multiPoint = (BasicWaypointMulti)currentPoint;
                    for (int i = 0; i < multiPoint.validPoints.Length; i++) {
                        if (CheckWaypointOverlap(multiPoint.validPoints[i])) {
                            currentPoint = multiPoint.validPoints[i];
                        }
                    }
                }
                else if (CheckWaypointOverlap(currentPoint.nextPoint)) {
                    currentPoint = currentPoint.nextPoint;
                }

                // Lines for visualizing position between waypoints
                Debug.DrawLine(tr.position, currentPoint.transform.position);
                if (currentPoint.nextPoint != null) {
                    Vector3 line = currentPoint.nextPoint.transform.position - currentPoint.transform.position;
                    Vector3 lineDir = line.normalized;
                    Vector3 linePoint = currentPoint.transform.position + lineDir * Mathf.Clamp(Vector3.Dot(tr.position - currentPoint.transform.position, lineDir), 0.0f, line.magnitude);
                    Debug.DrawLine(tr.position, linePoint, Color.magenta);
                }
            }
        }

        // Checks to see if the kart is within the range of the given waypoint
        bool CheckWaypointOverlap(BasicWaypoint point) {
            if (point == null || tr == null) { return false; }
            return (tr.position - point.transform.position).sqrMagnitude <= point.radius * point.radius;
        }

        // Returns the progress between the current point and the next point (0 to 1)
        public float GetPointProgress() {
            if (currentPoint == null) { return 0.0f; }
            if (currentPoint.nextPoint == null) { return 0.0f; }
            Vector3 line = currentPoint.nextPoint.transform.position - currentPoint.transform.position;
            return Mathf.Clamp(Vector3.Dot(tr.position - currentPoint.transform.position, line.normalized), 0.0f, line.magnitude) / line.magnitude;
        }

        // Returns the progress of the current lap (0 to 1)
        public float GetLapProgress() {
            if (currentPoint == null || track == null) { return 0.0f; }
            return Mathf.Clamp01((currentPoint.index + GetPointProgress()) / (track.maxIndex + 1.0f));
        }

        // Increments the current lap the kart is on and sets the current waypoint to the first one in the lap
        public void IncrementLap(BasicWaypoint startPoint) {
            if (!finishedRace) {
                lapCompletedEvent.Invoke(lap, this);
                currentPoint = startPoint;
                lap++;
                if (!finishedRace) {
                    lapsCompleted++;
                }
            }
        }

        // Returns the current position of the kart
        public int GetRacePosition() {
            if (rc != null) {
                return rc.GetPositionOfKart(this);
            }
            return -1;
        }

        // Respawns the kart by moving it to the last touched waypoint
        public override void Respawn() {
            base.Respawn();
            if (currentPoint != null && currentPoint.nextPoint != null && kart != null && kart.rotator != null) {
                kart.rotator.rotation = Quaternion.LookRotation(currentPoint.nextPoint.transform.position - currentPoint.transform.position, Vector3.up);
            }
        }

        // Comparer for sorting karts by their position in the race
        public class RaceAgentComparer : IComparer<RaceAgent>
        {
            public int Compare(RaceAgent x, RaceAgent y) {
                if (x == null || y == null) { return 0; }
                int compareVal = 0;

                if (x.finishedRace) { compareVal -= 100; }
                if (y.finishedRace) { compareVal += 100; }
                if (x.lap > y.lap) { compareVal -= 10; }
                if (y.lap > x.lap) { compareVal += 10; }
                if (x.GetLapProgress() > y.GetLapProgress()) { compareVal -= 1; }
                if (y.GetLapProgress() > x.GetLapProgress()) { compareVal += 1; }
                return System.Math.Sign(compareVal);
            }
        }
    }
}
