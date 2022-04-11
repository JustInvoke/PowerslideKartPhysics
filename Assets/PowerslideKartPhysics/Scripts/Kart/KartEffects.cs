// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Kart))]
    // Class for controlling kart particle effects
    public class KartEffects : MonoBehaviour
    {
        Kart theKart;
        public ConditionalParticles exhaustParticles;
        public ConditionalParticles exhaustBoostReadyParticles;
        public ConditionalParticles boostParticles;
        public ParticleSystem boostStartParticles;
        public ConditionalParticle[] driftBoostParticles;
        public bool moveParticlesWithDrift = true;

        Vector3[] driftBoostStartPositions;
        Quaternion[] driftBoostStartRotations;
        public ParticleSystem collisionParticles;

        private void Awake() {
            theKart = GetComponent<Kart>();

            // Set exhaust particle condition
            if (exhaustParticles != null) {
                exhaustParticles.condition = () => {
                    if (theKart != null) {
                        return !theKart.IsBoostReady();
                    }
                    else {
                        return false;
                    }
                };
            }

            // Set condition for exhaust particles indicating that the the kart's boost is "ready"
            if (exhaustBoostReadyParticles != null) {
                exhaustBoostReadyParticles.condition = () => {
                    if (theKart != null) {
                        return theKart.IsBoostReady();
                    }
                    else {
                        return false;
                    }
                };
            }

            // Set boost particle condition
            if (boostParticles != null) {
                boostParticles.condition = () => {
                    if (theKart != null) {
                        return theKart.boostReserve > 0;
                    }
                    else {
                        return false;
                    }
                };
            }

            // Positions and rotations of spark particles are tracked in order to swap which side the particles are on based on the drift direction
            if (driftBoostParticles != null) {
                driftBoostStartPositions = new Vector3[driftBoostParticles.Length];
                driftBoostStartRotations = new Quaternion[driftBoostParticles.Length];
                for (int i = 0; i < driftBoostParticles.Length; i++) {
                    if (driftBoostParticles[i] != null) {
                        if (driftBoostParticles[i].particles != null) {
                            driftBoostStartPositions[i] = driftBoostParticles[i].particles.transform.localPosition;
                            driftBoostStartRotations[i] = driftBoostParticles[i].particles.transform.localRotation;
                        }

                        // Set spark particle conditions for the drift auto boost type
                        // These are designed so that each particle represents a different level/tier of boost (see the boostCount variable in the Kart class)
                        ConditionalParticle curParticle = driftBoostParticles[i];
                        driftBoostParticles[i].condition = () => {
                            if (theKart != null) {
                                return theKart.boostType == KartBoostType.DriftAuto && theKart.boostCount == FindDriftBoostParticleIndex(curParticle) + 1;
                            }
                            else {
                                return false;
                            }
                        };
                    }
                }
            }
        }

        private void Update() {
            if (theKart == null) { return; }

            // Update exhaust particle condition
            if (exhaustParticles != null) {
                exhaustParticles.Update();
            }

            // Update exhaust boost ready particle condition
            if (exhaustBoostReadyParticles != null) {
                exhaustBoostReadyParticles.Update();
            }

            // Update boost particle condition
            if (boostParticles != null) {
                boostParticles.Update();
            }

            // Update drift boost particle conditions and transforms
            if (theKart.boostType == KartBoostType.DriftAuto && driftBoostParticles != null) {
                for (int i = 0; i < driftBoostParticles.Length; i++) {
                    if (driftBoostParticles[i] != null) {
                        // Update drift boost particle condition
                        driftBoostParticles[i].Update();

                        // Position the particles on the correct side based on the drift direction
                        if (moveParticlesWithDrift && driftBoostParticles[i].particles != null && driftBoostParticles[i].condition()) {
                            Vector3 localPos = driftBoostStartPositions[i];
                            driftBoostParticles[i].particles.transform.localPosition = new Vector3(Mathf.Abs(localPos.x) * theKart.driftDir, localPos.y, localPos.z);
                            Vector3 localForward = driftBoostStartRotations[i] * Vector3.forward;
                            Vector3 localUp = driftBoostStartRotations[i] * Vector3.up;
                            driftBoostParticles[i].particles.transform.localRotation = Quaternion.LookRotation(new Vector3(Mathf.Abs(localForward.x) * theKart.driftDir, localForward.y, localForward.z), new Vector3(Mathf.Abs(localUp.x) * theKart.driftDir, localUp.y, localUp.z));
                        }
                    }
                }
            }
        }

        // Play the boost start particles
        public void PlayBoostStartParticles() {
            if (boostStartParticles != null) {
                boostStartParticles.Play();
            }
        }

        // Play the collision particles, moving the particle system to the position and rotating it to face the direction
        public void PlayCollisionParticles(Vector3 pos, Vector3 dir) {
            if (collisionParticles != null) {
                collisionParticles.transform.position = pos;
                collisionParticles.transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
                collisionParticles.Play();
            }
        }

        // Returns the index of the given ConditionalParticle in driftBoostParticles if it exists, or -1 if it doesn't
        // This is necessary for the conditions in driftBoostParticles
        public int FindDriftBoostParticleIndex(ConditionalParticle particle) {
            if (driftBoostParticles != null && particle != null) {
                for (int i = 0; i < driftBoostParticles.Length; i++) {
                    if (driftBoostParticles[i] == particle) {
                        return i;
                    }
                }
                return -1;
            }
            else {
                return -1;
            }
        }
    }
}