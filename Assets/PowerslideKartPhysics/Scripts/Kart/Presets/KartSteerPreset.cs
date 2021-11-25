// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    [CreateAssetMenu(fileName = "Kart Steer Preset", menuName = "Powerslide Kart Physics/Kart Preset/Kart Steer Preset", order = 2)]
    // Class for kart steer presets
    public class KartSteerPreset : ScriptableObject
    {
        [Range(0.0f, 1.0f)]
        public float steerRate = 1.0f;
        public float maxSteer = 1.0f;
        public float minSteer = 0.5f;
        public float airSteer = 0.5f;
        public float steerSpeedLimit = 30.0f;
        public float steerSlowLimit = 5.0f;
        public float brakeSteerIncrease = 0.5f;
        public bool dontInvertSteerReverseAccel = true;
        [Range(0.0f, 1.0f)]
        public float visualSteerRate = 0.1f;
        public float visualSteerSpeedLimit = 10f;
        public float turnTiltAmount = 0.0f;
        public float turnTiltReferenceSpeed = 20f;
        [Range(0.0f, 1.0f)]
        public float turnTiltRate = 1.0f;
        public float turnTiltSideOffsetFactor = 1.0f;
        public bool invertTurnTiltHeightOffset = false;
        public float localTiltOffsetCompensation = 0.0f;
        public float accelTiltAmount = 0.0f;
        public float sidewaysFriction = 10f;
        public float airSidewaysFriction = 5.0f;
        public float brakeSlipAmount = 0.0f;
    }
}