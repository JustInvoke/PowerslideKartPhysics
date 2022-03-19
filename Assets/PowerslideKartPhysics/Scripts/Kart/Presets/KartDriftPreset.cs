// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    [CreateAssetMenu(fileName = "Kart Drift Preset", menuName = "Powerslide Kart Physics/Kart Preset/Kart Drift Preset", order = 7)]
    // Class for kart drift presets
    public class KartDriftPreset : ScriptableObject
    {
        public bool canDrift = true;
        public bool canDriftInAir = true;
        public float minDriftAngle = 0.5f;
        public float maxDriftAngle = 1.5f;
        public float visualDriftFactor = 0.5f;
        public float visualDriftAirFactor = 0.5f;
        public float driftSwingDuration = 0.5f;
        public float driftSwingForce = 1.0f;
        public float minDriftSpeed = 5.0f;
        public bool wallCollisionCancelsDrift = true;
        public bool brakeCancelsDrift = false;
        public float burnoutSpeed = 1.0f;
        public float burnoutSpeedLimit = 5.0f;
    }
}