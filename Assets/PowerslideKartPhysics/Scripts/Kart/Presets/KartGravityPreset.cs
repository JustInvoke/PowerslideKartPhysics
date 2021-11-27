// Copyright (c) 2022 Justin Couch / JustInvoke
using UnityEngine;
using System.Collections;

namespace PowerslideKartPhysics
{
    [CreateAssetMenu(fileName = "Kart Gravity Preset", menuName = "Powerslide Kart Physics/Kart Preset/Kart Gravity Preset", order = 6)]
    // Class for kart gravity presets
    public class KartGravityPreset : ScriptableObject
    {
        public float gravityAdd = -10f;
        public Vector3 gravityDir = Vector3.up;
        public bool gravityIsGroundNormal = false;
        public Kart.GravityMode airGravityMode = Kart.GravityMode.Initial;
        public int gravityCastLayers = 8;
        public int gravityCastSegments = 8;
        public float gravityCastRadius = 2.0f;
        public float gravityCastDistance = 1000f;
        public int gravityCastsPerFrame = 10;
        public bool drawGravityCastGizmos = false;
    }
}