// Copyright (c) 2022 Justin Couch / JustInvoke
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
            foreach (BattleWaypoint point in connectedPoints) {
                if (point != null && point != this) {
                    Vector3 startToEnd = (point.transform.position - transform.position).Flat().normalized;
                    float maxSteepness = 0.5f;
                    float stepDistance = 1.0f;
                    Vector3 curCheckPoint = transform.position;
                    int loopCount = 0;
                    while (Vector3.Distance(curCheckPoint.Flat(), point.transform.position.Flat()) > point.radius * 0.5f) {
                        if (Vector3.Distance(curCheckPoint, point.transform.position) < point.radius && !Physics.Linecast(curCheckPoint, point.transform.position, ~((1 << LayerMask.NameToLayer("Karts")) | (1 << LayerMask.NameToLayer("Kart Box Collider"))), QueryTriggerInteraction.Ignore)) {
                            break;
                        }

                        RaycastHit hit = new RaycastHit();
                        if (Physics.Raycast(curCheckPoint, startToEnd, out hit, stepDistance, ~((1 << LayerMask.NameToLayer("Karts")) | (1 << LayerMask.NameToLayer("Kart Box Collider"))), QueryTriggerInteraction.Ignore)) {
                            if (Vector3.Dot(hit.normal, Vector3.up) < 1.0f - maxSteepness) {
                                purgedPoints.Add(point);
                                break;
                            }
                            curCheckPoint += Vector3.ProjectOnPlane(startToEnd, hit.normal).normalized * stepDistance + hit.normal * 0.1f;
                        }
                        else if (Physics.Raycast(curCheckPoint, Vector3.down, out hit, Mathf.Infinity, ~((1 << LayerMask.NameToLayer("Karts")) | (1 << LayerMask.NameToLayer("Kart Box Collider"))), QueryTriggerInteraction.Ignore)) {
                            RaycastHit hit2 = new RaycastHit();
                            if (Physics.Raycast(hit.point + hit.normal * 0.1f, Vector3.ProjectOnPlane(startToEnd, hit.normal).normalized, out hit2, stepDistance, ~((1 << LayerMask.NameToLayer("Karts")) | (1 << LayerMask.NameToLayer("Kart Box Collider"))), QueryTriggerInteraction.Ignore)) {
                                curCheckPoint = hit2.point + hit2.normal * 0.1f;
                            }
                            else {
                                curCheckPoint = hit.point + Vector3.ProjectOnPlane(startToEnd, hit.normal).normalized * stepDistance + hit.normal * 0.1f;
                            }
                        }
                        else {
                            purgedPoints.Add(point);
                            break;
                            //curCheckPoint += startToEnd * stepDistance;
                        }

                        loopCount++;
                        if (loopCount > 1000) {
                            purgedPoints.Add(point);
                            break;
                        }
                    }

                    if (Physics.Linecast(curCheckPoint, point.transform.position, ~((1 << LayerMask.NameToLayer("Karts")) | (1 << LayerMask.NameToLayer("Kart Box Collider"))), QueryTriggerInteraction.Ignore)) {
                        purgedPoints.Add(point);
                    }
                }
            }

            foreach (BattleWaypoint point in purgedPoints) {
                connectedPoints.Remove(point);
            }
        }

        public override BasicWaypoint GetNextPoint() {
            return connectedPoints[Random.Range(0, connectedPoints.Count)];
        }

        protected override void OnDrawGizmos() {
            Gizmos.color = new Color(0.0f, 1.0f, 1.0f, 0.5f);
            Gizmos.DrawSphere(transform.position, radius);

            // Draw lines to connected points
            foreach (BattleWaypoint point in connectedPoints) {
                if (point != null && point != this) {
                    for (float i = 0.0f; i <= 0.8f; i += 0.2f) {
                        //break;
                        Gizmos.color = new Color(0.0f, Mathf.Pow(i + 0.05f, 0.1f), Mathf.Pow(1.0f - i, 0.4f));
                        Gizmos.DrawLine(InterpolateSine(transform.position, point.transform.position, i), InterpolateSine(transform.position, point.transform.position, i + 0.2f));
                        Gizmos.DrawLine(InterpolateSine(transform.position, point.transform.position, i, 30f), InterpolateSine(transform.position, point.transform.position, i + 0.05f));
                        Gizmos.DrawLine(InterpolateSine(transform.position, point.transform.position, i, -30f), InterpolateSine(transform.position, point.transform.position, i + 0.05f));
                    }
                    continue;
                    Gizmos.color = Color.red;
                    Vector3 startToEnd = (point.transform.position - transform.position).Flat().normalized;
                    float maxSteepness = 0.5f;
                    float stepDistance = 2.0f;
                    Vector3 curCheckPoint = transform.position;
                    Vector3 prevCheckPoint;
                    int loopCount = 0;
                    while (Vector3.Distance(curCheckPoint.Flat(), point.transform.position.Flat()) > point.radius * 0.5f) {
                        if (Vector3.Distance(curCheckPoint, point.transform.position) < point.radius && !Physics.Linecast(curCheckPoint, point.transform.position, ~((1 << LayerMask.NameToLayer("Karts")) | (1 << LayerMask.NameToLayer("Kart Box Collider"))), QueryTriggerInteraction.Ignore)) {
                            Gizmos.color = Color.green;
                            for (float i = 0.0f; i < 0.9f; i += 0.1f) {
                                Gizmos.color = new Color(0.0f, Mathf.Pow(i + 0.05f, 0.1f), Mathf.Pow(1.0f - i, 0.4f));
                                Gizmos.DrawLine(InterpolateSine(transform.position, point.transform.position, i), InterpolateSine(transform.position, point.transform.position, i + 0.1f));
                                Gizmos.DrawLine(InterpolateSine(transform.position, point.transform.position, i, 30f), InterpolateSine(transform.position, point.transform.position, i + 0.05f));
                                Gizmos.DrawLine(InterpolateSine(transform.position, point.transform.position, i, -30f), InterpolateSine(transform.position, point.transform.position, i + 0.05f));
                            }
                            break;
                        }

                        prevCheckPoint = curCheckPoint;
                        RaycastHit hit = new RaycastHit();
                        if (Physics.Raycast(curCheckPoint, startToEnd, out hit, stepDistance, ~((1 << LayerMask.NameToLayer("Karts")) | (1 << LayerMask.NameToLayer("Kart Box Collider"))), QueryTriggerInteraction.Ignore)) {
                            if (Vector3.Dot(hit.normal, Vector3.up) < 1.0f - maxSteepness) {
                                break;
                            }
                            curCheckPoint += Vector3.ProjectOnPlane(startToEnd, hit.normal).normalized * stepDistance + hit.normal * 0.1f;
                        }
                        else if (Physics.Raycast(curCheckPoint, Vector3.down, out hit, Mathf.Infinity, ~((1 << LayerMask.NameToLayer("Karts")) | (1 << LayerMask.NameToLayer("Kart Box Collider"))), QueryTriggerInteraction.Ignore)) {
                            RaycastHit hit2 = new RaycastHit();
                            if (Physics.Raycast(hit.point + hit.normal * 0.1f, Vector3.ProjectOnPlane(startToEnd, hit.normal).normalized, out hit2, stepDistance, ~((1 << LayerMask.NameToLayer("Karts")) | (1 << LayerMask.NameToLayer("Kart Box Collider"))), QueryTriggerInteraction.Ignore)) {
                                curCheckPoint = hit2.point + hit2.normal * 0.1f;
                            }
                            else {
                                curCheckPoint = hit.point + Vector3.ProjectOnPlane(startToEnd, hit.normal).normalized * stepDistance + hit.normal * 0.1f;
                            }
                        }
                        else {
                            break;
                            //curCheckPoint += startToEnd * stepDistance;
                        }
                        Gizmos.DrawLine(prevCheckPoint, curCheckPoint);
                        Gizmos.DrawWireSphere(curCheckPoint, 0.2f);

                        loopCount++;
                        if (loopCount > 200) {
                            //Debug.Log("ouch");
                            break;
                        }
                    }
                }
            }

            Vector3 InterpolateSine(Vector3 start, Vector3 end, float t, float sineAmpAdd = 0.0f) {
                return Vector3.Lerp(start, end, t) + Vector3.Cross(end - start, Vector3.up).normalized * (Mathf.Sin(t * Mathf.PI) * Mathf.Sqrt(Vector3.Distance(start, end) + sineAmpAdd));
            }
        }
    }
}
