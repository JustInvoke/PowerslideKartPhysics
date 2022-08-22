// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PowerslideKartPhysics
{
    // Class attached to a kart that participates in a battle
    [DisallowMultipleComponent]
    public class BattleAgent : MonoBehaviour
    {
        Transform tr;
        Rigidbody rb;
        Kart kart;
        BattleController bc;
        public BattleWaypoint currentPoint; // The last waypoint touched
        [System.NonSerialized]
        public int health = -1;
        public int points; // Current battle point score
        [System.NonSerialized]
        public bool finishedBattle = false; // True if the kart has reached the maximum points or run out of health
        public float respawnHeight = 1.0f; // Height above waypoints to respawn at

        private void Awake() {
            tr = transform;
            rb = GetComponent<Rigidbody>();
            kart = GetComponent<Kart>();
            bc = FindObjectOfType<BattleController>();
        }

        private void Start() {
            if (bc != null) {
                bc.FetchAllKarts(); // Find all active karts
            }
        }

        private void OnTriggerEnter(Collider other) {
            BattleWaypoint point = other.GetComponent<BattleWaypoint>();
            if (point != null) {
                currentPoint = point;
            }
        }

        // Respawns the kart by moving it to the last touched waypoint
        public void Respawn() {
            if (currentPoint != null) {
                tr.position = currentPoint.transform.position + Vector3.up * respawnHeight;
                if (kart != null) {
                    if (kart.rotator != null) {
                        kart.rotator.rotation = Quaternion.LookRotation(new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)), Vector3.up);
                        kart.CancelDrift();
                        kart.EmptyBoostReserve();
                        kart.CancelDriftBoost(false);
                        kart.CancelJump();
                    }

                    if (rb != null) {
                        rb.velocity = Vector3.zero;
                    }
                }
            }
        }

        // Comparer for sorting karts by their position in the race
        public class BattleAgentComparer : IComparer<BattleAgent>
        {
            public int Compare(BattleAgent x, BattleAgent y) {
                if (x == null || y == null || x.points == y.points) { return 0; }
                return y.points > x.points ? 1 : -1;
            }
        }
    }
}
