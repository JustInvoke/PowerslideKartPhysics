// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Kart))]
    // Class for playing kart sounds
    public class KartAudio : MonoBehaviour
    {
        Kart theKart;
        public bool zeroDoppler = true;
        public AudioSource engineSnd;
        public float enginePitchChangeRate = 10f;
        public float engineMinPitch = 0.5f;
        public float engineMaxPitch = 1.5f;
        public float boostReservePitchIncrease = 0.1f;
        public float engineMinVolume = 0.5f;
        public float engineMaxVolume = 1.0f;
        float airPitchFactor = 1.0f;
        public float minAirPitch = 0.5f;
        public float airPitchDecayRate = 0.1f;

        public AudioSource oneShotSource;
        public AudioClip jumpSnd;
        public AudioClip landSnd;
        public AudioClip[] collisionSnds;
        public float collisionVelocityVolumeScale = 0.1f;
        public ConditionalSound boostSnd;
        public AudioClip boostStartSnd;
        public AudioClip boostFailSnd;
        public AudioClip defaultTireSnd;
        public ConditionalSound tireSnd;
        public AudioClip itemUseSnd;
        public AudioClip itemHitSnd;

        private void Awake() {
            theKart = GetComponent<Kart>();

            if (engineSnd != null) {
                if (engineSnd.clip != null) {
                    // Randomize engine sound time in case multiple karts have the same engine clip
                    engineSnd.time = Random.Range(0.0f, Mathf.Max(0.0f, engineSnd.clip.length - 0.01f));
                }
            }

            // Set boost sound condition
            if (boostSnd != null) {
                boostSnd.condition = () => {
                    if (theKart != null) {
                        return theKart.boostReserve > 0;
                    }
                    else {
                        return false;
                    }
                };
            }

            // Set tire sound condition
            if (tireSnd != null) {
                tireSnd.condition = () => {
                    if (theKart != null) {
                        return theKart.IsWheelSliding();
                    }
                    else {
                        return false;
                    }
                };
            }

            // Set doppler levels of all audio sources to zero if true (good for player-controlled kart)
            if (zeroDoppler) {
                if (engineSnd != null) {
                    engineSnd.dopplerLevel = 0.0f;
                }

                if (oneShotSource != null) {
                    oneShotSource.dopplerLevel = 0.0f;
                }

                if (boostSnd != null) {
                    if (boostSnd.snd != null) {
                        boostSnd.snd.dopplerLevel = 0.0f;
                    }
                }

                if (tireSnd != null) {
                    if (tireSnd.snd != null) {
                        tireSnd.snd.dopplerLevel = 0.0f;
                    }
                }
            }
        }

        private void Update() {
            if (theKart == null) { return; }

            // Calculate engine pitch
            airPitchFactor = theKart.grounded ? 1.0f : Mathf.Max(minAirPitch, airPitchFactor - Time.deltaTime * airPitchDecayRate);
            float targetPitch = Mathf.Clamp01((Mathf.Abs(theKart.localVel.z) / Mathf.Max(theKart.maxSpeed, 0.01f)) * airPitchFactor + theKart.boostReserve * boostReservePitchIncrease + (theKart.burnout ? 1.0f : 0.0f));

            // Set engine pitch and volume
            if (engineSnd != null) {
                engineSnd.pitch = Mathf.Lerp(engineSnd.pitch, engineMinPitch + (engineMaxPitch - engineMinPitch) * targetPitch, Time.deltaTime * enginePitchChangeRate);
                engineSnd.volume = engineMinVolume + (engineMaxVolume - engineMinVolume) * targetPitch;
            }

            // Update boost sound condition
            if (boostSnd != null) {
                boostSnd.Update();
            }

            if (tireSnd != null) {
                if (tireSnd.snd != null) {
                    // Set tire sound clip based on the ground surface type
                    GroundSurfacePreset surface = theKart.GetWheelSurface(true);
                    if (surface != null) {
                        if (surface.tireSnd != null) {
                            tireSnd.snd.clip = surface.tireSnd;
                        }
                        else {
                            tireSnd.snd.clip = defaultTireSnd;
                        }
                    }
                    else {
                        tireSnd.snd.clip = defaultTireSnd;
                    }

                    if (tireSnd.snd.clip == null) {
                        tireSnd.snd.clip = defaultTireSnd;
                    }
                }

                tireSnd.Update();
            }
        }

        // Play the jump sound
        public void PlayJumpSnd() {
            if (oneShotSource != null && jumpSnd != null) {
                oneShotSource.PlayOneShot(jumpSnd, 1.0f);
            }
        }

        // Play the land sound
        public void PlayLandSnd() {
            if (oneShotSource != null && landSnd != null) {
                oneShotSource.PlayOneShot(landSnd, 1.0f);
            }
        }

        // Wrapper for the collision sound playing function
        // The position vector is a dummy parameter for the sake of working with the collision UnityEvent in the Kart class
        public void PlayCollisionSnd(Vector3 pos, Vector3 vel) {
            PlayCollisionSnd(vel.magnitude);
        }

        // Play a random collision sound with the given volume factor
        public void PlayCollisionSnd(float volume) {
            if (oneShotSource != null && collisionSnds != null) {
                if (collisionSnds.Length > 0) {
                    oneShotSource.PlayOneShot(collisionSnds[Random.Range(0, collisionSnds.Length)], Mathf.Clamp01(volume * collisionVelocityVolumeScale));
                }
            }
        }

        // Play the boost start sound
        public void PlayBoostStartSnd() {
            if (oneShotSource != null && boostStartSnd != null) {
                oneShotSource.PlayOneShot(boostStartSnd, 1.0f);
            }
        }

        // Play the boost fail sound
        public void PlayBoostFailSnd() {
            if (oneShotSource != null && boostFailSnd != null) {
                oneShotSource.PlayOneShot(boostFailSnd, 1.0f);
            }
        }

        // Play the sound for using and item
        public void PlayItemUseSnd() {
            if (oneShotSource != null && itemUseSnd != null) {
                oneShotSource.PlayOneShot(itemUseSnd, 1.0f);
            }
        }

        // Play the sound for getting hit by an item
        public void PlayItemHitSnd() {
            if (oneShotSource != null && itemHitSnd != null) {
                oneShotSource.PlayOneShot(itemHitSnd, 1.0f);
            }
        }
    }
}