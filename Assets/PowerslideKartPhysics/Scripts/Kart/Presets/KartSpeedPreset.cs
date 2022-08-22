// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    [CreateAssetMenu(fileName = "Kart Speed Preset", menuName = "Powerslide Kart Physics/Kart Preset/Kart Speed Preset", order = 1)]
    // Class for kart speed presets
    public class KartSpeedPreset : ScriptableObject
    {
        public float maxSpeed = 30f;
        public float maxReverseSpeed = 10f;
        public float acceleration = 1.0f;
        public float brakeForce = 1.5f;
        public float coastingFriction = 0.5f;
        public float slopeFriction = 0.5f;
        public float airDriveFriction = 0.0f;
        public float autoStopSpeed = 1.0f;
        public float autoStopForce = 1.0f;
        public float autoStopNormalDotLimit = 0.9f;
        public float maxFallSpeed = 30f;
        public float spinDecel = 1.0f;
    }
}