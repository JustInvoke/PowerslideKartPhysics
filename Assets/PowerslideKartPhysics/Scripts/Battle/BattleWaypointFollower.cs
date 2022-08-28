// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Kart input class for following battle waypoints
    public class BattleWaypointFollower : BasicWaypointFollowerDrift
    {
        protected override void CheckPointOverlap() { }

        private void OnTriggerEnter(Collider other) {
            BattleWaypoint point = other.GetComponent<BattleWaypoint>();
            if (point != null) {
                //Debug.Log("triggered");
                targetPoint = point.GetNextPoint();
                if (targetPoint != null) {
                    nextPoint = targetPoint.GetNextPoint();
                }
            }
        }

        protected override void StopReversing() {
            base.StopReversing();
            bool foundPoint = false;
            int loopCount = 0;
            float overlapRadius = 50f;
            while (!foundPoint && loopCount < 100) {
                Collider[] cols = Physics.OverlapSphere(tr.position, overlapRadius, LayerInfo.WaypointLayer, QueryTriggerInteraction.Collide);
                foreach (Collider col in cols) {
                    if (!Physics.Linecast(tr.position, col.transform.position,  LayerInfo.AllExcludingKarts, QueryTriggerInteraction.Ignore)) {
                        BattleWaypoint point = col.GetComponent<BattleWaypoint>();
                        if (point != null) {
                            targetPoint = point.GetNextPoint();
                            if (targetPoint != null) {
                                nextPoint = targetPoint.GetNextPoint();
                            }
                            //Debug.Log(overlapRadius);
                            foundPoint = true;
                            break;
                        }
                    }
                }
                overlapRadius += 20f;
                loopCount++;
            }
        }

        public void OnDrawGizmos() {
            if (targetPoint != null) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, targetPoint.transform.position);
            }
        }
    }
}
