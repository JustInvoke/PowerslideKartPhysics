// Copyright (c) 2023 Justin Couch / JustInvoke
using UnityEngine;

namespace PowerslideKartPhysics
{
    [CreateAssetMenu(fileName = "Kart Wheels Preset", menuName = "Powerslide Kart Physics/Kart Preset/Kart Wheels Preset", order = 4)]
    // Class for kart wheel presets
    public class KartWheelsPreset : ScriptableObject
    {
        public LayerMask wheelCastMask = 1;
        public int maxWheelCastHits = 2;
        public bool oneWheelCastPerFrame = false;
        public float groundNormalSmoothRate = 10f;
    }
}