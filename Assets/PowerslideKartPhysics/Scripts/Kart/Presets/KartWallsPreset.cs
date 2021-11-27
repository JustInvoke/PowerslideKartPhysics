// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    [CreateAssetMenu(fileName = "Kart Walls Preset", menuName = "Powerslide Kart Physics/Kart Preset/Kart Walls Preset", order = 9)]
    // Class for kart wall collision presets
    public class KartWallsPreset : ScriptableObject
    {
        public float wallFriction = 5.0f;
        public float wallBounceTurnAmount = 0.5f;
        public float wallBounceTurnDecayRate = 20f;
        public float minWallHitSpeed = 5.0f;
        public float wallHitDuration = 0.5f;
        public WallDetectProps wallCollisionProps = WallDetectProps.Default;
        public bool localUpWallDotComparison = true;
    }
}