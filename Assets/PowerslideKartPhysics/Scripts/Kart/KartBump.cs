﻿// Copyright (c) 2022 Justin Couch / JustInvoke
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    [RequireComponent(typeof(Kart))]
    // Class enabling karts to bump/bounce off of each other
    public class KartBump : MonoBehaviour
    {
        Transform tr;
        Rigidbody rb;
        Kart kart;
        public float bumpFactor = 1.0f; // Multiplier for bump force magnitude
        public float minBumpMagnitude = 2.0f; // Minimum bump force
        public float maxBumpMagnitude = 5.0f; // Maximum bump force

        private void Awake() {
            tr = transform;
            rb = GetComponent<Rigidbody>();
            kart = GetComponent<Kart>();
        }

        // Check for collisions with other karts
        private void OnCollisionEnter(Collision collision) {
            foreach (ContactPoint contact in collision.contacts) {
                if (contact.otherCollider.GetComponentInParent<Kart>() != null) {
                    // Bump force is collision magnitude in direction between the karts' origins
                    Bump((tr.position - contact.otherCollider.transform.position).normalized * collision.relativeVelocity.magnitude);
                    break;
                }
            }
        }

        // Applies the force vector, clamped and projected onto the kart's ground normal plane to avoid flying up in the air
        public void Bump(Vector3 force) {
            rb.AddForce(Vector3.ProjectOnPlane(force.normalized * Mathf.Clamp(force.magnitude * bumpFactor, minBumpMagnitude, maxBumpMagnitude), kart.upDir), ForceMode.VelocityChange);
        }
    }
}
