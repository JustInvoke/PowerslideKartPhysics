// Copyright (c) 2022 Justin Couch / JustInvoke
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerslideKartPhysics
{
    [Serializable]
    // This class represents particles that automatically play/stop based on arbitrary conditions (anonymous methods)
    public class ConditionalParticle
    {
        public Func<bool> condition = () => false;
        public ParticleSystem particles;

        public ConditionalParticle() { }

        public ConditionalParticle(ParticleSystem part) {
            particles = part;
        }

        public void Update() {
            // Play or stop particles automatically based on the condition
            if (particles != null) {
                if (condition()) {
                    if (!particles.isEmitting) {
                        particles.Play(true);
                    }
                }
                else if (particles.isEmitting) {
                    particles.Stop(true);
                }
            }
        }
    }

    [Serializable]
    // This class represents particles that automatically play/stop based on arbitrary conditions (anonymous methods)
    // This class is the same as above, except that it supports multiple particle systems (still with one condition)
    public class ConditionalParticles
    {
        public Func<bool> condition = () => false;
        public ParticleSystem[] particles;

        public ConditionalParticles() { }

        public ConditionalParticles(ParticleSystem[] parts) {
            particles = parts;
        }

        public void Update() {
            // Play or stop particles automatically based on the condition
            for (int i = 0; i < particles.Length; i++) {
                if (condition()) {
                    if (!particles[i].isEmitting) {
                        particles[i].Play();
                    }
                }
                else if (particles[i].isEmitting) {
                    particles[i].Stop();
                }
            }
        }
    }

    [Serializable]
    // This class represents looping sounds that automatically play/stop based on arbitrary conditions (anonymous methods)
    public class ConditionalSound
    {
        public Func<bool> condition = () => false;
        public AudioSource snd;
        float volume = 0.0f;
        public float maxVolume = 1.0f;
        public float volumePower = 2.0f;
        public float onRate = 1.0f;
        public float offRate = 1.0f;
        float pitch = 1.0f;
        public float onPitch = 1.0f;
        public float offPitch = 1.0f;
        public float onPitchRate = 1.0f;
        public float offPitchRate = 1.0f;
        bool pitchInverted = false;

        public ConditionalSound() { }

        public ConditionalSound(AudioSource source) {
            snd = source;
        }

        public ConditionalSound(AudioSource source, float maxV) {
            snd = source;
            maxVolume = maxV;
        }

        public ConditionalSound(AudioSource source, float on, float off) {
            snd = source;
            onRate = on;
            offRate = off;
        }

        public ConditionalSound(AudioSource source, float maxV, float on, float off) {
            snd = source;
            maxVolume = maxV;
            onRate = on;
            offRate = off;
        }

        public void Update() {
            pitchInverted = onPitch < offPitch;
            // Play or stop sound automatically based on the condition
            if (snd != null) {
                // Fade volume in or out
                volume = Mathf.Clamp(condition() ? volume + onRate * Time.deltaTime : volume - offRate * Time.deltaTime, 0.0f, maxVolume);
                snd.volume = Mathf.Pow(volume, volumePower);

                // Adjust pitch while fading in or out
                if (!pitchInverted) {
                    pitch = Mathf.Clamp(condition() ? pitch + onPitchRate * Time.deltaTime : pitch - offPitchRate * Time.deltaTime, offPitch, onPitch);
                }
                else {
                    pitch = Mathf.Clamp(condition() ? pitch - onPitchRate * Time.deltaTime : pitch + offPitchRate * Time.deltaTime, onPitch, offPitch);
                }

                snd.pitch = pitch;
                if (volume > 0) {
                    if (!snd.isPlaying) {
                        snd.Play();
                    }
                }
                else if (snd.isPlaying) {
                    snd.Stop();
                }
            }
        }
    }
}