// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    [CreateAssetMenu(fileName = "Kart Suspension Preset", menuName = "Powerslide Kart Physics/Kart Preset/Kart Suspension Preset", order = 3)]
    // Class for kart suspension presets
    public class KartSuspensionPreset : ScriptableObject
    {
        public float springForce = 50f;
        public float springDampening = 0.1f;
        public float springDampVelMin = -1.0f;
        public float springDampVelMax = 1.0f;
        [Range(0.0f, 1.0f)]
        public float compressionSpringFactor = 0.5f;
        public float groundStickForce = 100f;
        [Range(0.0f, 1.0f)]
        public float groundStickCompression = 0.5f;
    }
}