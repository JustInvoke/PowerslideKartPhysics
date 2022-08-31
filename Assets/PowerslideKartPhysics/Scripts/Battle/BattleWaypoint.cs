// Copyright (c) 2022 Justin Couch / JustInvoke
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Waypoint class supporting connections to multiple waypoints for battle mode
    [RequireComponent(typeof(SphereCollider))]
    public class BattleWaypoint : BasicWaypoint
    {
        public List<BattleWaypoint> connectedPoints = new List<BattleWaypoint>(); // Points that this one is connected to
        public float maxConnectionSteepness = 0.5f;
        public float connectionStepDistance = 1.0f;
        public int maxConnectionSteps = 1000;
        public float similarConnectionAngleDotLimit = 0.9f;
        public bool showDebugConnections;

        public void CalculateConnections(BattleWaypoint[] suppliedPoints = null) {
            if (suppliedPoints == null) {
                suppliedPoints = FindObjectsOfType<BattleWaypoint>();
            }

            connectedPoints.Clear();
            connectedPoints.AddRange(suppliedPoints.Where(wp => wp != this));
            PurgeInvalidConnections();
        }

        public void PurgeInvalidConnections() {
            List<BattleWaypoint> purgedPoints = new List<BattleWaypoint>();
            CheckAllWaypoints(connectedPoints, (point) => { purgedPoints.Add(point); }, null);

            foreach (BattleWaypoint point in purgedPoints) {
                connectedPoints.Remove(point);
            }

            purgedPoints.Clear();

            foreach (BattleWaypoint point in connectedPoints) {
                foreach (BattleWaypoint point2 in connectedPoints) {
                    Vector3 toPoint = (point.transform.position - transform.position);
                    Vector3 toPoint2 = (point2.transform.position - transform.position);
                    if (point != point2 && Vector3.Dot(toPoint.normalized, toPoint2.normalized) > similarConnectionAngleDotLimit && !purgedPoints.Contains(point) && !purgedPoints.Contains(point2)) {
                        if (toPoint.sqrMagnitude < toPoint2.sqrMagnitude) {
                            purgedPoints.Add(point2);
                        }
                        else if (toPoint.sqrMagnitude >= toPoint2.sqrMagnitude) {
                            purgedPoints.Add(point);
                        }
                    }
                }
            }

            foreach (BattleWaypoint point in purgedPoints) {
                connectedPoints.Remove(point);
            }
        }

        public override BasicWaypoint GetNextPoint() {
            return connectedPoints[UnityEngine.Random.Range(0, connectedPoints.Count)];
        }

        private void CheckAllWaypoints(List<BattleWaypoint> suppliedPoints, Action<BattleWaypoint> connectionFailed, Action<BattleWaypoint> connectionSucceeded, Action<Vector3, Vector3> postCheck = null) {
            foreach (BattleWaypoint point in suppliedPoints) {
                if (point != null && point != this) {
                    Vector3 startToEnd = (point.transform.position - transform.position).Flat().normalized;
                    Vector3 curCheckPoint = transform.position;
                    Vector3 prevCheckPoint;
                    int stepCount = 0;
                    while (Vector3.Distance(curCheckPoint.Flat(), point.transform.position.Flat()) > point.radius * 0.5f) {
                        if (Vector3.Distance(curCheckPoint, point.transform.position) < point.radius && !Physics.Linecast(curCheckPoint, point.transform.position, LayerInfo.AllExcludingKarts, QueryTriggerInteraction.Ignore)) {
                            connectionSucceeded?.Invoke(point);
                            break;
                        }

                        prevCheckPoint = curCheckPoint;
                        RaycastHit hit = new RaycastHit();
                        if (Physics.Raycast(curCheckPoint, startToEnd, out hit, connectionStepDistance, LayerInfo.AllExcludingKarts, QueryTriggerInteraction.Ignore)) {
                            if (SteepnessCheck(hit)) {
                                connectionFailed?.Invoke(point);
                                break;
                            }
                            curCheckPoint += StraightProjection(startToEnd, hit.normal).normalized * connectionStepDistance + Vector3.up * 0.1f;
                        }
                        else if (Physics.Raycast(curCheckPoint, Vector3.down, out hit, Mathf.Infinity, LayerInfo.AllExcludingKarts, QueryTriggerInteraction.Ignore)) {
                            if (SteepnessCheck(hit)) {
                                connectionFailed?.Invoke(point);
                                break;
                            }

                            RaycastHit hit2 = new RaycastHit();
                            if (Physics.Raycast(hit.point + Vector3.up * 0.1f, StraightProjection(startToEnd, hit.normal).normalized, out hit2, connectionStepDistance, LayerInfo.AllExcludingKarts, QueryTriggerInteraction.Ignore)) {
                                if (SteepnessCheck(hit2)) {
                                    connectionFailed?.Invoke(point);
                                    break;
                                }

                                curCheckPoint = hit2.point + Vector3.up * 0.1f;
                            }
                            else {
                                curCheckPoint = hit.point + StraightProjection(startToEnd, hit.normal).normalized * connectionStepDistance + Vector3.up * 0.1f;
                            }
                        }
                        else {
                            connectionFailed?.Invoke(point);
                            break;
                        }

                        postCheck?.Invoke(prevCheckPoint, curCheckPoint);

                        stepCount++;
                        if (stepCount > maxConnectionSteps) {
                            break;
                        }
                    }

                    if (Physics.Linecast(curCheckPoint, point.transform.position, LayerInfo.AllExcludingKarts, QueryTriggerInteraction.Ignore)) {
                        connectionFailed?.Invoke(point);
                    }
                }
            }

            bool SteepnessCheck(RaycastHit hit) {
                return Vector3.Dot(hit.normal, Vector3.up) < 1.0f - maxConnectionSteepness;
            }
        }

        private Vector3 StraightProjection(Vector3 vector, Vector3 planeNormal) {
            return new Vector3(vector.x, vector.magnitude * Mathf.Tan(Vector3.Angle(planeNormal, Vector3.up) * Mathf.Deg2Rad), vector.z);
        }

        Vector3 InterpolateSine(Vector3 start, Vector3 end, float t, float sineAmpAdd = 0.0f) {
            float sine = Mathf.Sin(t * Mathf.PI);
            return Vector3.Lerp(start, end, t) + Vector3.Cross(end - start, Vector3.up).normalized * (sine * Mathf.Sqrt(Vector3.Distance(start, end) + sineAmpAdd)) + Vector3.up * sine * 10f;
        }

        private void DrawConnectionArc(Vector3 start, Vector3 end) {
            for (float i = 0.0f; i <= 0.8f; i += 0.2f) {
                Gizmos.color = new Color(0.0f, Mathf.Pow(i + 0.05f, 0.1f), Mathf.Pow(1.0f - i, 0.4f));
                Gizmos.DrawLine(InterpolateSine(start, end, i), InterpolateSine(start, end, i + 0.2f));
                if (i > 0) {
                    Gizmos.DrawLine(InterpolateSine(start, end, i - 0.05f, 30f), InterpolateSine(start, end, i));
                    Gizmos.DrawLine(InterpolateSine(start, end, i - 0.05f, -30f), InterpolateSine(start, end, i));
                }
            }
        }

        protected override void OnDrawGizmos() {
            Gizmos.color = new Color(0.0f, 1.0f, 1.0f, 0.5f);
            Gizmos.DrawSphere(transform.position, radius);

            // Draw lines to connected points
            foreach (BattleWaypoint point in connectedPoints) {
                if (point != null && point != this) {
                    DrawConnectionArc(transform.position, point.transform.position);
                }
            }
        }

        private void OnDrawGizmosSelected() {
            if (showDebugConnections) {
                CheckAllWaypoints(
                    FindObjectsOfType<BattleWaypoint>().Where(wp => wp != this).ToList(),
                    null,
                    (point) => {
                        DrawConnectionArc(transform.position, point.transform.position);
                    },
                    (prevPoint, curPoint) => {
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(prevPoint, curPoint);
                        Gizmos.DrawWireSphere(curPoint, 0.2f);
                    });
            }
        }
    }
}
