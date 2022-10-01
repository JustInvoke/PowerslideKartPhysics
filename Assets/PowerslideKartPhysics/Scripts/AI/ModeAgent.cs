// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    // Class attached to a kart that participates in a race
    [DisallowMultipleComponent]
    public abstract class ModeAgent : MonoBehaviour
    {
        protected Transform tr;
        protected Rigidbody rb;
        protected Kart kart;
        public BasicWaypoint currentPoint; // The last waypoint touched
        public float respawnHeight = 1.0f; // Height above waypoints to respawn at
        [System.NonSerialized]
        public bool finishedMode = false; // True if the kart has met the conditions for winning or finishing the mode
        [System.NonSerialized]
        public ModeController mc;

        protected virtual void Awake() {
            tr = transform;
            rb = GetComponent<Rigidbody>();
            kart = GetComponent<Kart>();
        }

        // Respawns the kart by moving it to the last touched waypoint
        public virtual void Respawn() {
            if (currentPoint != null) {
                tr.position = currentPoint.transform.position + Vector3.up * respawnHeight;
            }

            if (kart != null) {
                if (kart.rotator != null) {
                    kart.CancelDrift();
                    kart.EmptyBoostReserve();
                    kart.CancelDriftBoost(false);
                    kart.CancelJump();
                }
            }

            if (rb != null) {
                rb.velocity = Vector3.zero;
            }
        }

        // Returns the current position of the kart
        public int GetModePosition() {
            if (mc != null) {
                return mc.GetPositionOfKart(this);
            }
            return -1;
        }
    }
}
