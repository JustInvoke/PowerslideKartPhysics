// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    [CreateAssetMenu(fileName = "Kart Jump Preset", menuName = "Powerslide Kart Physics/Kart Preset/Kart Jump Preset", order = 5)]
    // Class for kart jump presets
    public class KartJumpPreset : ScriptableObject
    {
        public bool canJump = true;
        public float jumpForce = 100f;
        public float jumpDuration = 0.1f;
        public float jumpStickForce = 10f;
        public float airJumpTimeLimit = 0.1f;
    }
}